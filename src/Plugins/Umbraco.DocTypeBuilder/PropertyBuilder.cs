using System;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.DocTypeBuilder;

/// <summary>
/// Builder for creating Umbraco property types using a fluent API.
/// </summary>
public class PropertyBuilder
{
    private readonly IShortStringHelper _shortStringHelper;
    private string _alias = "defaultProperty";
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _propertyEditorAlias = "Umbraco.TextBox";
    private bool _mandatory = false;
    private bool _labelOnTop = false;
    private string _validationRegExp = string.Empty;
    private string _validationRegExpMessage = string.Empty;
    private ValueStorageType _valueStorageType = ValueStorageType.Nvarchar;
    private int _sortOrder = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyBuilder"/> class.
    /// </summary>
    /// <param name="shortStringHelper">The short string helper used for creating aliases.</param>
    public PropertyBuilder(IShortStringHelper shortStringHelper)
    {
        _shortStringHelper = shortStringHelper;
    }

    /// <summary>
    /// Sets the alias for the property.
    /// </summary>
    /// <param name="alias">The alias to set.</param>
    /// <returns>The current PropertyBuilder instance for method chaining.</returns>
    public PropertyBuilder SetAlias(string alias)
    {
        _alias = alias;
        return this;
    }

    /// <summary>
    /// Sets the name for the property.
    /// </summary>
    /// <param name="name">The name to set.</param>
    /// <returns>The current PropertyBuilder instance for method chaining.</returns>
    public PropertyBuilder SetName(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the description for the property.
    /// </summary>
    /// <param name="description">The description to set.</param>
    /// <returns>The current PropertyBuilder instance for method chaining.</returns>
    public PropertyBuilder SetDescription(string description)
    {
        _description = description;
        return this;
    }

    /// <summary>
    /// Sets the property editor alias.
    /// </summary>
    /// <param name="propertyEditorAlias">The property editor alias to set.</param>
    /// <returns>The current PropertyBuilder instance for method chaining.</returns>
    public PropertyBuilder SetPropertyEditorAlias(string propertyEditorAlias)
    {
        _propertyEditorAlias = propertyEditorAlias;
        return this;
    }

    /// <summary>
    /// Sets whether the property is mandatory.
    /// </summary>
    /// <param name="mandatory">Whether the property is mandatory.</param>
    /// <returns>The current PropertyBuilder instance for method chaining.</returns>
    public PropertyBuilder SetMandatory(bool mandatory = true)
    {
        _mandatory = mandatory;
        return this;
    }

    /// <summary>
    /// Sets whether the label should be on top.
    /// </summary>
    /// <param name="labelOnTop">Whether the label should be on top.</param>
    /// <returns>The current PropertyBuilder instance for method chaining.</returns>
    public PropertyBuilder SetLabelOnTop(bool labelOnTop = true)
    {
        _labelOnTop = labelOnTop;
        return this;
    }

    /// <summary>
    /// Sets the validation regular expression.
    /// </summary>
    /// <param name="validationRegExp">The validation regular expression.</param>
    /// <param name="validationMessage">The validation message.</param>
    /// <returns>The current PropertyBuilder instance for method chaining.</returns>
    public PropertyBuilder SetValidation(string validationRegExp, string validationMessage = "")
    {
        _validationRegExp = validationRegExp;
        _validationRegExpMessage = validationMessage;
        return this;
    }

    /// <summary>
    /// Sets the value storage type.
    /// </summary>
    /// <param name="valueStorageType">The value storage type.</param>
    /// <returns>The current PropertyBuilder instance for method chaining.</returns>
    public PropertyBuilder SetValueStorageType(ValueStorageType valueStorageType)
    {
        _valueStorageType = valueStorageType;
        return this;
    }

    /// <summary>
    /// Sets the sort order.
    /// </summary>
    /// <param name="sortOrder">The sort order.</param>
    /// <returns>The current PropertyBuilder instance for method chaining.</returns>
    public PropertyBuilder SetSortOrder(int sortOrder)
    {
        _sortOrder = sortOrder;
        return this;
    }

    /// <summary>
    /// Builds and returns the configured PropertyType.
    /// </summary>
    /// <returns>The configured PropertyType instance.</returns>
    public PropertyType Build()
    {
        // Use the correct Umbraco 15.4.3 constructor pattern
        var propertyType = new PropertyType(_shortStringHelper, _propertyEditorAlias, _valueStorageType);
        
        // Set the alias after creation (this should work in Umbraco 15)
        propertyType.Alias = _alias;
        
        // Set all the configured properties
        propertyType.Name = _name;
        propertyType.Description = _description;
        propertyType.Mandatory = _mandatory;
        propertyType.LabelOnTop = _labelOnTop;
        propertyType.ValidationRegExp = _validationRegExp;
        propertyType.ValidationRegExpMessage = _validationRegExpMessage;
        propertyType.SortOrder = _sortOrder;
        
        return propertyType;
    }
}