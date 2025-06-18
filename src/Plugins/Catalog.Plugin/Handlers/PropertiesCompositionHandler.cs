using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using CmsBuilder;

namespace Catalog.Plugin.Handlers;

/// <summary>
/// Handler that creates a ContentProperties composition document type.
/// This is a composition that can be used by other document types to include content properties.
/// </summary>
public class PropertiesCompositionHandler : INotificationAsyncHandler<UmbracoApplicationStartingNotification>
{
    const string contentTypeAlias = "contentProperties";
    const string compositionsFolderName = "Compositions";
    private readonly IShortStringHelper _shortStringHelper;
    private readonly IContentTypeService _contentTypeService;
    private readonly ICoreScopeProvider _scopeProvider;
    private readonly ILogger<PropertiesCompositionHandler> _logger;
    private readonly IFileService _fileService;
    public PropertiesCompositionHandler(
        IShortStringHelper shortStringHelper,
        IContentTypeService contentTypeService,
        ICoreScopeProvider scopeProvider,
        ILogger<PropertiesCompositionHandler> logger,
        IFileService fileService)
    {
        _shortStringHelper = shortStringHelper;
        _contentTypeService = contentTypeService;
        _scopeProvider = scopeProvider;
        _logger = logger;
        _fileService = fileService;
    }

    public async Task HandleAsync(UmbracoApplicationStartingNotification notification, CancellationToken cancellationToken)
    {

        _logger.LogInformation("Starting PropertiesCompositionHandler.HandleAsync");

        using (var scope = _scopeProvider.CreateCoreScope())
        {
            try
            {
                var folderId = GetOrCreateCompositionsFolder();

                CreateContentPropertiesComposition(folderId);
                CreateFooterPropertiesComposition(folderId);

                _logger.LogInformation("Completing scope");
                scope.Complete();
                _logger.LogInformation("Scope completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in PropertiesCompositionHandler scope: {Message}", ex.Message);
                throw;
            }
        }
    }

    private void CreateContentPropertiesComposition(int folderId)
    {
        _logger.LogInformation("Creating ContentProperties composition");

        // Create and build the document type - Build() now handles duplicate checking and persistence
        var contentType = new DocumentTypeBuilder(_shortStringHelper, _contentTypeService, _fileService)
            .WithAlias(contentTypeAlias)
            .WithName("Content Properties")
            .WithDescription("A composition that contains content properties")
            .WithIcon("icon-plugin")
            .IsElement(true)  // Set as composition using the builder method
            .WithParentFolder(folderId > 0 ? folderId : -1)  // Use the new method
            .AddTab("Content", tab => tab
                .WithAlias("content")
                .WithSortOrder(1)
                .AddTextAreaProperty("Main Content", "mainContent",
                    description: "Main content"))
            .Build();  // This now handles duplicate checking and saves to the database

        _logger.LogInformation("Composition content type created and saved successfully: {ContentTypeAlias}", contentTypeAlias);
    }

    private void CreateFooterPropertiesComposition(int folderId)
    {
        // Create and build the document type - Build() now handles duplicate checking and persistence
        var contentType = new DocumentTypeBuilder(_shortStringHelper, _contentTypeService, _fileService)
            .WithAlias("footerProperties")
            .WithName("Footer Properties")
            .WithDescription("A composition that contains footer properties")
            .WithIcon("icon-plugin")
            .IsElement(true)  // Set as composition using the builder method
            .WithParentFolder(folderId > 0 ? folderId : -1)  // Use the new method
            .AddTab("Content", tab => tab
                .WithAlias("content")
                .WithSortOrder(1)
                .AddTextAreaProperty("Footer Content", "footerContent",
                    description: "Footer Content"))
            .Build();  // This now handles duplicate checking and saves to the database
    }
    private int GetOrCreateCompositionsFolder()
    {
        try
        {
            _logger.LogInformation("Creating or getting Compositions folder");

            // Check if the folder already exists
            var existingFolders = _contentTypeService.GetContainers(Array.Empty<int>());
            var existingFolder = existingFolders.FirstOrDefault(f => f.Name == compositionsFolderName);

            if (existingFolder != null)
            {
                _logger.LogInformation("Found existing Compositions folder with ID: {FolderId}", existingFolder.Id);
                return existingFolder.Id;
            }

            // Create a new folder
            _logger.LogInformation("Creating new Compositions folder");
            var result = _contentTypeService.CreateContainer(-1, Guid.NewGuid(), compositionsFolderName);

            if (result.Success)
            {
                // Check if the container was created successfully
                var containers = _contentTypeService.GetContainers(Array.Empty<int>());
                var newContainer = containers.FirstOrDefault(f => f.Name == compositionsFolderName);

                if (newContainer != null)
                {
                    _logger.LogInformation("Created Compositions folder with ID: {FolderId}", newContainer.Id);
                    return newContainer.Id;
                }
            }

            _logger.LogError("Failed to create Compositions folder");
            return -1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Compositions folder: {Message}", ex.Message);
            return -1;
        }
    }
}