using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.DocTypeBuilder;

namespace Catalog.Plugin.Composers;

public class ProductDocTypeHandler : INotificationAsyncHandler<UmbracoApplicationStartingNotification>
{
    const string contentTypeAlias = "product";
    private readonly IShortStringHelper _shortStringHelper;
    private readonly IContentTypeService _contentTypeService;
    private readonly ICoreScopeProvider _scopeProvider;

    public ProductDocTypeHandler(IShortStringHelper shortStringHelper, IContentTypeService contentTypeService, ICoreScopeProvider scopeProvider)
    {
        _shortStringHelper = shortStringHelper;
        _contentTypeService = contentTypeService;
        _scopeProvider = scopeProvider;
    }

    public async Task HandleAsync(UmbracoApplicationStartingNotification notification, CancellationToken cancellationToken)
    {
        using (var scope = _scopeProvider.CreateCoreScope())
        {

            var contentTypeBuilder = new DocumentTypeBuilder(_shortStringHelper)
                .SetAlias(contentTypeAlias)
                .SetName("Product")
                .SetDescription("A product document type")
                .SetIcon("icon-shopping-basket")
                .AddTab("Content", tab => tab
                    .SetSortOrder(1)
                    .AddTextBoxProperty("title", "Title", property => property
                        .SetDescription("Page title")
                        .SetMandatory(true)
                        .SetValueStorageType(ValueStorageType.Nvarchar))
                    .AddTextAreaProperty("description", "Description")
                    .AddIntegerProperty("price", "Price", property => property
                        .SetMandatory(true))
                    .AddMediaPickerProperty("productImage", "Product Image"));

            var contentType = contentTypeBuilder.Build();
            _contentTypeService.Save(contentType);

            scope.Complete();
        }
        return;
    }
}
