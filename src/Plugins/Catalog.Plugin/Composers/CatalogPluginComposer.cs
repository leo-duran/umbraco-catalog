using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Catalog.Plugin.Handlers;

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
                .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, PropertiesCompositionHandler>()
                .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, ProductDocTypeHandler>()
                .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, PageDocTypeHandler>();
        }
    }
}
