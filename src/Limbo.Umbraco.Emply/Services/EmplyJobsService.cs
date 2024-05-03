using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Limbo.Integrations.Emply;
using Limbo.Integrations.Emply.Models.Data.Values;
using Limbo.Integrations.Emply.Models.Jobs;
using Limbo.Integrations.Emply.Models.Postings;
using Limbo.Umbraco.Emply.Constants;
using Limbo.Umbraco.Emply.Extensions;
using Limbo.Umbraco.Emply.Models.Import;
using Limbo.Umbraco.Emply.Models.Settings;
using Limbo.Umbraco.Emply.PropertyEditors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Skybrud.Essentials.Common;
using Skybrud.Essentials.Strings;
using Skybrud.Essentials.Time;
using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace Limbo.Umbraco.Emply.Services;

public class EmplyJobsService {

    private readonly EmplySettings _settings;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IContentTypeService _contentTypeService;
    private readonly IContentService _contentService;

    public EmplyJobsService(IOptions<EmplySettings> settings, IWebHostEnvironment webHostEnvironment, IContentTypeService contentTypeService, IContentService contentService) {
        _settings = settings.Value;
        _webHostEnvironment = webHostEnvironment;
        _contentTypeService = contentTypeService;
        _contentService = contentService;
    }

    #region Public member methods

    public virtual EmplyImportResult Import(EmplySourceSettings source, bool write) {
        return Import(new EmplyImportOptions(source, write));
    }

    public virtual EmplyImportResult Import(EmplyImportOptions options) {

        // Validate the feed
        if (string.IsNullOrWhiteSpace(options.CustomerName)) throw new PropertyNotSetException(nameof(options.CustomerName));
        if (options.ParentContentKey == Guid.Empty) throw new PropertyNotSetException(nameof(options.ParentContentKey));
        if (string.IsNullOrWhiteSpace(options.ContentTypeAlias)) throw new PropertyNotSetException(nameof(options.ContentTypeAlias));

        EmplyImportResult job = new EmplyImportResult($"Importing job items for '{options.CustomerName}'").Start();

        // Get the parent content node
        ImportTask task1 = job.AddTask($"Getting parent node by key '{options.ParentContentKey}'...").Start();
        IContent? parent = _contentService.GetById(options.ParentContentKey);
        if (parent is null) {
            task1.AppendToMessage($"Parent with key '{options.ParentContentKey}' not found.").Failed();
            return job;
        } else {
            task1.AppendToMessage($"Found content node with name '{parent.Name}'...").Completed();
        }

        #region Get content type

        ImportTask task11 = job.AddTask("Getting content type...").Start();

        IContentType? contentType = _contentTypeService.Get(options.ContentTypeAlias);

        if (contentType is null) {
            task11.AppendToMessage($"Content type '{options.ContentTypeAlias}' not found...").Failed();
            return job;
        }

        IPropertyType? idProperty = null;
        IPropertyType? dataProperty = null;
        IPropertyType? lastUpdatedProperty = null;
        IPropertyType? titleProperty = null;

        foreach (IPropertyType propertyType in contentType.PropertyTypes) {

            if (propertyType.Alias == EmplyProperties.JobId) {
                idProperty = propertyType;
            }

            switch (propertyType.PropertyEditorAlias) {
                case EmplyJobDataEditor.EditorAlias: dataProperty = propertyType; break;
                case EmplyLastUpdatedEditor.EditorAlias: lastUpdatedProperty = propertyType; break;
            }

            switch (propertyType.Alias) {

                case "title":
                case "headerTitle":
                case "heroTitle":
                case "introTitle":
                    titleProperty = propertyType;
                    break;

            }

        }

        if (idProperty == null) {
            task11.AppendToMessage($"Required property with property with '{EmplyProperties.JobId}' not found for content type '{contentType.Alias}'.").Failed();
            return job;
        }

        task11.AppendToMessage($"Found Signatur job ID property with alias '{idProperty.Alias}'...");

        if (dataProperty == null) {
            task11.AppendToMessage($"Required property with property editor '{EmplyJobDataEditor.EditorAlias}' not found for content type '{contentType.Alias}'.").Failed();
            return job;
        }

        task11.AppendToMessage($"Found Signatur job data property with alias '{dataProperty.Alias}'...");

        if (lastUpdatedProperty == null) {
            task11.AppendToMessage("Signatur last updated property not found. Skipping as not mandatory...");
        } else {
            task11.AppendToMessage($"Found Signatur last updated property with alias '{lastUpdatedProperty.Alias}'...");
        }

        if (titleProperty == null) {
            task11.AppendToMessage("Title property not found. Skipping as not mandatory...");
        } else {
            task11.AppendToMessage($"Found title property with alias '{titleProperty.Alias}'...");
        }

        EmplyImportJobsSettings settings = new(options, parent, contentType, idProperty, dataProperty, lastUpdatedProperty, titleProperty);

        task11.Completed();

        #endregion

        #region Get existing content/jobs

        ImportTask task2 = job.AddTask("Getting existing jobs from content service...").Start();

        Dictionary<int, IContent> existing = new();

        try {

            // Get all job pages from the content cache
            IEnumerable<IContent> children = _contentService
                .GetPagedChildren(parent.Id, 0, int.MaxValue, out long _);

            // Iterate through the
            foreach (IContent content in children) {

                // Skip the content item if the content type alias doesn't match
                if (content.ContentType.Alias != options.ContentTypeAlias) continue;

                // Get the job ID
                int jobId = content.GetValue<int>(settings.IdProperty.Alias);
                if (jobId == 0) continue;

                // Add the job to the dictionary
                existing[jobId] = content;

            }

            // Append a bit of information to the log
            task2.AppendToMessage($"Found {existing.Count} job pages...").Completed();

        } catch (Exception ex) {

            task2.Failed(ex);

            return job;

        }

        #endregion

        #region Get the jobs from the Emply API

        ImportTask task3 = job.AddTask("Getting jobs from the Emply API...").Start();

        IReadOnlyList<EmplyPosting> postings;
        try {
            postings = EmplyHttpService.Create(options.CustomerName).Postings.GetPostings().Body;
            task3.AppendToMessage($"Found {postings.Count} job items...").Completed();
        } catch (Exception ex) {
            task3.Failed(ex);
            return job;
        }

        #endregion

        #region Add or update the jobs in Umbraco

        ImportTask task4 = job.AddTask("Importing jobs in Umbraco...").Start();

        try {

            foreach (EmplyPosting item in postings) {
                AddOrUpdate(item, settings, task4, existing);
            }

            if (task4.Status == ImportStatus.Failed) return job;

            task4.Completed();

        } catch (Exception ex) {

            task4.Failed(ex);
            return job;

        }

        #endregion

        #region Delete jobs no longer in the feed

        ImportTask task5 = job.AddTask("Deleting jobs in Umbraco...").Start();

        try {

            task5.AppendToMessage(existing.Count == 0
                ? "Found no jobs to delete."
                : $"Found {existing.Count} {StringUtils.ToPlural("job", existing.Count)} to delete.");

            foreach (IContent content in existing.Values) {

                ImportTask deleteTask = task5.AddTask($"Deleting content node with name '{content.Name}'...").Start();

                try {

                    if (options.Write) _contentService.Delete(content, _settings.ImportUserId);

                    deleteTask.Completed();

                } catch (Exception ex) {

                    deleteTask.Failed(ex);

                }

            }

            task5.Completed();

        } catch (Exception ex) {

            task5.Failed(ex);

            return job;

        }

        #endregion

        return job.Completed();



    }

