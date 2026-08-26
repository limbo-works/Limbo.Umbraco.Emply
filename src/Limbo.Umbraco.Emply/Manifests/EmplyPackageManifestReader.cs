using System.Collections.Generic;
using System.Threading.Tasks;
using Limbo.Umbraco.Emply.PropertyEditors;
using Skybrud.Essentials.Security.Extensions;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.Emply.Manifests;

public class EmplyPackageManifestReader : IPackageManifestReader {

    public static string Alias => EmplyPackage.Alias;

    public static string Name => EmplyPackage.Name;

    public Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync() {

        string cacheBuster = EmplyPackage.InformationalVersion.ToMd5Hash();

        IEnumerable<PackageManifest> manifests = [
            new() {
                Id = Alias,
                Name = Name,
                AllowTelemetry = true,
                Version = EmplyPackage.InformationalVersion,
                Extensions = [
                    new {
                        type = "localization",
                        alias = $"{Alias}.Localization.EnUs",
                        name = $"{Name}: English (en-US)",
                        js = $"/App_Plugins/{Alias}/Localization/en-US.js?v={cacheBuster}",
                        meta = new {
                            culture = "en"
                        }
                    },
                    new {
                        type = "localization",
                        alias = $"{Alias}.Localization.DaDk",
                        name = $"{Name}: Danish (da-DK)",
                        js = $"/App_Plugins/{Alias}/Localization/da-DK.js?v={cacheBuster}",
                        meta = new {
                            culture = "da"
                        }
                    },
                    new {
                        type = "icons",
                        alias = $"{Alias}.Icons",
                        name = $"{Name}: Icons",
                        js = $"/App_Plugins/{Alias}/Icons.js?v={cacheBuster}"
                    },

                    new {
                        type = "propertyEditorSchema",
                        alias = EmplyJobDataPropertyEditor.EditorAlias,
                        name = $"{Name}: Job Data Property Editor Schema",
                        meta = new {
                            defaultPropertyEditorUiAlias = EmplyJobDataPropertyEditor.EditorUiAlias
                        }
                    },

                    new {
                        type = "propertyEditorUi",
                        alias = "Limbo.Umbraco.Emply.PropertyEditorUi.JobData",
                        name = $"{Name}: Job Data Property Editor UI",
                        element = $"/App_Plugins/{Alias}/Elements/JobData.js?v={cacheBuster}",
                        elementName = "limbo-umbraco-emply-job-data-property-editor",
                        meta = new {
                            label = "Limbo Emply Job Data",
                            icon = "icon-limbo-emply",
                            group = "Limbo",
                            propertyEditorSchemaAlias = "Limbo.Umbraco.Emply.JobData",
                            supportsReadOnly = true
                        }
                    },

                    new {
                        type = "propertyEditorSchema",
                        alias = EmplyJobIdPropertyEditor.EditorAlias,
                        name = $"{Name}: Job ID Property Editor Schema",
                        meta = new {
                            defaultPropertyEditorUiAlias = EmplyJobIdPropertyEditor.EditorUiAlias
                        }
                    },

                    new {
                        type = "propertyEditorUi",
                        alias = EmplyJobIdPropertyEditor.EditorUiAlias,
                        name = $"{Name}: Job ID Property Editor UI",
                        element = $"/App_Plugins/{Alias}/Elements/JobId.js?v={cacheBuster}",
                        elementName = "limbo-umbraco-emply-job-id-property-editor",
                        meta = new {
                            label = "Limbo Emply Job ID",
                            icon = "icon-limbo-emply",
                            group = "Limbo",
                            propertyEditorSchemaAlias = EmplyJobIdPropertyEditor.EditorAlias,
                            supportsReadOnly = true
                        }
                    },

                    new {
                        type = "propertyEditorSchema",
                        alias = EmplyLastUpdatedPropertyEditor.EditorAlias,
                        name = $"{Name}: Last Updated Property Editor Schema",
                        meta = new {
                            defaultPropertyEditorUiAlias = EmplyLastUpdatedPropertyEditor.EditorUiAlias
                        }
                    },

                    new {
                        type = "propertyEditorUi",
                        alias = EmplyLastUpdatedPropertyEditor.EditorUiAlias,
                        name = $"{Name}: Last Updated Property Editor UI",
                        element = $"/App_Plugins/{Alias}/Elements/LastUpdated.js?v={cacheBuster}",
                        elementName = "limbo-umbraco-emply-last-updated-property-editor",
                        meta = new {
                            label = "Limbo Emply Last Updated",
                            icon = "icon-limbo-emply",
                            group = "Limbo",
                            propertyEditorSchemaAlias = EmplyLastUpdatedPropertyEditor.EditorAlias,
                            supportsReadOnly = true
                        }
                    }

                ],
                Importmap = null
            }
        ];

        return Task.FromResult(manifests);

    }

}