using System;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.DocTypeBuilder;

/// <summary>
/// Builder for creating Umbraco property groups (tabs) using a fluent API.
/// </summary>
public class TabBuilder
{
    private readonly IShortStringHelper _shortStringHelper;
    private string _name = string.Empty;
    private int _sortOrder = 0;
    private PropertyGroup _propertyGroup;

    /// <summary>
    /// Initializes a new instance of the <see cref="TabBuilder"/> class.
    /// </summary>
    /// <param name="shortStringHelper">The short string helper used for creating aliases.</param>
    public TabBuilder(IShortStringHelper shortStringHelper)
    {
        _shortStringHelper = shortStringHelper;
        _propertyGroup = new PropertyGroup(false);
    }

    /// <summary>
    /// Sets the name for the tab.
    /// </summary>
    /// <param name="name">The name to set.</param>
    /// <returns>The current TabBuilder instance for method chaining.</returns>
    public TabBuilder SetName(string name)
    {
        _name = name;
        _propertyGroup.Name = name;
        return this;
    }

    /// <summary>
    /// Sets the sort order for the tab.
    /// </summary>
    /// <param name="sortOrder">The sort order to set.</param>
    /// <returns>The current TabBuilder instance for method chaining.</returns>
    public TabBuilder SetSortOrder(int sortOrder)
    {
        _sortOrder = sortOrder;
        _propertyGroup.SortOrder = sortOrder;
        return this;
    }

    /// <summary>
    /// Adds a TextBox property to the tab.
    /// </summary>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="name">The name of the property.</param>
    /// <param name="configureProperty">Optional action to configure the property.</param>
    /// <returns>The current TabBuilder instance for method chaining.</returns>
    public TabBuilder AddTextBoxProperty(string alias, string name, Action<PropertyBuilder>? configureProperty = null)
    {
        var propertyBuilder = new PropertyBuilder(_shortStringHelper)
            .SetAlias(alias)
            .SetName(name)
            .SetPropertyEditorAlias("Umbraco.TextBox");
            
        configureProperty?.Invoke(propertyBuilder);
        
        var property = propertyBuilder.Build();
        _propertyGroup.PropertyTypes.Add(property);
        
        return this;
    }

    /// <summary>
    /// Adds a TextArea property to the tab.
    /// </summary>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="name">The name of the property.</param>
    /// <param name="configureProperty">Optional action to configure the property.</param>
    /// <returns>The current TabBuilder instance for method chaining.</returns>
    public TabBuilder AddTextAreaProperty(string alias, string name, Action<PropertyBuilder>? configureProperty = null)
    {
        var propertyBuilder = new PropertyBuilder(_shortStringHelper)
            .SetAlias(alias)
            .SetName(name)
            .SetPropertyEditorAlias("Umbraco.TextArea");
            
        configureProperty?.Invoke(propertyBuilder);
        
        var property = propertyBuilder.Build();
        _propertyGroup.PropertyTypes.Add(property);
        
        return this;
    }

    /// <summary>
    /// Adds a RichText property to the tab.
    /// </summary>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="name">The name of the property.</param>
    /// <param name="configureProperty">Optional action to configure the property.</param>
    /// <returns>The current TabBuilder instance for method chaining.</returns>
    public TabBuilder AddRichTextProperty(string alias, string name, Action<PropertyBuilder>? configureProperty = null)
    {
        var propertyBuilder = new PropertyBuilder(_shortStringHelper)
            .SetAlias(alias)
            .SetName(name)
            .SetPropertyEditorAlias("Umbraco.RichText");
            
        configureProperty?.Invoke(propertyBuilder);
        
        var property = propertyBuilder.Build();
        _propertyGroup.PropertyTypes.Add(property);
        
        return this;
    }