    public virtual List<KeyValuePair<string, IEnumerable<object?>>> GetIndexValues(IProperty property, EmplyPosting item, string? culture, string? segment, bool published) {

        List<KeyValuePair<string, IEnumerable<object?>>> list = new() {
            { $"{property.Alias}_jobId", item.JobId }
        };

        if (item.TryGetData(x => x.Title.ToString() == EmplyAliases.Stillingskategori, out EmplyJobDataType1? categoryData)) {
            foreach (EmplyDataLocalizedValue category in categoryData.Value) {
                list.Add($"{property.Alias}_category", category.Title.ToString());
                list.Add($"{property.Alias}_category_search", category.Id.ToString("N"));
            }
        }

        list.Add($"{property.Alias}_title", item.Title.ToString());
        if (item.DeadlineUtc is not null) list.Add($"{property.Alias}_deadline", item.DeadlineUtc.ToLocalTime().DateTimeOffset);

        return list;

    }

    /// <summary>
    /// Writes the log of the specified <paramref name="job"/> to the disk.
    /// </summary>
    /// <param name="job">The job.</param>
    public virtual void WriteToLog(EmplyImportResult job) {

        string path = Path.Combine(global::Umbraco.Cms.Core.Constants.SystemDirectories.LogFiles, EmplyPackage.Alias, $"{DateTime.UtcNow:yyyyMMddHHmmss}.json");

        string fullPath = _webHostEnvironment.MapPathContentRoot(path);

        // ReSharper disable once AssignNullToNotNullAttribute
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        System.IO.File.AppendAllText(fullPath, JsonConvert.SerializeObject(job), Encoding.UTF8);

    }

    #endregion

    #region Protected member methods

