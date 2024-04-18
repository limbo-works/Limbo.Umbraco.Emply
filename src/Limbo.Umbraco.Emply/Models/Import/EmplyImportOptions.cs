using System;
using Limbo.Umbraco.Emply.Models.Settings;

namespace Limbo.Umbraco.Emply.Models.Import;

public class EmplyImportOptions {

    public string CustomerName { get; init; }

    public string? ApiKey { get; init; }

    public Guid ParentContentKey { get; init; }

    public string ContentTypeAlias { get; init; }

    public bool Write { get; init; }

    public EmplyImportOptions(EmplySourceSettings source) {
        CustomerName = source.CustomerName;
        ApiKey = source.ApiKey;
        ParentContentKey = source.ParentContentKey;
        ContentTypeAlias = source.ContentTypeAlias;
        Write = true;
    }

    public EmplyImportOptions(EmplySourceSettings source, bool write) {
        CustomerName = source.CustomerName;
        ApiKey = source.ApiKey;
        ParentContentKey = source.ParentContentKey;
        ContentTypeAlias = source.ContentTypeAlias;
        Write = write;
    }

}