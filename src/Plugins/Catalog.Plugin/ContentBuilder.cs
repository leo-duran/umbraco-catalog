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
}