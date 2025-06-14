using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Persistence.Querying;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;

namespace Catalog.Plugin.Composers;

/// <summary>
/// Handler that creates the Catalog Page content when the Umbraco application starts.
/// </summary>
public class CatalogContentHandler : INotificationAsyncHandler<UmbracoApplicationStartingNotification>
{
    private const string CatalogPageAlias = "catalogPage";
    private const string CatalogPageName = "Catalog";

    private readonly IContentService _contentService;
    private readonly ICoreScopeProvider _scopeProvider;
    private readonly ILogger<CatalogContentHandler> _logger;
    private readonly IContentTypeService _contentTypeService;
    private readonly ILoggerFactory _loggerFactory;

    public CatalogContentHandler(
        IContentService contentService,
        ICoreScopeProvider scopeProvider,
        ILogger<CatalogContentHandler> logger,
        IContentTypeService contentTypeService,
        ILoggerFactory loggerFactory)
    {
        _contentService = contentService;
        _scopeProvider = scopeProvider;
        _logger = logger;
        _contentTypeService = contentTypeService;
        _loggerFactory = loggerFactory;
    }

    public async Task HandleAsync(UmbracoApplicationStartingNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting CatalogContentHandler.HandleAsync");

            using (var scope = _scopeProvider.CreateCoreScope())
            {
                try
                {
                    // Check if the catalog page content type exists
                    var catalogPageContentType = _contentTypeService.Get(CatalogPageAlias);
                    if (catalogPageContentType == null)
                    {
                        _logger.LogWarning("Catalog Page content type not found. Skipping content creation.");
                        scope.Complete();
                        return;
                    }

                    // Get the content type ID
                    int contentTypeId = catalogPageContentType.Id;

                    // Check if catalog page content already exists
                    // Use GetRootContent instead of GetPagedOfType to avoid filter parameter issues
                    var rootContent = _contentService.GetRootContent();
                    var existingContent = rootContent.FirstOrDefault(c => c.ContentType.Id == contentTypeId);

                    if (existingContent != null)
                    {
                        _logger.LogInformation("Catalog Page content already exists. Skipping content creation.");
                        scope.Complete();
                        return;
                    }

                    // Create the catalog page content
                    _logger.LogInformation("Creating Catalog Page content");

                    var contentBuilder = new ContentBuilder(
                        _contentService,
                        CatalogPageAlias,
                        CatalogPageName,
                        -1, // Create at root level
                        _loggerFactory.CreateLogger<ContentBuilder>());

                    // Set properties
                    contentBuilder.WithProperty("title", "2025 Catalog");

                    // Save and publish the content
                    var content = contentBuilder.Save(true);

                    _logger.LogInformation("Catalog Page content created successfully with ID: {ContentId}", content.Id);

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in CatalogContentHandler scope: {Message}", ex.Message);
                    throw;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CatalogContentHandler.HandleAsync: {Message}", ex.Message);
            throw;
        }
    }
}