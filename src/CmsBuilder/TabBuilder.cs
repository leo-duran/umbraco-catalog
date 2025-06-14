using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;

namespace CmsBuilder;

/// <summary>
/// Builder for creating property groups (tabs) in Umbraco document types.
/// </summary>
public class TabBuilder
{
    private readonly PropertyGroup _tab;
    private readonly IShortStringHelper _shortStringHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="TabBuilder"/> class.
    /// </summary>
    /// <param name="name">The name of the tab.</param>
    /// <param name="shortStringHelper">The short string helper used for creating aliases.</param>
    public TabBuilder(string name, IShortStringHelper shortStringHelper)
    {
        _shortStringHelper = shortStringHelper;
        _tab = new PropertyGroup(new PropertyTypeCollection(true))
        {
            Name = name,
            Type = PropertyGroupType.Tab
        };
    }

    /// <summary>
    /// Sets the alias of the tab.
    /// </summary>
    /// <param name="alias">The alias to set.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder WithAlias(string alias)
    {
        _tab.Alias = alias;
        return this;
    }

    /// <summary>
    /// Sets the sort order of the tab.
    /// </summary>
    /// <param name="sortOrder">The sort order to set.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder WithSortOrder(int sortOrder)
    {
        _tab.SortOrder = sortOrder;
        return this;
    }

    /// <summary>
    /// Adds a property to the tab.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="editorAlias">The alias of the property editor.</param>
    /// <param name="configureProperty">Action to configure the property.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder AddProperty(string name, string alias, string editorAlias, Action<PropertyBuilder> configureProperty)
    {
        var propertyBuilder = new PropertyBuilder(name, alias, editorAlias, _shortStringHelper);
        configureProperty(propertyBuilder);
        _tab.PropertyTypes?.Add(propertyBuilder.Build());
        return this;
    }

    /// <summary>
    /// Adds a text box property to the tab.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="configureProperty">Action to configure the property.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder AddTextBoxProperty(string name, string alias, Action<PropertyBuilder>? configureProperty = null)
    {
        return AddProperty(name, alias, "Umbraco.TextBox", configureProperty ?? (p => { }));
    }

    /// <summary>
    /// Adds a text area property to the tab.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="configureProperty">Action to configure the property.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder AddTextAreaProperty(string name, string alias, Action<PropertyBuilder>? configureProperty = null)
    {
        return AddProperty(name, alias, "Umbraco.TextArea", configureProperty ?? (p => { }));
    }

    /// <summary>
    /// Adds a rich text editor property to the tab.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="configureProperty">Action to configure the property.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder AddRichTextProperty(string name, string alias, Action<PropertyBuilder>? configureProperty = null)
    {
        return AddProperty(name, alias, "Umbraco.TinyMCE", configureProperty ?? (p => { }));
    }

    /// <summary>
    /// Adds a media picker property to the tab.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="configureProperty">Action to configure the property.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder AddMediaPickerProperty(string name, string alias, Action<PropertyBuilder>? configureProperty = null)
    {
        return AddProperty(name, alias, "Umbraco.MediaPicker3", configureProperty ?? (p => { }));
    }

    /// <summary>
    /// Adds a content picker property to the tab.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="configureProperty">Action to configure the property.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder AddContentPickerProperty(string name, string alias, Action<PropertyBuilder>? configureProperty = null)
    {
        return AddProperty(name, alias, "Umbraco.ContentPicker", configureProperty ?? (p => { }));
    }

    /// <summary>
    /// Adds a numeric property to the tab.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="configureProperty">Action to configure the property.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder AddNumericProperty(string name, string alias, Action<PropertyBuilder>? configureProperty = null)
    {
        return AddProperty(name, alias, "Umbraco.Integer", configureProperty ?? (p => { }));
    }

    /// <summary>
    /// Adds a checkbox property to the tab.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="configureProperty">Action to configure the property.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder AddCheckboxProperty(string name, string alias, Action<PropertyBuilder>? configureProperty = null)
    {
        return AddProperty(name, alias, "Umbraco.TrueFalse", configureProperty ?? (p => { }));
    }

    /// <summary>
    /// Adds a date picker property to the tab.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="configureProperty">Action to configure the property.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder AddDatePickerProperty(string name, string alias, Action<PropertyBuilder>? configureProperty = null)
    {
        return AddProperty(name, alias, "Umbraco.DateTime", configureProperty ?? (p => { }));
    }

    /// <summary>
    /// Builds and returns the PropertyGroup instance.
    /// </summary>
    /// <returns>The built PropertyGroup.</returns>
    public PropertyGroup Build()
    {
        return _tab;
    }
}