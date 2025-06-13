using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.DocTypeBuilder;

/// <summary>
/// Builder for creating Umbraco document types (ContentType) using a fluent API.
/// </summary>
public class DocumentTypeBuilder
{
    private readonly IShortStringHelper _shortStringHelper;
    private string _alias = "defaultAlias";
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _icon = "icon-document";
    private bool _allowedAsRoot = false;
    private bool _isElement = false;
    private int _parentId = -1;
    private readonly List<PropertyGroup> _tabs = new List<PropertyGroup>();

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentTypeBuilder"/> class.
    /// </summary>
    /// <param name="shortStringHelper">The short string helper used for creating aliases.</param>
    public DocumentTypeBuilder(IShortStringHelper shortStringHelper)
    {
        _shortStringHelper = shortStringHelper;
    }

    /// <summary>
    /// Sets the alias for the document type.
    /// </summary>
    /// <param name="alias">The alias to set.</param>
    /// <returns>The current DocumentTypeBuilder instance for method chaining.</returns>
    public DocumentTypeBuilder SetAlias(string alias)
    {
        _alias = alias;
        return this;
    }

    /// <summary>
    /// Sets the name for the document type.
    /// </summary>
    /// <param name="name">The name to set.</param>
    /// <returns>The current DocumentTypeBuilder instance for method chaining.</returns>
    public DocumentTypeBuilder SetName(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the description for the document type.
    /// </summary>
    /// <param name="description">The description to set.</param>
    /// <returns>The current DocumentTypeBuilder instance for method chaining.</returns>
    public DocumentTypeBuilder SetDescription(string description)
    {
        _description = description;
        return this;
    }

    /// <summary>
    /// Sets the icon for the document type.
    /// </summary>
    /// <param name="icon">The icon to set.</param>
    /// <returns>The current DocumentTypeBuilder instance for method chaining.</returns>
    public DocumentTypeBuilder SetIcon(string icon)
    {
        _icon = icon;
        return this;
    }

    /// <summary>
    /// Sets whether this document type is allowed at root.
    /// </summary>
    /// <param name="allowedAtRoot">Whether to allow at root.</param>
    /// <returns>The current DocumentTypeBuilder instance for method chaining.</returns>
    public DocumentTypeBuilder SetAllowedAtRoot(bool allowedAtRoot = true)
    {
        _allowedAsRoot = allowedAtRoot;
        return this;
    }

    /// <summary>
    /// Sets whether this document type is an element type.
    /// </summary>
    /// <param name="isElement">Whether this is an element type.</param>
    /// <returns>The current DocumentTypeBuilder instance for method chaining.</returns>
    public DocumentTypeBuilder SetIsElement(bool isElement = true)
    {
        _isElement = isElement;
        return this;
    }

    /// <summary>
    /// Adds a tab to the document type.
    /// </summary>
    /// <param name="tabName">The name of the tab.</param>
    /// <param name="configureTab">Optional action to configure the tab.</param>
    /// <returns>The current DocumentTypeBuilder instance for method chaining.</returns>
    public DocumentTypeBuilder AddTab(string tabName, Action<TabBuilder>? configureTab = null)
    {
        var tabBuilder = new TabBuilder(_shortStringHelper);
        tabBuilder.SetName(tabName);
        
        configureTab?.Invoke(tabBuilder);
        
        var tab = tabBuilder.Build();
        _tabs.Add(tab);
        
        return this;
    }

    /// <summary>
    /// Adds a composition to the document type.
    /// </summary>
    /// <param name="compositionAlias">The alias of the composition to add.</param>
    /// <returns>The current DocumentTypeBuilder instance for method chaining.</returns>
    public DocumentTypeBuilder AddComposition(string compositionAlias)
    {
        // Note: In a real implementation, you would need to resolve the composition
        // from a service. This is a simplified version for demonstration.
        return this;
    }

    /// <summary>
    /// Builds and returns the configured ContentType.
    /// </summary>
    /// <returns>The configured ContentType instance.</returns>
    public ContentType Build()
    {
        // Use the correct Umbraco 15.4.3 constructor pattern - just use parentId for now
        var contentType = new ContentType(_shortStringHelper, _parentId);
        
        // Set the alias after creation (this should work in Umbraco 15)
        contentType.Alias = _alias;
        
        // Set all the configured properties
        contentType.Name = _name;
        contentType.Description = _description;
        contentType.Icon = _icon;
        contentType.AllowedAsRoot = _allowedAsRoot;
        contentType.IsElement = _isElement;
        
        // Add all the tabs
        foreach (var tab in _tabs)
        {
            contentType.PropertyGroups.Add(tab);
        }
        
        return contentType;
    }
}