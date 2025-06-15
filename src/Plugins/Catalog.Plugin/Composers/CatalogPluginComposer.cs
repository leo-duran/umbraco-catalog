using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace Catalog.Plugin.Composers
{
    public class CatalogPluginComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // Register handlers in the correct order - compositions first, then document types
            builder.AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, ContentSettingsCompositionHandler>();
            builder.AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, CatalogPageDocTypeHandler>();
            builder.AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, ProductDocTypeHandler>();
            // builder.AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, AboutUsDocTypeHandler>();
        }
    }
}
