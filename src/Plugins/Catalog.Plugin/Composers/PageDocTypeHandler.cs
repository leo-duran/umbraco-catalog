using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using CmsBuilder;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Catalog.Plugin.Composers;

public class PageDocTypeHandler : INotificationAsyncHandler<UmbracoApplicationStartingNotification>
{
    private readonly IShortStringHelper _shortStringHelper;
    private readonly IContentTypeService _contentTypeService;
    private readonly ICoreScopeProvider _scopeProvider;
    private readonly ILogger<PageDocTypeHandler> _logger;

    private readonly IContentService _contentService;
    private readonly IFileService _fileService;

    const string contentTypeAlias = "homePage";
    const string contentPropertiesAlias = "contentProperties";

    public PageDocTypeHandler(
        IShortStringHelper shortStringHelper,
        IContentTypeService contentTypeService,
        ICoreScopeProvider scopeProvider,
        ILogger<PageDocTypeHandler> logger,
        IContentService contentService,
        IFileService fileService)
    {
        _shortStringHelper = shortStringHelper;
        _contentTypeService = contentTypeService;
        _scopeProvider = scopeProvider;
        _logger = logger;
        _contentService = contentService;
        _fileService = fileService;
    }
    public Task HandleAsync(UmbracoApplicationStartingNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting PageDocTypeHandler.HandleAsync");

            // Get the ContentProperties composition
            _logger.LogInformation("Getting ContentProperties composition: {CompositionAlias}", contentPropertiesAlias);
            var contentPropertiesComposition = _contentTypeService.Get(contentPropertiesAlias);
            if (contentPropertiesComposition == null)
            {
                _logger.LogError("ContentProperties composition not found. Make sure PropertiesCompositionHandler runs first.");
                throw new InvalidOperationException($"ContentProperties composition '{contentPropertiesAlias}' not found. Ensure PropertiesCompositionHandler runs first.");
            }

            var homePageType = new DocumentTypeBuilder(_shortStringHelper, _contentTypeService, _fileService)
                .AddFolder("Pages")
                .WithAlias(contentTypeAlias)
                .WithName("Home Page")
                .WithDescription("The home page of the website")
                .WithIcon("icon-home")
                .AllowAtRoot(true)
                .WithTemplate("homePage", "Home Page", "<div>Home Page</div>")
                .AddComposition(contentPropertiesComposition)
                .Build();

            _logger.LogInformation("Home page type created: {HomePageType}", homePageType.Alias);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in PageDocTypeHandler.HandleAsync: {Message}", ex.Message);
        }

        return Task.CompletedTask;
    }
}