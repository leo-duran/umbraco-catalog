using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Catalog.Plugin;

/// <summary>
/// Builder for creating Umbraco content using a fluent API.
/// </summary>
public class ContentBuilder
{
    private readonly IContent _content;
    private readonly IContentService _contentService;
    private readonly ILogger<ContentBuilder> _logger;
    private readonly IContentTypeService? _contentTypeService;
    private readonly IShortStringHelper? _shortStringHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentBuilder"/> class.
    /// </summary>
    /// <param name="contentService">The content service for content operations.</param>
    /// <param name="contentTypeAlias">The alias of the content type to create.</param>
    /// <param name="name">The name of the content item.</param>
    /// <param name="parentId">The ID of the parent content item, or -1 for root.</param>
    /// <param name="logger">The logger.</param>
    public ContentBuilder(
        IContentService contentService,
        string contentTypeAlias,
        string name,
        int parentId,
        ILogger<ContentBuilder> logger)
    {
        _contentService = contentService;
        _logger = logger;

        // Create the content item
        _content = _contentService.Create(name, parentId, contentTypeAlias);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentBuilder"/> class with additional services for folder creation.
    /// </summary>
    /// <param name="contentService">The content service for content operations.</param>
    /// <param name="contentTypeService">The content type service for content type operations.</param>
    /// <param name="shortStringHelper">The short string helper for alias generation.</param>
    /// <param name="contentTypeAlias">The alias of the content type to create.</param>
    /// <param name="name">The name of the content item.</param>
    /// <param name="parentId">The ID of the parent content item, or -1 for root.</param>
    /// <param name="logger">The logger.</param>
    public ContentBuilder(
        IContentService contentService,
        IContentTypeService contentTypeService,
        IShortStringHelper shortStringHelper,
        string contentTypeAlias,
        string name,
        int parentId,
        ILogger<ContentBuilder> logger)
    {
        _contentService = contentService;
        _contentTypeService = contentTypeService;
        _shortStringHelper = shortStringHelper;
        _logger = logger;

        // Create the content item
        _content = _contentService.Create(name, parentId, contentTypeAlias);
    }

    /// <summary>
    /// Sets a property value on the content item.
    /// </summary>
    /// <param name="propertyAlias">The alias of the property.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public ContentBuilder WithProperty(string propertyAlias, object? value)
    {
        _content.SetValue(propertyAlias, value);
        return this;
    }

    /// <summary>
    /// Sets multiple property values on the content item.
    /// </summary>
    /// <param name="properties">Dictionary of property aliases and values.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public ContentBuilder WithProperties(IDictionary<string, object?> properties)
    {
        foreach (var property in properties)
        {
            _content.SetValue(property.Key, property.Value);
        }
        return this;
    }

    /// <summary>
    /// Sets the sort order of the content item.
    /// </summary>
    /// <param name="sortOrder">The sort order to set.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public ContentBuilder WithSortOrder(int sortOrder)
    {
        _content.SortOrder = sortOrder;
        return this;
    }

    /// <summary>
    /// Sets the culture for the content item (for multi-lingual sites).
    /// </summary>
    /// <param name="culture">The culture code.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public ContentBuilder WithCulture(string culture)
    {
        _content.SetCultureName(culture, _content.Name);
        return this;
    }

    /// <summary>
    /// Saves the content item.
    /// </summary>
    /// <param name="publish">Whether to publish the content item.</param>
    /// <returns>The saved content item.</returns>
    public IContent Save(bool publish = true)
    {
        try
        {
            _logger.LogInformation("Saving content: {ContentName} ({ContentType})", _content.Name, _content.ContentType.Alias);

            if (publish)
            {
                // First save the content
                _contentService.Save(_content);

                // Then publish it - pass an empty string array for cultures to publish in all cultures
                var result = _contentService.Publish(_content, Array.Empty<string>());
                if (result.Success)
                {
                    _logger.LogInformation("Content saved and published successfully: {ContentId}", _content.Id);
                }
                else
                {
                    _logger.LogWarning("Content saved but not published: {ContentId}. Errors: {Errors}",
                        _content.Id, string.Join(", ", result.InvalidProperties?.Select(p => p.Alias) ?? Array.Empty<string>()));
                }
            }
            else
            {
                _contentService.Save(_content);
                _logger.LogInformation("Content saved successfully: {ContentId}", _content.Id);
            }

            return _content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving content: {Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Gets the content item being built.
    /// </summary>
    /// <returns>The content item.</returns>
    public IContent GetContent()
    {
        return _content;
    }

    /// <summary>
    /// Creates a folder document type if it doesn't exist.
    /// </summary>
    /// <param name="contentTypeService">The content type service.</param>
    /// <param name="shortStringHelper">The short string helper.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>The folder content type.</returns>
    public static IContentType EnsureFolderDocumentType(
        IContentTypeService contentTypeService,
        IShortStringHelper shortStringHelper,
        ILogger logger)
    {
        const string folderAlias = "folder";

        try
        {
            // Check if folder document type exists
            var folderContentType = contentTypeService.Get(folderAlias);
            if (folderContentType != null)
            {
                return folderContentType;
            }

            logger.LogInformation("Creating folder document type");

            // Create the content type with no parent
            folderContentType = new ContentType(shortStringHelper, -1)
            {
                Alias = folderAlias,
                Name = "Folder",
                Description = "A folder to organize content",
                Icon = "icon-folder",
                AllowedAsRoot = true
            };

            // Save the content type
            contentTypeService.Save(folderContentType);
            logger.LogInformation("Folder document type created successfully");

            return folderContentType;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating folder document type: {Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Gets or creates a folder with the specified name.
    /// </summary>
    /// <param name="contentService">The content service.</param>
    /// <param name="contentTypeService">The content type service.</param>
    /// <param name="shortStringHelper">The short string helper.</param>
    /// <param name="folderName">The name of the folder to create.</param>
    /// <param name="parentId">The ID of the parent content item, or -1 for root.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>The folder content item.</returns>
    public static IContent GetOrCreateFolder(
        IContentService contentService,
        IContentTypeService contentTypeService,
        IShortStringHelper shortStringHelper,
        string folderName,
        int parentId,
        ILogger logger)
    {
        try
        {
            // Ensure the folder document type exists
            var folderContentType = EnsureFolderDocumentType(contentTypeService, shortStringHelper, logger);

            // Check if the folder already exists
            IContent? folder = null;
            if (parentId == -1)
            {
                // Check at root level
                var rootContent = contentService.GetRootContent();
                folder = rootContent.FirstOrDefault(c => c.ContentType.Alias == "folder" && c.Name == folderName);
            }
            else
            {
                // Check under the specified parent
                var children = contentService.GetPagedChildren(parentId, 0, 1000, out _);
                folder = children.FirstOrDefault(c => c.ContentType.Alias == "folder" && c.Name == folderName);
            }

            // Return the existing folder if found
            if (folder != null)
            {
                logger.LogInformation("Found existing folder: {FolderName} with ID: {FolderId}", folderName, folder.Id);
                return folder;
            }

            // Create a new folder
            logger.LogInformation("Creating folder: {FolderName}", folderName);
            var content = contentService.Create(folderName, parentId, "folder");

            // Save and publish the folder
            contentService.Save(content);
            var result = contentService.Publish(content, Array.Empty<string>());

            if (result.Success)
            {
                logger.LogInformation("Folder created successfully: {FolderName} with ID: {FolderId}", folderName, content.Id);
            }
            else
            {
                logger.LogWarning("Folder created but not published: {FolderName}", folderName);
            }

            return content;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting or creating folder {FolderName}: {Message}", folderName, ex.Message);
            throw;
        }
    }
}