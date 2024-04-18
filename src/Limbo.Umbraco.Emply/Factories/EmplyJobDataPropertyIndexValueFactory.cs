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

    public virtual IEnumerable<KeyValuePair<string, IEnumerable<object?>>> GetIndexValues(IProperty property, string? culture, string? segment, bool published) {

        // Get the source value from the property
        object? source = property.GetValue(culture, segment, published);

        // Validate the source value
        if (source is not string json || string.IsNullOrWhiteSpace(json)) yield break;

        // Strip the leading underscore if any
        if (json[0] == '_') json = json[1..];

        // Add the property value (XML serialized string) to the index
        yield return new KeyValuePair<string, IEnumerable<object?>>(property.Alias, new[] { json });

        // Parse the raw JSON into an 'EmplyPosting' instance
        EmplyPosting posting = JsonUtils.ParseJsonObject(json, EmplyPosting.Parse);

        // Delegate the rest of the work to the jobs service
        foreach (var pair in _emplyJobsService.GetIndexValues(property, posting, culture, segment, published)) {
            yield return pair;
        }

    }

}