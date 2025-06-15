using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Persistence.Querying;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Catalog.Plugin.Composers;

/// <summary>
/// Handler that creates the Catalog Page content when the Umbraco application starts.
/// </summary>
public class CatalogContentHandler : INotificationAsyncHandler<UmbracoApplicationStartingNotification>
{
    private const string CatalogPageAlias = "catalogPage";
    private const string CatalogPageName = "Catalog";
    private const string AllCatalogsFolder = "All Catalogs";
    private const string ArticlesFolder = "Articles";
    private const string PagesFolder = "Pages";
    private const string NewsFolder = "News";

    private readonly IContentService _contentService;
    private readonly ICoreScopeProvider _scopeProvider;
    private readonly ILogger<CatalogContentHandler> _logger;
    private readonly IContentTypeService _contentTypeService;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IShortStringHelper _shortStringHelper;

    public CatalogContentHandler(
        IContentService contentService,
        ICoreScopeProvider scopeProvider,
        ILogger<CatalogContentHandler> logger,
        IContentTypeService contentTypeService,
        ILoggerFactory loggerFactory,
        IShortStringHelper shortStringHelper)
    {
        _contentService = contentService;
        _scopeProvider = scopeProvider;
        _logger = logger;
        _contentTypeService = contentTypeService;
        _loggerFactory = loggerFactory;
        _shortStringHelper = shortStringHelper;
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

                    // Create folder structure
                    var mainFolder = ContentBuilder.GetOrCreateFolder(
                        _contentService,
                        _contentTypeService,
                        _shortStringHelper,
                        AllCatalogsFolder,
                        -1, // Root level
                        _logger);

                    // Create subfolders
                    var articlesFolder = ContentBuilder.GetOrCreateFolder(
                        _contentService,
                        _contentTypeService,
                        _shortStringHelper,
                        ArticlesFolder,
                        mainFolder.Id,
                        _logger);

                    var pagesFolder = ContentBuilder.GetOrCreateFolder(
                        _contentService,
                        _contentTypeService,
                        _shortStringHelper,
                        PagesFolder,
                        mainFolder.Id,
                        _logger);

                    var newsFolder = ContentBuilder.GetOrCreateFolder(
                        _contentService,
                        _contentTypeService,
                        _shortStringHelper,
                        NewsFolder,
                        mainFolder.Id,
                        _logger);

                    // Check if catalog page content already exists in the Pages folder
                    var pagesChildren = _contentService.GetPagedChildren(pagesFolder.Id, 0, 1000, out _);
                    var existingContent = pagesChildren.FirstOrDefault(c => c.ContentType.Alias == CatalogPageAlias);

                    if (existingContent != null)
                    {
                        _logger.LogInformation("Catalog Page content already exists in Pages folder. Skipping content creation.");
                        scope.Complete();
                        return;
                    }

                    // Create the catalog page content in the Pages folder
                    _logger.LogInformation("Creating Catalog Page content in Pages folder");

                    var contentBuilder = new ContentBuilder(
                        _contentService,
                        CatalogPageAlias,
                        CatalogPageName,
                        pagesFolder.Id, // Create in Pages folder
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