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
    /// Adds a document type property to the tab.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="description">The description of the property.</param>
    /// <param name="mandatory">Whether the property is mandatory.</param>
    /// <param name="sortOrder">The sort order of the property.</param>
    /// <param name="validationRegex">The validation regular expression.</param>
    /// <param name="labelOnTop">Whether the label should be on top.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder AddDocumentTypeProperty(
        string name,
        string alias,
        string? description = null,
        bool mandatory = false,
        int sortOrder = 0,
        string? validationRegex = null,
        bool labelOnTop = false)
    {
        return AddProperty(name, alias, "Umbraco.DocumentType", description, mandatory, sortOrder, validationRegex, labelOnTop);
    }

    /// <summary>
    /// Adds a property to the tab.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="editorAlias">The alias of the property editor.</param>
    /// <param name="description">The description of the property.</param>
    /// <param name="mandatory">Whether the property is mandatory.</param>
    /// <param name="sortOrder">The sort order of the property.</param>
    /// <param name="validationRegex">The validation regular expression.</param>
    /// <param name="labelOnTop">Whether the label should be on top.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder AddProperty(
        string name,
        string alias,
        string editorAlias,
        string? description = null,
        bool mandatory = false,
        int sortOrder = 0,
        string? validationRegex = null,
        bool labelOnTop = false)
    {
        var propertyBuilder = new PropertyBuilder(name, alias, editorAlias, _shortStringHelper);

        if (!string.IsNullOrEmpty(description))
            propertyBuilder.WithDescription(description);

        if (mandatory)
            propertyBuilder.IsMandatory(mandatory);

        if (sortOrder != 0)
            propertyBuilder.WithSortOrder(sortOrder);

        if (!string.IsNullOrEmpty(validationRegex))
            propertyBuilder.WithValidationRegex(validationRegex);

        if (labelOnTop)
            propertyBuilder.WithLabelOnTop(labelOnTop);

        _tab.PropertyTypes?.Add(propertyBuilder.Build());
        return this;
    }

    /// <summary>
    /// Adds a text box property to the tab.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="description">The description of the property.</param>
    /// <param name="mandatory">Whether the property is mandatory.</param>
    /// <param name="sortOrder">The sort order of the property.</param>
    /// <param name="validationRegex">The validation regular expression.</param>
    /// <param name="labelOnTop">Whether the label should be on top.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder AddTextBoxProperty(
        string name,
        string alias,
        string? description = null,
        bool mandatory = false,
        int sortOrder = 0,
        string? validationRegex = null,
        bool labelOnTop = false)
    {
        return AddProperty(name, alias, "Umbraco.TextBox", description, mandatory, sortOrder, validationRegex, labelOnTop);
    }

    /// <summary>
    /// Adds a text area property to the tab.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="description">The description of the property.</param>
    /// <param name="mandatory">Whether the property is mandatory.</param>
    /// <param name="sortOrder">The sort order of the property.</param>
    /// <param name="validationRegex">The validation regular expression.</param>
    /// <param name="labelOnTop">Whether the label should be on top.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder AddTextAreaProperty(
        string name,
        string alias,
        string? description = null,
        bool mandatory = false,
        int sortOrder = 0,
        string? validationRegex = null,
        bool labelOnTop = false)
    {
        return AddProperty(name, alias, "Umbraco.TextArea", description, mandatory, sortOrder, validationRegex, labelOnTop);
    }

    /// <summary>
    /// Adds a rich text editor property to the tab.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="description">The description of the property.</param>
    /// <param name="mandatory">Whether the property is mandatory.</param>
    /// <param name="sortOrder">The sort order of the property.</param>
    /// <param name="validationRegex">The validation regular expression.</param>
    /// <param name="labelOnTop">Whether the label should be on top.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder AddRichTextProperty(
        string name,
        string alias,
        string? description = null,
        bool mandatory = false,
        int sortOrder = 0,
        string? validationRegex = null,
        bool labelOnTop = false)
    {
        return AddProperty(name, alias, "Umbraco.TinyMCE", description, mandatory, sortOrder, validationRegex, labelOnTop);
    }

    /// <summary>
    /// Adds a media picker property to the tab.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="description">The description of the property.</param>
    /// <param name="mandatory">Whether the property is mandatory.</param>
    /// <param name="sortOrder">The sort order of the property.</param>
    /// <param name="validationRegex">The validation regular expression.</param>
    /// <param name="labelOnTop">Whether the label should be on top.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder AddMediaPickerProperty(
        string name,
        string alias,
        string? description = null,
        bool mandatory = false,
        int sortOrder = 0,
        string? validationRegex = null,
        bool labelOnTop = false)
    {
        return AddProperty(name, alias, "Umbraco.MediaPicker3", description, mandatory, sortOrder, validationRegex, labelOnTop);
    }

    /// <summary>
    /// Adds a content picker property to the tab.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="description">The description of the property.</param>
    /// <param name="mandatory">Whether the property is mandatory.</param>
    /// <param name="sortOrder">The sort order of the property.</param>
    /// <param name="validationRegex">The validation regular expression.</param>
    /// <param name="labelOnTop">Whether the label should be on top.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder AddContentPickerProperty(
        string name,
        string alias,
        string? description = null,
        bool mandatory = false,
        int sortOrder = 0,
        string? validationRegex = null,
        bool labelOnTop = false)
    {
        return AddProperty(name, alias, "Umbraco.ContentPicker", description, mandatory, sortOrder, validationRegex, labelOnTop);
    }

    /// <summary>
    /// Adds a numeric property to the tab.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="description">The description of the property.</param>
    /// <param name="mandatory">Whether the property is mandatory.</param>
    /// <param name="sortOrder">The sort order of the property.</param>
    /// <param name="validationRegex">The validation regular expression.</param>
    /// <param name="labelOnTop">Whether the label should be on top.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder AddNumericProperty(
        string name,
        string alias,
        string? description = null,
        bool mandatory = false,
        int sortOrder = 0,
        string? validationRegex = null,
        bool labelOnTop = false)
    {
        return AddProperty(name, alias, "Umbraco.Integer", description, mandatory, sortOrder, validationRegex, labelOnTop);
    }

    /// <summary>
    /// Adds a checkbox property to the tab.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="description">The description of the property.</param>
    /// <param name="mandatory">Whether the property is mandatory.</param>
    /// <param name="sortOrder">The sort order of the property.</param>
    /// <param name="validationRegex">The validation regular expression.</param>
    /// <param name="labelOnTop">Whether the label should be on top.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder AddCheckboxProperty(
        string name,
        string alias,
        string? description = null,
        bool mandatory = false,
        int sortOrder = 0,
        string? validationRegex = null,
        bool labelOnTop = false)
    {
        return AddProperty(name, alias, "Umbraco.TrueFalse", description, mandatory, sortOrder, validationRegex, labelOnTop);
    }

    /// <summary>
    /// Adds a date picker property to the tab.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="description">The description of the property.</param>
    /// <param name="mandatory">Whether the property is mandatory.</param>
    /// <param name="sortOrder">The sort order of the property.</param>
    /// <param name="validationRegex">The validation regular expression.</param>
    /// <param name="labelOnTop">Whether the label should be on top.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TabBuilder AddDatePickerProperty(
        string name,
        string alias,
        string? description = null,
        bool mandatory = false,
        int sortOrder = 0,
        string? validationRegex = null,
        bool labelOnTop = false)
    {
        return AddProperty(name, alias, "Umbraco.DateTime", description, mandatory, sortOrder, validationRegex, labelOnTop);
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