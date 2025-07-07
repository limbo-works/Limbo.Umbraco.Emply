using System.Collections.Generic;
using Umbraco.Cms.Core.Manifest;

namespace Limbo.Umbraco.Emply;

/// <inheritdoc />
public class EmplyManifestFilter : IManifestFilter {

    /// <inheritdoc />
    public void Filter(List<PackageManifest> manifests) {

        // Initialize a new manifest filter for this package
        PackageManifest manifest = new() {
            AllowPackageTelemetry = true,
            PackageId = EmplyPackage.Alias,
            PackageName = EmplyPackage.Name,
            Version = EmplyPackage.InformationalVersion,
            Scripts = [
                "/App_Plugins/Limbo.Umbraco.Emply/Scripts/Controllers/JobData.js",
                "/App_Plugins/Limbo.Umbraco.Emply/Scripts/Controllers/Timestamp.js"
            ],
            Stylesheets = [
                "/App_Plugins/Limbo.Umbraco.Emply/Styles/Styles.css"
            ]
        };

        // Append the manifest
        manifests.Add(manifest);

    }

}