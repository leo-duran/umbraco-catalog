using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace Catalog.Plugin.Composers;

/// <summary>
/// Composer that registers all the components for the Catalog plugin.
/// </summary>
public class CatalogComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        // Register our notification handlers
        builder
            .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, ProductDocTypeHandler>()
            .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, CatalogPageDocTypeHandler>()
            .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, CatalogContentHandler>();
    }
}