using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using CmsBuilder;

namespace Catalog.Plugin.Handlers;

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

            var templateContent = GetTemplateContent();

            var homePageType = new DocumentTypeBuilder(_shortStringHelper, _contentTypeService, _fileService)
                .AddFolder("Pages")
                .WithAlias(contentTypeAlias)
                .WithName("Home Page")
                .WithDescription("The home page of the website")
                .WithIcon("icon-home")
                .AllowAtRoot(true)
                .WithTemplate("homePage", "Home Page", templateContent)
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

    /// <summary>
    /// Reads the template content from the Views/homePage.cshtml file.
    /// </summary>
    /// <returns>The template content as a string.</returns>
    private string GetTemplateContent()
    {
        // Get the path to the template file relative to the handler's location
        var templatePath = Path.Combine(Path.GetDirectoryName(typeof(PageDocTypeHandler).Assembly.Location) ?? "",
                "Handlers", "Views", "homePage.txt");


        if (System.IO.File.Exists(templatePath))
        {
            _logger.LogInformation("Reading template content from: {TemplatePath}", templatePath);
            return System.IO.File.ReadAllText(templatePath);
        }
        else
        {
            _logger.LogError("Template file not found at: {TemplatePath}", templatePath);
            throw new FileNotFoundException($"Template file not found at: {templatePath}");
        }
    }
}