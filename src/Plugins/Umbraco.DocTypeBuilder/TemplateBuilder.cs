using System;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.DocTypeBuilder;

/// <summary>
/// Builder for creating Umbraco templates using a fluent API.
/// </summary>
public class TemplateBuilder
{
    private readonly IShortStringHelper _shortStringHelper;
    private string _alias = "defaultTemplate";
    private string _name = string.Empty;
    private string _content = string.Empty;
    private string _masterTemplateAlias = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateBuilder"/> class.
    /// </summary>
    /// <param name="shortStringHelper">The short string helper used for creating aliases.</param>
    public TemplateBuilder(IShortStringHelper shortStringHelper)
    {
        _shortStringHelper = shortStringHelper;
    }

    /// <summary>
    /// Sets the alias for the template.
    /// </summary>
    /// <param name="alias">The alias to set.</param>
    /// <returns>The current TemplateBuilder instance for method chaining.</returns>
    public TemplateBuilder SetAlias(string alias)
    {
        _alias = alias;
        return this;
    }

    /// <summary>
    /// Sets the name for the template.
    /// </summary>
    /// <param name="name">The name to set.</param>
    /// <returns>The current TemplateBuilder instance for method chaining.</returns>
    public TemplateBuilder SetName(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the content for the template.
    /// </summary>
    /// <param name="content">The content to set.</param>
    /// <returns>The current TemplateBuilder instance for method chaining.</returns>
    public TemplateBuilder SetContent(string content)
    {
        _content = content;
        return this;
    }

    /// <summary>
    /// Sets the master template alias.
    /// </summary>
    /// <param name="masterTemplateAlias">The master template alias to set.</param>
    /// <returns>The current TemplateBuilder instance for method chaining.</returns>
    public TemplateBuilder SetMasterTemplate(string masterTemplateAlias)
    {
        _masterTemplateAlias = masterTemplateAlias;
        return this;
    }

    /// <summary>
    /// Builds and returns the configured Template.
    /// </summary>
    /// <returns>The configured Template instance.</returns>
    public Template Build()
    {
        // Use the correct Umbraco 15.4.3 constructor pattern
        var template = new Template(_shortStringHelper, _name, string.Empty);
        
        // Set the alias after creation (this should work in Umbraco 15)
        template.Alias = _alias;
        
        // Set the content
        template.Content = _content;
        
        // Set master template if specified
        if (!string.IsNullOrEmpty(_masterTemplateAlias))
        {
            template.MasterTemplateAlias = _masterTemplateAlias;
        }
        
        return template;
    }
}