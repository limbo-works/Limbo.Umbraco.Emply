using System;
using System.Collections.Generic;
using Limbo.Integrations.Emply.Models.Postings;
using Limbo.Umbraco.Emply.Services;
using Skybrud.Essentials.Json.Newtonsoft;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Emply.Factories;

public class EmplyJobDataPropertyIndexValueFactory : IPropertyIndexValueFactory {

    private readonly EmplyJobsService _emplyJobsService;

    public EmplyJobDataPropertyIndexValueFactory(EmplyJobsService emplyJobsService) {
        _emplyJobsService = emplyJobsService;
    }

    public virtual IEnumerable<IndexValue> GetIndexValues(IProperty property, string? culture, string? segment, bool published, IEnumerable<string> availableCultures, IDictionary<Guid, IContentType> contentTypeDictionary) {

        // Get the source value from the property
        object? source = property.GetValue(culture, segment, published);

        // Validate the source value
        if (source is not string json || string.IsNullOrWhiteSpace(json)) yield break;

        // Strip the leading underscore if any
        if (json[0] == '_') json = json[1..];

        // Add the raw JSON value to the index
        yield return new IndexValue {
            Culture = culture,
            FieldName = property.Alias,
            Values = [json]
        };

        // Parse the raw JSON into an 'EmplyPosting' instance
        EmplyPosting posting = JsonUtils.ParseJsonObject(json, EmplyPosting.Parse);

        // Delegate the rest of the work to the jobs service
        foreach (KeyValuePair<string, IEnumerable<object?>> pair in _emplyJobsService.GetIndexValues(property, posting, culture, segment, published)) {
            yield return new IndexValue {
                Culture = culture,
                FieldName = pair.Key,
                Values = pair.Value
            };
        }

    }

}
