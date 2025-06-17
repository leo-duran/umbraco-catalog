using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace CmsBuilder;

/// <summary>
/// Builder for creating Umbraco document types (ContentType) using a fluent API.
/// </summary>
public class DocumentTypeBuilder
{
    private readonly ContentType _contentType;
    private readonly IContentTypeService _contentTypeService;
    private readonly IShortStringHelper _shortStringHelper;
    private Template? _template;
    private string? _templateContent;
    private readonly IFileService? _fileService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentTypeBuilder"/> class.
    /// </summary>
    /// <param name="shortStringHelper">The short string helper used for creating aliases.</param>
    public DocumentTypeBuilder(IShortStringHelper shortStringHelper, IContentTypeService contentTypeService)
    {
        _shortStringHelper = shortStringHelper;
        _contentTypeService = contentTypeService;
        _contentType = new ContentType(shortStringHelper, -1);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentTypeBuilder"/> class with template support.
    /// </summary>
    /// <param name="shortStringHelper">The short string helper used for creating aliases.</param>
    /// <param name="fileService">The file service for template operations.</param>
    public DocumentTypeBuilder(
        IShortStringHelper shortStringHelper,
        IContentTypeService contentTypeService,
        IFileService fileService)
    {
        _shortStringHelper = shortStringHelper;
        _contentTypeService = contentTypeService;
        _contentType = new ContentType(shortStringHelper, -1);
        _fileService = fileService;
    }

    /// <summary>
    /// Sets the alias of the document type.
    /// </summary>
    /// <param name="alias">The alias to set.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public DocumentTypeBuilder WithAlias(string alias)
    {
        _contentType.Alias = alias;
        return this;
    }

    /// <summary>
    /// Sets the name of the document type.
    /// </summary>
    /// <param name="name">The name to set.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public DocumentTypeBuilder WithName(string name)
    {
        _contentType.Name = name;
        return this;
    }

    /// <summary>
    /// Sets the description of the document type.
    /// </summary>
    /// <param name="description">The description to set.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public DocumentTypeBuilder WithDescription(string description)
    {
        _contentType.Description = description;
        return this;
    }

    /// <summary>
    /// Sets the icon for the document type.
    /// </summary>
    /// <param name="icon">The icon to set.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public DocumentTypeBuilder WithIcon(string icon)
    {
        _contentType.Icon = icon;
        return this;
    }

    /// <summary>
    /// Sets whether the document type is allowed at the root level.
    /// </summary>
    /// <param name="allowAtRoot">Whether to allow at root.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public DocumentTypeBuilder AllowAtRoot(bool allowAtRoot = true)
    {
        _contentType.AllowedAsRoot = allowAtRoot;
        return this;
    }

    /// <summary>
    /// Sets whether the document type is an element type.
    /// </summary>
    /// <param name="isElement">Whether the document type is an element type.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public DocumentTypeBuilder IsElement(bool isElement = true)
    {
        _contentType.IsElement = isElement;
        return this;
    }

    /// <summary>
    /// Sets the parent folder/container for the document type.
    /// </summary>
    /// <param name="parentId">The ID of the parent folder/container.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public DocumentTypeBuilder WithParentFolder(int parentId)
    {
        _contentType.ParentId = parentId;
        return this;
    }

    /// <summary>
    /// Adds a tab (property group) to the document type.
    /// </summary>
    /// <param name="name">The name of the tab.</param>
    /// <param name="configureTab">Action to configure the tab.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public DocumentTypeBuilder AddTab(string name, Action<TabBuilder> configureTab)
    {
        var tabBuilder = new TabBuilder(name, _shortStringHelper);
        configureTab(tabBuilder);
        _contentType.PropertyGroups.Add(tabBuilder.Build());
        return this;
    }

    /// <summary>
    /// Adds a composition (parent document type) to inherit from.
    /// </summary>
    /// <param name="compositionContentType">The composition content type to add.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public DocumentTypeBuilder AddComposition(IContentTypeComposition compositionContentType)
    {
        _contentType.AddContentType(compositionContentType);
        return this;
    }

    /// <summary>
    /// Creates and associates a template with the document type (synchronous version).
    /// </summary>
    /// <param name="templateAlias">The alias of the template.</param>
    /// <param name="templateName">The name of the template.</param>
    /// <param name="templateContent">The content of the template.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when template services are not available.</exception>
    public DocumentTypeBuilder WithTemplate(string templateAlias, string templateName, string templateContent)
    {
        if (_fileService == null)
        {
            throw new InvalidOperationException("File service is not available. Use the constructor that accepts IFileService.");
        }

        try
        {
            // Try to get existing template first
            _template = _fileService.GetTemplate(templateAlias) as Template;
            if (_template == null)
            {
                // Create a new template
                _template = new Template(_shortStringHelper, templateAlias, templateName)
                {
                    Content = templateContent
                };

                // Save the template
                _fileService.SaveTemplate(_template);
            }
            else
            {
                // Update the existing template content if it's different
                if (_template.Content != templateContent)
                {
                    _template.Content = templateContent;
                    _fileService.SaveTemplate(_template);
                }
            }

            // Store the template content for later use
            _templateContent = templateContent;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create or update template: {ex.Message}", ex);
        }

        return this;
    }

    /// <summary>
    /// Associates an existing template with the document type by alias.
    /// </summary>
    /// <param name="templateAlias">The alias of the existing template.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when file service is not available or template is not found.</exception>
    public DocumentTypeBuilder WithExistingTemplate(string templateAlias)
    {
        if (_fileService == null)
        {
            throw new InvalidOperationException("File service is not available. Use the constructor that accepts IFileService.");
        }

        // Get the existing template
        _template = _fileService.GetTemplate(templateAlias) as Template;
        if (_template == null)
        {
            throw new InvalidOperationException($"Template with alias '{templateAlias}' not found.");
        }

        return this;
    }

    /// <summary>
    /// Builds, saves, and returns the ContentType instance.
    /// After calling this method, the document type will be persisted to the database.
    /// </summary>
    /// <returns>The built and saved ContentType.</returns>
    public ContentType Build()
    {
        // Check if content type already exists
        var existingContentType = _contentTypeService.Get(_contentType.Alias);
        if (existingContentType != null)
        {
            return existingContentType as ContentType ?? throw new InvalidOperationException($"Existing content type '{_contentType.Alias}' could not be cast to ContentType");
        }

        // Set IsElement to false by default to ensure the document type appears in the content tree
        if (!_contentType.IsElement)
        {
            _contentType.IsElement = false;
        }

        // Associate the template if one was created
        if (_template != null)
        {
            _contentType.AllowedTemplates = [_template];
            _contentType.SetDefaultTemplate(_template);
        }

        // Save the content type to the database
        _contentTypeService.Save(_contentType);

        return _contentType;
    }

    /// <summary>
    /// Builds the ContentType instance without saving it to the database.
    /// Use this method when you want to inspect or modify the content type before saving.
    /// </summary>
    /// <returns>The built ContentType (not saved to database).</returns>
    public ContentType BuildWithoutSaving()
    {
        // Set IsElement to false by default to ensure the document type appears in the content tree
        if (!_contentType.IsElement)
        {
            _contentType.IsElement = false;
        }

        // Associate the template if one was created
        if (_template != null)
        {
            _contentType.AllowedTemplates = [_template];
            _contentType.SetDefaultTemplate(_template);
        }

        return _contentType;
    }

    /// <summary>
    /// Gets the template associated with this document type, if any.
    /// </summary>
    /// <returns>The template, or null if no template is associated.</returns>
    public Template? GetTemplate()
    {
        return _template;
    }

    /// <summary>
    /// Gets the template content associated with this document type, if any.
    /// </summary>
    /// <returns>The template content, or null if no template is associated.</returns>
    public string? GetTemplateContent()
    {
        return _templateContent;
    }
}