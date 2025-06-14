using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.DocTypeBuilder;

namespace Catalog.Plugin.Composers;

public class CatalogPageDocTypeHandler : INotificationAsyncHandler<UmbracoApplicationStartingNotification>
{
    const string contentTypeAlias = "catalogPage";
    const string templateAlias = "catalogPageTemplate";
    const string templateName = "Catalog Page Template";
    private readonly IShortStringHelper _shortStringHelper;
    private readonly IContentTypeService _contentTypeService;
    private readonly ICoreScopeProvider _scopeProvider;
    private readonly IFileService _fileService;
    private readonly ILogger<CatalogPageDocTypeHandler> _logger;
    private readonly IDataTypeService _dataTypeService;

    public CatalogPageDocTypeHandler(
        IShortStringHelper shortStringHelper,
        IContentTypeService contentTypeService,
        ICoreScopeProvider scopeProvider,
        IFileService fileService,
        ILogger<CatalogPageDocTypeHandler> logger,
        IDataTypeService dataTypeService)
    {
        _shortStringHelper = shortStringHelper;
        _contentTypeService = contentTypeService;
        _scopeProvider = scopeProvider;
        _fileService = fileService;
        _logger = logger;
        _dataTypeService = dataTypeService;
    }

    public async Task HandleAsync(UmbracoApplicationStartingNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting CatalogPageDocTypeHandler.HandleAsync");

            using (var scope = _scopeProvider.CreateCoreScope())
            {
                try
                {
                    // CreateHomePageType();

                    // Check if content type already exists
                    _logger.LogInformation("Checking if content type already exists: {ContentTypeAlias}", contentTypeAlias);
                    var existingContentType = _contentTypeService.Get(contentTypeAlias);
                    if (existingContentType != null)
                    {
                        _logger.LogInformation("Content type already exists: {ContentTypeAlias}, ID: {ContentTypeId}", contentTypeAlias, existingContentType.Id);
                        scope.Complete();
                        return;
                    }

                    // Create the document type with template support
                    _logger.LogInformation("Creating document type: {ContentTypeAlias}", contentTypeAlias);

                    CreateSimplePage(contentTypeAlias, "Catalog Page", "A page that displays a catalog of products", templateAlias, templateName);

                    _logger.LogInformation("Completing scope");
                    scope.Complete();
                    _logger.LogInformation("Scope completed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in CatalogPageDocTypeHandler scope: {Message}", ex.Message);
                    throw;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CatalogPageDocTypeHandler.HandleAsync: {Message}", ex.Message);
            throw;
        }

        return;
    }

    private void CreateHomePageType()
    {
        string homePageAlias = "homePage";
        string homePageName = "Home Page";
        string homePageDescription = "Home page for the catalog website";
        string homePageTemplateAlias = "homePageTemplate";
        string homePageTemplateName = "Home Page Template";

        // check for existing home page
        var existingHomePage = CheckForExistingContentType(homePageAlias);
        if (existingHomePage != null)
        {
            _logger.LogInformation("{HomePageName} already exists: {HomePageAlias}", homePageName, homePageAlias);
            return;
        }

        var homePageTemplateContent = GetHomePageTemplateContent();

        var homePageBuilder = new DocumentTypeBuilder(_shortStringHelper, _fileService)
            .WithAlias(homePageAlias)
            .WithName(homePageName)
            .WithDescription(homePageDescription)
            .WithIcon("icon-home")
            .AddTab("Content", tab => tab
                .WithAlias("content")
                .WithSortOrder(1)
                .AddTextBoxProperty("Title", "title", property => property
                    .WithDescription("Page title")
                    .IsMandatory()
                    .WithValueStorageType(ValueStorageType.Nvarchar)))
            .AllowAtRoot(true)
            .WithTemplate(homePageTemplateAlias, homePageTemplateName, homePageTemplateContent);


        _logger.LogInformation("Building content type");
        var contentType = homePageBuilder.Build();

        // Enable ModelsBuilder for this content type
        // _logger.LogInformation("Enabling ModelsBuilder for content type");
        // contentType.IsElement = false;
        // contentType.AllowedAsRoot = true;

        // Save the document type
        _logger.LogInformation("Saving content type: {ContentTypeAlias}", homePageAlias);
        _contentTypeService.Save(contentType);
        _logger.LogInformation("Content type saved successfully: {ContentTypeAlias}", homePageAlias);

    }

    private IContentType CheckForExistingContentType(string homePageAlias)
    {
        return _contentTypeService.Get(homePageAlias);
    }

    private string GetHomePageTemplateContent()
    {
        return @"@using Umbraco.Cms.Web.Common.PublishedModels;
@inherits Umbraco.Cms.Web.Common.Views.UmbracoViewPage<ContentModels.HomePage>
@using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;

@{
    Layout = ""_Layout.cshtml"";
}

<div class=""home-page"">
    <h1>@Model.Title</h1>
</div>
";
    }

    private void CreateSimplePage(string pageAlias, string title, string description, string pageTemplateAlias, string pageTemplateName)
    {
        // Get the template content
        string templateContent = GetDefaultTemplateContent();

        _logger.LogInformation("Creating document type with template: {TemplateAlias}", pageTemplateAlias);
        var contentTypeBuilder = new DocumentTypeBuilder(_shortStringHelper, _fileService)
            .WithAlias(pageAlias)
            .WithName(title)
            .WithDescription(description)
            .WithIcon("icon-shopping-basket-alt")
            .AddTab("Content", tab => tab
                .WithAlias("content")
                .WithSortOrder(1)
                .AddTextBoxProperty("Title", "title", property => property
                    .WithDescription("Page title")
                    .IsMandatory()
                    .WithValueStorageType(ValueStorageType.Nvarchar)))
            .AllowAtRoot(true)
            .WithTemplate(pageTemplateAlias, pageTemplateName, templateContent);

        // Build the content type
        _logger.LogInformation("Building content type");
        var contentType = contentTypeBuilder.Build();

        // Enable ModelsBuilder for this content type
        _logger.LogInformation("Enabling ModelsBuilder for content type");
        contentType.IsElement = false;
        contentType.AllowedAsRoot = true;

        // Save the document type
        _logger.LogInformation("Saving content type: {ContentTypeAlias}", pageAlias);
        _contentTypeService.Save(contentType);
        _logger.LogInformation("Content type saved successfully: {ContentTypeAlias}", pageAlias);
    }

    private string GetDefaultTemplateContent()
    {
        return @"@using Umbraco.Cms.Web.Common.PublishedModels;
@inherits Umbraco.Cms.Web.Common.Views.UmbracoViewPage<ContentModels.CatalogPage>
@using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;

@{
    Layout = ""_Layout.cshtml"";
    ViewData[""Title""] = Model.Name;
}

<div class=""catalog-page"">
    <header class=""catalog-header"">
        <h1>@Model.Name</h1>
        @if (Model.HasValue(""description""))
        {
            <div class=""catalog-description"">@Model.Value(""description"")</div>
        }
    </header>

    <section class=""product-catalog"">
        <h2>Products</h2>
        <!-- Product listing would go here -->
    </section>
</div>";
    }
}