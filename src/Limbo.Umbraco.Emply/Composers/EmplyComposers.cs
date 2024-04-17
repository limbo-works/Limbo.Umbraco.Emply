using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Limbo.Umbraco.Emply.Composers;

/// <inheritdoc />
public class EmplyComposer : IComposer {

    /// <inheritdoc />
    public void Compose(IUmbracoBuilder builder) {

        builder.ManifestFilters().Append<EmplyManifestFilter>();

    }

}