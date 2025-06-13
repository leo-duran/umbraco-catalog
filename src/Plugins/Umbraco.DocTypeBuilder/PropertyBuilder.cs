using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.DocTypeBuilder;

/// <summary>
/// Builder for creating property types in Umbraco document types.
/// </summary>
public class PropertyBuilder
{
    private readonly PropertyType _property;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyBuilder"/> class.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="alias">The alias of the property.</param>
    /// <param name="editorAlias">The alias of the property editor.</param>
    /// <param name="shortStringHelper">The short string helper used for creating aliases.</param>
    public PropertyBuilder(string name, string alias, string editorAlias, IShortStringHelper shortStringHelper)
    {
        _property = new PropertyType(shortStringHelper, editorAlias, ValueStorageType.Nvarchar, alias)
        {
            Name = name
        };
    }

    /// <summary>
    /// Sets the description of the property.
    /// </summary>
    /// <param name="description">The description to set.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public PropertyBuilder WithDescription(string description)
    {
        _property.Description = description;
        return this;
    }

    /// <summary>
    /// Sets whether the property is mandatory.
    /// </summary>
    /// <param name="mandatory">Whether the property is mandatory.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public PropertyBuilder IsMandatory(bool mandatory = true)
    {
        _property.Mandatory = mandatory;
        return this;
    }

    /// <summary>
    /// Sets the value storage type of the property.
    /// </summary>
    /// <param name="valueStorageType">The value storage type to set.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public PropertyBuilder WithValueStorageType(ValueStorageType valueStorageType)
    {
        _property.ValueStorageType = valueStorageType;
        return this;
    }

    /// <summary>
    /// Sets the sort order of the property.
    /// </summary>
    /// <param name="sortOrder">The sort order to set.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public PropertyBuilder WithSortOrder(int sortOrder)
    {
        _property.SortOrder = sortOrder;
        return this;
    }

    /// <summary>
    /// Sets the validation regular expression for the property.
    /// </summary>
    /// <param name="regex">The regular expression to set.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public PropertyBuilder WithValidationRegex(string regex)
    {
        _property.ValidationRegExp = regex;
        return this;
    }

    /// <summary>
    /// Sets the data type definition ID for the property.
    /// </summary>
    /// <param name="dataTypeDefinitionId">The data type definition ID to set.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public PropertyBuilder WithDataTypeDefinitionId(int dataTypeDefinitionId)
    {
        _property.DataTypeId = dataTypeDefinitionId;
        return this;
    }

    /// <summary>
    /// Sets the label on top for the property.
    /// </summary>
    /// <param name="labelOnTop">Whether the label should be on top.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public PropertyBuilder WithLabelOnTop(bool labelOnTop = true)
    {
        _property.LabelOnTop = labelOnTop;
        return this;
    }

    /// <summary>
    /// Builds and returns the PropertyType instance.
    /// </summary>
    /// <returns>The built PropertyType.</returns>
    public PropertyType Build()
    {
        return _property;
    }
}