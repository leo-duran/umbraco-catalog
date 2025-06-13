# Catalog Page Document Type Implementation

## Overview
In this session, we created a `CatalogPageDocTypeHandler` to generate a Catalog Page document type in Umbraco using our custom DocTypeBuilder library. This document type will serve as a template for catalog pages that display products.

## Implementation Details

### 1. CatalogPageDocTypeHandler Class

```csharp
public class CatalogPageDocTypeHandler : INotificationAsyncHandler<UmbracoApplicationStartingNotification>
{
    const string contentTypeAlias = "catalogPage";
    private readonly IShortStringHelper _shortStringHelper;
    private readonly IContentTypeService _contentTypeService;
    private readonly ICoreScopeProvider _scopeProvider;
    private readonly ITemplateService _templateService;
    private readonly IFileService _fileService;

    public CatalogPageDocTypeHandler(
        IShortStringHelper shortStringHelper, 
        IContentTypeService contentTypeService, 
        ICoreScopeProvider scopeProvider,
        ITemplateService templateService,
        IFileService fileService)
    {
        _shortStringHelper = shortStringHelper;
        _contentTypeService = contentTypeService;
        _scopeProvider = scopeProvider;
        _templateService = templateService;
        _fileService = fileService;
    }

    public async Task HandleAsync(UmbracoApplicationStartingNotification notification, CancellationToken cancellationToken)
    {
        using (var scope = _scopeProvider.CreateCoreScope())
        {
            // Implementation details...
            
            scope.Complete();
        }
        
        return;
    }
}
```

### 2. Document Type Configuration

The Catalog Page document type includes:

- **Basic Properties**:
  - Alias: "catalogPage"
  - Name: "Catalog Page"
  - Description: "A page that displays a catalog of products"
  - Icon: "icon-shopping-basket-alt"
  - Allowed at root: true

- **Content Tab**:
  - Title (TextBox, mandatory)
  - Description (TextArea)
  - Hero Image (MediaPicker3)

- **Products Tab**:
  - Featured Products (MultiNodeTreePicker)
  - Products Per Page (Integer, mandatory)

### 3. Template Creation

We also created a template for the Catalog Page:

```csharp
private string GetDefaultTemplateContent()
{
    return @"@using Umbraco.Cms.Web.Common.PublishedModels;
@inherits Umbraco.Cms.Web.Common.Views.UmbracoViewPage<ContentModels.CatalogPage>
@using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;

@{
    Layout = ""_Layout.cshtml"";
    ViewData[""Title""] = Model.Title;
}

<div class=""catalog-page"">
    <header class=""catalog-header"">
        <h1>@Model.Title</h1>
        @if (!string.IsNullOrEmpty(Model.Description))
        {
            <div class=""catalog-description"">@Model.Description</div>
        }
        @if (Model.HeroImage != null)
        {
            <div class=""catalog-hero"">
                <img src=""@Model.HeroImage.Url()"" alt=""@Model.Title"" />
            </div>
        }
    </header>

    @if (Model.FeaturedProducts != null && Model.FeaturedProducts.Any())
    {
        <section class=""featured-products"">
            <h2>Featured Products</h2>
            <div class=""product-grid"">
                @foreach (var product in Model.FeaturedProducts)
                {
                    <div class=""product-card"">
                        <h3>@product.Name</h3>
                        <!-- Add product details here -->
                    </div>
                }
            </div>
        </section>
    }

    <section class=""product-catalog"">
        <h2>All Products</h2>
        <!-- Product listing would go here -->
    </section>
</div>";
}
```

### 4. Registration in CatalogPluginComposer

We registered the handler in the CatalogPluginComposer:

```csharp
public class CatalogPluginComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        // Register our notification handlers
        builder
            // .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, ContentTypeCreator>();
            .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, ProductDocTypeHandler>()
            .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, CatalogPageDocTypeHandler>();
    }
}
```

## Benefits

1. **Structured Content**: The Catalog Page document type provides a structured way to create catalog pages in Umbraco.

2. **Separation of Concerns**: Content is separated from presentation, making it easier to maintain.

3. **Reusability**: The document type can be reused for different catalog pages throughout the site.

4. **User-Friendly**: Content editors can easily create and edit catalog pages without needing technical knowledge.

5. **Builder Pattern**: Uses our custom DocTypeBuilder library for clean, readable code.

## Next Steps

1. **Create Content**: Use the Catalog Page document type to create actual catalog pages in Umbraco.

2. **Style the Template**: Add CSS to style the catalog page template.

3. **Implement Pagination**: Add functionality for paginating through products.

4. **Filtering and Sorting**: Add options for filtering and sorting products.

5. **Product Detail Pages**: Create a complementary Product Detail document type for individual product pages. 