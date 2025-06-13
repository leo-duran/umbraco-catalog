using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace Catalog.Plugin.Composers
{
    public class ProgrammaticDocumentTypeComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // Register our notification handler
            builder
                .AddNotificationHandler<UmbracoApplicationStartingNotification, ProgrammaticDocTypeHandler>();
        }
    }
}