    /// <summary>
    /// Adds a MediaPicker3 property to the tab.
    /// </summary>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="name">The name of the property.</param>
    /// <param name="configureProperty">Optional action to configure the property.</param>
    /// <returns>The current TabBuilder instance for method chaining.</returns>
    public TabBuilder AddMediaPickerProperty(string alias, string name, Action<PropertyBuilder>? configureProperty = null)
    {
        var propertyBuilder = new PropertyBuilder(_shortStringHelper)
            .SetAlias(alias)
            .SetName(name)
            .SetPropertyEditorAlias("Umbraco.MediaPicker3");
            
        configureProperty?.Invoke(propertyBuilder);
        
        var property = propertyBuilder.Build();
        _propertyGroup.PropertyTypes.Add(property);
        
        return this;
    }

    /// <summary>
    /// Adds a ContentPicker property to the tab.
    /// </summary>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="name">The name of the property.</param>
    /// <param name="configureProperty">Optional action to configure the property.</param>
    /// <returns>The current TabBuilder instance for method chaining.</returns>
    public TabBuilder AddContentPickerProperty(string alias, string name, Action<PropertyBuilder>? configureProperty = null)
    {
        var propertyBuilder = new PropertyBuilder(_shortStringHelper)
            .SetAlias(alias)
            .SetName(name)
            .SetPropertyEditorAlias("Umbraco.ContentPicker");
            
        configureProperty?.Invoke(propertyBuilder);
        
        var property = propertyBuilder.Build();
        _propertyGroup.PropertyTypes.Add(property);
        
        return this;
    }

    /// <summary>
    /// Adds an Integer property to the tab.
    /// </summary>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="name">The name of the property.</param>
    /// <param name="configureProperty">Optional action to configure the property.</param>
    /// <returns>The current TabBuilder instance for method chaining.</returns>
    public TabBuilder AddIntegerProperty(string alias, string name, Action<PropertyBuilder>? configureProperty = null)
    {
        var propertyBuilder = new PropertyBuilder(_shortStringHelper)
            .SetAlias(alias)
            .SetName(name)
            .SetPropertyEditorAlias("Umbraco.Integer")
            .SetValueStorageType(ValueStorageType.Integer);
            
        configureProperty?.Invoke(propertyBuilder);
        
        var property = propertyBuilder.Build();
        _propertyGroup.PropertyTypes.Add(property);
        
        return this;
    }

    /// <summary>
    /// Adds a TrueFalse (checkbox) property to the tab.
    /// </summary>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="name">The name of the property.</param>
    /// <param name="configureProperty">Optional action to configure the property.</param>
    /// <returns>The current TabBuilder instance for method chaining.</returns>
    public TabBuilder AddCheckboxProperty(string alias, string name, Action<PropertyBuilder>? configureProperty = null)
    {
        var propertyBuilder = new PropertyBuilder(_shortStringHelper)
            .SetAlias(alias)
            .SetName(name)
            .SetPropertyEditorAlias("Umbraco.TrueFalse")
            .SetValueStorageType(ValueStorageType.Integer);
            
        configureProperty?.Invoke(propertyBuilder);
        
        var property = propertyBuilder.Build();
        _propertyGroup.PropertyTypes.Add(property);
        
        return this;
    }

    /// <summary>
    /// Adds a DateTime property to the tab.
    /// </summary>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="name">The name of the property.</param>
    /// <param name="configureProperty">Optional action to configure the property.</param>
    /// <returns>The current TabBuilder instance for method chaining.</returns>
    public TabBuilder AddDatePickerProperty(string alias, string name, Action<PropertyBuilder>? configureProperty = null)
    {
        var propertyBuilder = new PropertyBuilder(_shortStringHelper)
            .SetAlias(alias)
            .SetName(name)
            .SetPropertyEditorAlias("Umbraco.DateTime")
            .SetValueStorageType(ValueStorageType.Date);
            
        configureProperty?.Invoke(propertyBuilder);
        
        var property = propertyBuilder.Build();
        _propertyGroup.PropertyTypes.Add(property);
        
        return this;
    }

    /// <summary>
    /// Builds and returns the configured PropertyGroup.
    /// </summary>
    /// <returns>The configured PropertyGroup instance.</returns>
    public PropertyGroup Build()
    {
        return _propertyGroup;
    }
}