    protected virtual void AddOrUpdate(EmplyPosting item, EmplyImportJobsSettings settings, ImportTask parentTask, Dictionary<int, IContent> existing) {

        ImportTask task = parentTask.AddTask($"Import job item with name '{item.Title}' and ID '{item.JobId}'...").Start();

        // Determine the node name. Umbraco doesn't support node names longer than 255 characters, and will throw an
        // exception if the node name is longer, so we need to truncate the name. As we add the job ID to the end of
        // the name, we also need to take this into account when truncating as we'd otherwise still end up with a
        // node name that is too long
        int maxLength = 255 - item.JobId.ToString().Length - 3;
        string nodeName = $"{item.Title.ToString().Trim()} ({item.JobId})";
        if (nodeName.Length > maxLength) nodeName = $"{nodeName[..(maxLength - 3)]} ({{item.JobId})...";

        try {

            bool isNew = false;

            // If the content doesn't already exist, we create in
            if (!existing.TryGetValue(item.JobId, out IContent? content)) {
                content = _contentService.Create(nodeName, settings.Parent, settings.ContentType.Alias, _settings.ImportUserId);
                isNew = true;
                task.AppendToMessage("Job item not found in Umbraco. Creating new content item...");
            } else {
                task.AppendToMessage($"Job item found in Umbraco with ID '{content.Id}'...");
            }

            // Update the Umbraco properties based on the job item
            bool modified = UpdateProperties(item, content, nodeName, task, settings, content.Id == 0);

            // Save and published the content item if we detecthed any changes
            if (modified) {
                if (settings.Write) _contentService.SaveAndPublish(content, userId: _settings.ImportUserId);
                if (isNew) {
                    task.AppendToMessage($"Successfully created and published content item with ID '{content.Id}'...").SetAction(ImportAction.Added);
                } else {
                    task.AppendToMessage("Successfully saved and published changes...").SetAction(ImportAction.Updated);
                }
            } else if (!isNew) {
                task.AppendToMessage("No new changed detected for job item. Skipping for now...").SetAction(ImportAction.NotModified);
            }

            existing.Remove(item.JobId);

            task.Completed();

        } catch (Exception ex) {

            task.Failed(ex);

        }

    }

    /// <summary>
    /// Updates the properties of a job to be added or updated.
    /// </summary>
    /// <param name="item">An item representing the job item in the Signatur RSS feed.</param>
    /// <param name="content">The <see cref="IContent"/> representing the job in Umbraco.</param>
    /// <param name="nodeName">The node name.</param>
    /// <param name="task">The parent task.</param>
    /// <param name="settings">The settings for this run of the import.</param>
    /// <param name="isNew">Whether <paramref name="content"/> is new - aka the first time the job is being added</param>
    /// <returns><see langword="true"/> if any properties were modified; otherwise, <see langword="false"/>.</returns>
    protected virtual bool UpdateProperties(EmplyPosting item, IContent content, string nodeName, ImportTask task, EmplyImportJobsSettings settings, bool isNew) {

        bool modified = false;

        string? oldName = content.Name;

        // Did the node name change?
        if (oldName != nodeName) {
            content.Name = nodeName;
            modified = true;
        }

        // If the content item hasn't been created yet, we should make sure to set the job ID
        if (content.Id == 0) {
            content.SetValue(settings.IdProperty.Alias, item.JobId);
            modified = true;
        }

        // Has the data changed?
        string? oldData = isNew ? null : content.GetValue<string>(settings.DataProperty.Alias);
        string newData = $"_{item.JObject.ToString(Formatting.None)}";
        SetValueIfModified(content, settings.DataProperty.Alias, oldData, newData, ref modified);

        if (settings.LastUpdatedProperty is not null) {
            if (isNew) {
                content.SetValue(settings.LastUpdatedProperty.Alias, $"_{EssentialsTime.UtcNow.Iso8601}");
            } else {
                string? value = content.GetValue<string>(settings.LastUpdatedProperty.Alias);
                if (string.IsNullOrWhiteSpace(value)) {
                    modified = true;
                }
                content.SetValue(settings.LastUpdatedProperty.Alias, $"_{EssentialsTime.UtcNow.Iso8601}");
            }
        }

        if (settings.TitleProperty is not null) {
            string? oldTitle = content.GetValue<string>(settings.TitleProperty.Alias);
            string newTitle = item.Title.ToString().Trim();
            SetValueIfModified(content, settings.TitleProperty, oldTitle, newTitle, ref modified);
        }

        // Return whether the content item was modified
        return modified || content.Id == 0;

    }

    protected void SetValueIfModified<T>(IContent content, IPropertyType property, T? oldValue, T? newValue, ref bool modified) {
        SetValueIfModified(content, property.Alias, oldValue, newValue, ref modified);
    }

    protected void SetValueIfModified<T>(IContent content, string propertyAlias, T? oldValue, T? newValue, ref bool modified) {
        if (Equals(oldValue, newValue)) return;
        content.SetValue(propertyAlias, newValue);
        modified = true;
    }

    protected virtual string StripHtml(string html) {
        return string.IsNullOrWhiteSpace(html) ? string.Empty : Regex.Replace(html, "<.*?>", " ");
    }

    #endregion

}
