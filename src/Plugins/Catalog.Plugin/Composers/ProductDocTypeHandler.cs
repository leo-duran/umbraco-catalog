using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Catalog.Plugin.Composers;

public class ProductDocTypeHandler : INotificationAsyncHandler<UmbracoApplicationStartingNotification>
{
    const string contentTypeAlias = "product";
    private readonly IShortStringHelper _shortStringHelper;
    private readonly IContentTypeService _contentTypeService;

    public ProductDocTypeHandler(IShortStringHelper shortStringHelper, IContentTypeService contentTypeService)
    {
        _shortStringHelper = shortStringHelper;
        _contentTypeService = contentTypeService;
    }

    public async Task HandleAsync(UmbracoApplicationStartingNotification notification, CancellationToken cancellationToken)
    {
        var contentType = new ContentType(_shortStringHelper, -1)
        {
            Alias = contentTypeAlias,
            Name = "Product",
        };

        var contentTab = new PropertyGroup(new PropertyTypeCollection(true))
        {
            Name = "Content",
            Alias = "content",
            SortOrder = 1,
            Type = PropertyGroupType.Tab
        };
        contentType.PropertyGroups.Add(contentTab);

        var titlePropertyType = new PropertyType(_shortStringHelper, "Umbraco.TextBox", ValueStorageType.Nvarchar, "title")
        {
            Name = "Title",
            Description = "Page title",
            Mandatory = true,
        };
        contentTab.PropertyTypes?.Add(titlePropertyType);


        _contentTypeService.Save(contentType);

        return;
    }
}
