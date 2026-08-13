using System.Collections.Generic;
using System.Threading.Tasks;
using Skybrud.Essentials.Security.Extensions;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.Emply.Manifests;

public class EmplyPackageManifestReader : IPackageManifestReader {

    public Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync() {

        const string packageAlias = EmplyPackage.Alias;

        string cacheBuster = EmplyPackage.InformationalVersion.ToMd5Hash();
        string packagePath = $"/App_Plugins/{packageAlias}";

        IEnumerable<PackageManifest> manifests = [
            new PackageManifest {
                Id = packageAlias,
                Name = EmplyPackage.Name,
                AllowTelemetry = true,
                Version = EmplyPackage.InformationalVersion,
                Extensions = [

                    new {
                        type = "localization",
                        alias = "Limbo.Umbraco.Emply.Localization.EnUs",
                        name = "English",
                        js = $"{packagePath}/Localization/en-US.js?v={cacheBuster}",
                        meta = new {
                            culture = "en"
                        }
                    },

                    new {
                        type = "localization",
                        alias = "Limbo.Umbraco.Emply.Localization.DaDk",
                        name = "Danish",
                        js = $"{packagePath}/Localization/da-DK.js?v={cacheBuster}",
                        meta = new {
                            culture = "da"
                        }
                    },

                    new {
                        type = "icons",
                        alias = "Limbo.Umbraco.Emply.Icons",
                        name = "Limbo Emply Icons",
                        js = $"{packagePath}/Icons.js?v={cacheBuster}"
                    },

                    new {
                        type = "propertyEditorSchema",
                        alias = "Limbo.Umbraco.Emply.JobData",
                        name = "Limbo Emply Job Data",
                        meta = new {
                            defaultPropertyEditorUiAlias = "Limbo.Umbraco.Emply.PropertyEditorUi.JobData"
                        }
                    },

                    new {
                        type = "propertyEditorUi",
                        alias = "Limbo.Umbraco.Emply.PropertyEditorUi.JobData",
                        name = "Limbo Emply Job Data Property Editor UI",
                        element = $"{packagePath}/Elements/JobData.js?v={cacheBuster}",
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
                        alias = "Limbo.Umbraco.Emply.JobId",
                        name = "Limbo Emply Job ID",
                        meta = new {
                            defaultPropertyEditorUiAlias = "Limbo.Umbraco.Emply.PropertyEditorUi.JobId"
                        }
                    },

                    new {
                        type = "propertyEditorUi",
                        alias = "Limbo.Umbraco.Emply.PropertyEditorUi.JobId",
                        name = "Limbo Emply Job ID Property Editor UI",
                        element = $"{packagePath}/Elements/JobId.js?v={cacheBuster}",
                        elementName = "limbo-umbraco-emply-job-id-property-editor",
                        meta = new {
                            label = "Limbo Emply Job ID",
                            icon = "icon-limbo-emply",
                            group = "Limbo",
                            propertyEditorSchemaAlias = "Limbo.Umbraco.Emply.JobId",
                            supportsReadOnly = true
                        }
                    },

                    new {
                        type = "propertyEditorSchema",
                        alias = "Limbo.Umbraco.Emply.LastUpdated",
                        name = "Limbo Emply Last Updated",
                        meta = new {
                            defaultPropertyEditorUiAlias = "Limbo.Umbraco.Emply.PropertyEditorUi.LastUpdated"
                        }
                    },

                    new {
                        type = "propertyEditorUi",
                        alias = "Limbo.Umbraco.Emply.PropertyEditorUi.LastUpdated",
                        name = "Limbo Emply Last Updated Property Editor UI",
                        element = $"{packagePath}/Elements/LastUpdated.js?v={cacheBuster}",
                        elementName = "limbo-umbraco-emply-last-updated-property-editor",
                        meta = new {
                            label = "Limbo Emply Last Updated",
                            icon = "icon-limbo-emply",
                            group = "Limbo",
                            propertyEditorSchemaAlias = "Limbo.Umbraco.Emply.LastUpdated",
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