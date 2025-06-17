using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace Catalog.Plugin.Composers
{
    /// <summary>
    /// Composer that registers all the components for the Catalog plugin.
    /// </summary>
    public class CatalogPluginComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // Register our notification handlers in the correct order
            builder
                // Register ContentSettingsCompositionHandler first since it's a composition
                .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, ContentSettingsCompositionHandler>()
                .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, ProductDocTypeHandler>();
            // .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, CatalogPageDocTypeHandler>()
            // .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, CatalogContentHandler>();
            // Uncomment when AboutUsDocTypeHandler is implemented
            // .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, AboutUsDocTypeHandler>();
        }
    }
}
