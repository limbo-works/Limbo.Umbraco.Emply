using System.Collections.Generic;
using System.Threading.Tasks;
using Limbo.Umbraco.Emply.PropertyEditors;
using Skybrud.Essentials.Security.Extensions;
using Skybrud.Essentials.Umbraco.Manifests.Extensions;
using Skybrud.Essentials.Umbraco.Manifests.Extensions.Icons;
using Skybrud.Essentials.Umbraco.Manifests.Extensions.Localization;
using Skybrud.Essentials.Umbraco.Manifests.Extensions.PropertyEditors;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.Emply.Manifests;

public class EmplyPackageManifestReader : IPackageManifestReader {

    public static string Alias => EmplyPackage.Alias;

    public static string Name => EmplyPackage.Name;

    public static string CacheBuster = EmplyPackage.InformationalVersion.ToMd5Hash();

    public Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync() {

        IEnumerable<PackageManifest> manifests = [
            new() {
                Id = Alias,
                Name = Name,
                AllowTelemetry = true,
                Version = EmplyPackage.InformationalVersion,
                Extensions = [..GetExtensions()]
            }
        ];

        return Task.FromResult(manifests);

    }

    private static IEnumerable<IExtension> GetExtensions() {

        yield return new LocalizationExtension {
            Alias = $"{Alias}.Localization.EnUs",
            Name = $"{Name}: English (en-US)",
            Js = $"/App_Plugins/{Alias}/Localization/en-US.js?v={CacheBuster}",
            Meta = new LocalizationMeta { Culture = "en" }
        };

        yield return new LocalizationExtension {
            Alias = $"{Alias}.Localization.DaDk",
            Name = $"{Name}: Danish (da-DK)",
            Js = $"/App_Plugins/{Alias}/Localization/da-DK.js?v={CacheBuster}",
            Meta = new LocalizationMeta { Culture = "da" }
        };

        yield return new IconsExtension {
            Alias = $"{Alias}.Icons",
            Name = $"{Name}: Icons",
            Js = $"/App_Plugins/{Alias}/Icons.js?v={CacheBuster}"
        };

        yield return new PropertyEditorSchemaExtension {
            Alias = EmplyJobDataPropertyEditor.EditorAlias,
            Name = $"{Name}: Job Data Property Editor Schema",
            Meta = new PropertyEditorSchemaMeta {
                DefaultPropertyEditorUiAlias = EmplyJobDataPropertyEditor.EditorUiAlias
            }
        };

        yield return new PropertyEditorUiExtension {
            Alias = EmplyJobDataPropertyEditor.EditorUiAlias,
            Name = $"{Name}: Job Data Property Editor UI",
            Element = $"/App_Plugins/{Alias}/Elements/JobData.js?v={CacheBuster}",
            ElementName = "limbo-umbraco-emply-job-data-property-editor",
            Meta = new PropertyEditorUiMeta {
                Label = "Limbo Emply Job Data",
                Icon = "icon-limbo-emply",
                Group = "Limbo",
                PropertyEditorSchemaAlias = EmplyJobDataPropertyEditor.EditorAlias,
                SupportsReadOnly = true
            }
        };

        yield return new PropertyEditorSchemaExtension {
            Alias = EmplyJobIdPropertyEditor.EditorAlias,
            Name = $"{Name}: Job ID Property Editor Schema",
            Meta = new PropertyEditorSchemaMeta {
                DefaultPropertyEditorUiAlias = EmplyJobIdPropertyEditor.EditorUiAlias
            }
        };

        yield return new PropertyEditorUiExtension {
            Alias = EmplyJobIdPropertyEditor.EditorUiAlias,
            Name = $"{Name}: Job ID Property Editor UI",
            Element = $"/App_Plugins/{Alias}/Elements/JobId.js?v={CacheBuster}",
            ElementName = "limbo-umbraco-emply-job-id-property-editor",
            Meta = new PropertyEditorUiMeta {
                Label = "Limbo Emply Job ID",
                Icon = "icon-limbo-emply",
                Group = "Limbo",
                PropertyEditorSchemaAlias = EmplyJobIdPropertyEditor.EditorAlias,
                SupportsReadOnly = true
            }
        };

        yield return new PropertyEditorSchemaExtension {
            Alias = EmplyLastUpdatedPropertyEditor.EditorAlias,
            Name = $"{Name}: Last Updated Property Editor Schema",
            Meta = new PropertyEditorSchemaMeta {
                DefaultPropertyEditorUiAlias = EmplyLastUpdatedPropertyEditor.EditorUiAlias
            }
        };

        yield return new PropertyEditorUiExtension {
            Alias = EmplyLastUpdatedPropertyEditor.EditorUiAlias,
            Name = $"{Name}: Last Updated Property Editor UI",
            Element = $"/App_Plugins/{Alias}/Elements/LastUpdated.js?v={CacheBuster}",
            ElementName = "limbo-umbraco-emply-last-updated-property-editor",
            Meta = new PropertyEditorUiMeta {
                Label = "Limbo Emply Last Updated",
                Icon = "icon-limbo-emply",
                Group = "Limbo",
                PropertyEditorSchemaAlias = EmplyLastUpdatedPropertyEditor.EditorAlias,
                SupportsReadOnly = true
            }
        };

    }

}