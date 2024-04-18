using Limbo.Umbraco.Emply.Models.Import;
using Umbraco.Cms.Core.Models;

namespace Limbo.Umbraco.Emply.Models.Settings;

public class EmplyImportJobsSettings {

    public EmplyImportOptions Options { get; }

    public IContent Parent { get; }

    public IContentType ContentType { get; }

    public IPropertyType IdProperty { get; }

    public IPropertyType DataProperty { get; }

    public IPropertyType? LastUpdatedProperty { get; }

    public IPropertyType? TitleProperty { get; }

    public bool Write => Options.Write;

    public EmplyImportJobsSettings(EmplyImportOptions options, IContent parent,
        IContentType contentType, IPropertyType idProperty, IPropertyType dataProperty,
        IPropertyType? lastUpdatedProperty, IPropertyType? titleProperty) {
        Options = options;
        Parent = parent;
        ContentType = contentType;
        IdProperty = idProperty;
        DataProperty = dataProperty;
        LastUpdatedProperty = lastUpdatedProperty;
        TitleProperty = titleProperty;
    }

}