using System;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.DocTypeBuilder;

/// <summary>
/// Builder for creating Umbraco document types (ContentType) using a fluent API.
/// </summary>
public class DocumentTypeBuilder
{
    private readonly ContentType _contentType;
    private readonly IShortStringHelper _shortStringHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentTypeBuilder"/> class.
    /// </summary>
    /// <param name="shortStringHelper">The short string helper used for creating aliases.</param>
    public DocumentTypeBuilder(IShortStringHelper shortStringHelper)
    {
        _shortStringHelper = shortStringHelper;
        _contentType = new ContentType(shortStringHelper, -1);
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
    /// Builds and returns the ContentType instance.
    /// </summary>
    /// <returns>The built ContentType.</returns>
    public ContentType Build()
    {
        return _contentType;
    }
}