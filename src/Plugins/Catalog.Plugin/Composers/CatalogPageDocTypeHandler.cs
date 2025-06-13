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
                    const string templateAlias = "catalogPageTemplate";
                    const string templateName = "Catalog Page Template";

                    // Get the template content
                    string templateContent = GetDefaultTemplateContent();

                    _logger.LogInformation("Creating document type with template: {TemplateAlias}", templateAlias);
                    var contentTypeBuilder = new DocumentTypeBuilder(_shortStringHelper, _fileService)
                        .WithAlias(contentTypeAlias)
                        .WithName("Catalog Page")
                        .WithDescription("A page that displays a catalog of products")
                        .WithIcon("icon-shopping-basket-alt")
                        .AllowAtRoot(true)
                        .WithTemplate(templateAlias, templateName, templateContent);

                    // Build the content type
                    _logger.LogInformation("Building content type");
                    var contentType = contentTypeBuilder.Build();

                    // Enable ModelsBuilder for this content type
                    _logger.LogInformation("Enabling ModelsBuilder for content type");
                    contentType.IsElement = false;
                    contentType.AllowedAsRoot = true;

                    // Save the document type
                    _logger.LogInformation("Saving content type: {ContentTypeAlias}", contentTypeAlias);
                    _contentTypeService.Save(contentType);
                    _logger.LogInformation("Content type saved successfully: {ContentTypeAlias}", contentTypeAlias);

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