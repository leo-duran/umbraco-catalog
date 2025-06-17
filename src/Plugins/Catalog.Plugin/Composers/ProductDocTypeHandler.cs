using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using CmsBuilder;

namespace Catalog.Plugin.Composers;

public class ProductDocTypeHandler : INotificationAsyncHandler<UmbracoApplicationStartingNotification>
{
    const string contentTypeAlias = "product";
    const string contentPropertiesAlias = "contentProperties";
    const string footerPropertiesAlias = "footerProperties";
    private readonly IShortStringHelper _shortStringHelper;
    private readonly IContentTypeService _contentTypeService;
    private readonly ICoreScopeProvider _scopeProvider;
    private readonly ILogger<ProductDocTypeHandler> _logger;

    public ProductDocTypeHandler(
        IShortStringHelper shortStringHelper,
        IContentTypeService contentTypeService,
        ICoreScopeProvider scopeProvider,
        ILogger<ProductDocTypeHandler> logger)
    {
        _shortStringHelper = shortStringHelper;
        _contentTypeService = contentTypeService;
        _scopeProvider = scopeProvider;
        _logger = logger;
    }

    public async Task HandleAsync(UmbracoApplicationStartingNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting ProductDocTypeHandler.HandleAsync");

            using (var scope = _scopeProvider.CreateCoreScope())
            {
                try
                {
                    // Check if content type already exists and handle composition updates
                    var existingContentType = _contentTypeService.Get(contentTypeAlias);
                    if (existingContentType != null)
                    {
                        _logger.LogInformation("Content type already exists: {ContentTypeAlias}, ID: {ContentTypeId}", contentTypeAlias, existingContentType.Id);

                        // Check if it already has the composition
                        var hasComposition = existingContentType.ContentTypeComposition.Any(c => c.Alias == contentPropertiesAlias);
                        if (!hasComposition)
                        {
                            _logger.LogInformation("Adding ContentProperties composition to existing Product document type");
                            AddCompositionToExistingContentType(existingContentType);
                        }
                        else
                        {
                            _logger.LogInformation("Content type already has ContentProperties composition");
                        }

                        scope.Complete();
                        return;
                    }

                    // Get the ContentProperties composition
                    _logger.LogInformation("Getting ContentProperties composition: {CompositionAlias}", contentPropertiesAlias);
                    var contentPropertiesComposition = _contentTypeService.Get(contentPropertiesAlias);
                    if (contentPropertiesComposition == null)
                    {
                        _logger.LogError("ContentProperties composition not found. Make sure PropertiesCompositionHandler runs first.");
                        throw new InvalidOperationException($"ContentProperties composition '{contentPropertiesAlias}' not found. Ensure PropertiesCompositionHandler runs first.");
                    }

                    var footerPropertiesComposition = _contentTypeService.Get(footerPropertiesAlias);
                    if (footerPropertiesComposition == null)
                    {
                        _logger.LogError("FooterProperties composition not found. Make sure PropertiesCompositionHandler runs first.");
                        throw new InvalidOperationException($"FooterProperties composition '{footerPropertiesAlias}' not found. Ensure PropertiesCompositionHandler runs first.");
                    }


                    // Create and build the document type - Build() now handles duplicate checking and persistence
                    _logger.LogInformation("Creating document type: {ContentTypeAlias}", contentTypeAlias);
                    var contentType = new DocumentTypeBuilder(_shortStringHelper, _contentTypeService)
                        .WithAlias(contentTypeAlias)
                        .WithName("Product")
                        .WithDescription("A product document type")
                        .WithIcon("icon-shopping-basket")
                        .AddComposition(contentPropertiesComposition)  // Add the composition first
                        .AddComposition(footerPropertiesComposition)  // Add the composition first
                        .AddTab("Product Info", tab => tab
                            .WithAlias("productInfo")
                            .WithSortOrder(2)  // Set higher sort order so it appears after the Content tab from composition
                            .AddTextAreaProperty("Description", "description")
                            .AddNumericProperty("Price", "price",
                                mandatory: true)
                            .AddMediaPickerProperty("Product Image", "productImage"))
                        .Build();  // This now handles duplicate checking and saves to the database

                    _logger.LogInformation("Product document type created and saved successfully: {ContentTypeAlias}", contentTypeAlias);

                    _logger.LogInformation("Completing scope");
                    scope.Complete();
                    _logger.LogInformation("Scope completed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in ProductDocTypeHandler scope");
                    throw;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ProductDocTypeHandler.HandleAsync");
            throw;
        }

        return;
    }


    private void AddCompositionToExistingContentType(IContentType existingContentType)
    {
        try
        {
            // Get the ContentProperties composition
            var contentPropertiesComposition = _contentTypeService.Get(contentPropertiesAlias);
            if (contentPropertiesComposition == null)
            {
                _logger.LogError("ContentProperties composition not found when trying to add to existing content type");
                return;
            }

            // Add the composition
            existingContentType.AddContentType(contentPropertiesComposition);

            // Save the updated content type
            _contentTypeService.Save(existingContentType);
            _logger.LogInformation("Successfully added ContentProperties composition to existing Product document type");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding composition to existing content type: {Message}", ex.Message);
            throw;
        }
    }
}
