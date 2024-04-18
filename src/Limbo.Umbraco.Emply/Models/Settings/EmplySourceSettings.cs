using System;
using Skybrud.Essentials.Strings.Extensions;

namespace Limbo.Umbraco.Emply.Models.Settings;

public class EmplySourceSettings {

    public string CustomerName { get; }

    public string? ApiKey { get; }

    public Guid ParentContentKey { get; }

    public string ContentTypeAlias { get; }

    public EmplySourceSettings(string customerName, string? apiKey, string? parentContentKey, string contentTypeAlias) {
        CustomerName = customerName;
        ApiKey = apiKey.NullIfWhiteSpace();
        if (!Guid.TryParse(parentContentKey, out Guid parentContentKeyGuid)) throw new ArgumentException("Value is not a valid GUID.", nameof(parentContentKeyGuid));
        ParentContentKey = parentContentKeyGuid;
        ContentTypeAlias = contentTypeAlias;
    }

    public EmplySourceSettings(string customerName, string? apiKey, Guid parentContentKey, string contentTypeAlias) {
        CustomerName = customerName;
        ApiKey = apiKey;
        ParentContentKey = parentContentKey;
        ContentTypeAlias = contentTypeAlias;
    }

}