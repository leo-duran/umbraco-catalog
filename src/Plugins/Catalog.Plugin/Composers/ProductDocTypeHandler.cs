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
                .WithAlias(contentTypeAlias)
                .WithName("Product")
                .WithDescription("A product document type")
                .WithIcon("icon-shopping-basket")
                .AddTab("Content", tab => tab
                    .WithAlias("content")
                    .WithSortOrder(1)
                    .AddTextBoxProperty("Title", "title", property => property
                        .WithDescription("Page title")
                        .IsMandatory()
                        .WithValueStorageType(ValueStorageType.Nvarchar))
                    .AddTextAreaProperty("Description", "description")
                    .AddNumericProperty("Price", "price", property => property
                        .IsMandatory())
                    .AddMediaPickerProperty("Product Image", "productImage"));

            var contentType = contentTypeBuilder.Build();
            _contentTypeService.Save(contentType);

            scope.Complete();
        }
        return;
    }
}
