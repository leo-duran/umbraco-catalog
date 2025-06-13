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
            // Register our notification handlers
            // Make sure ProductDocTypeHandler runs first, then CatalogPageDocTypeHandler
            // This is important because CatalogPageDocTypeHandler might have a dependency on the Product document type
            builder
                // .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, ContentTypeCreator>();
                .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, ProductDocTypeHandler>()
                .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, CatalogPageDocTypeHandler>();
        }
    }
}
