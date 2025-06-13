using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.DocTypeBuilder;

/// <summary>
/// Builder for creating Umbraco templates using a fluent API.
/// </summary>
public class TemplateBuilder
{
    private readonly Template _template;
    private readonly IShortStringHelper _shortStringHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateBuilder"/> class.
    /// </summary>
    /// <param name="shortStringHelper">The short string helper used for creating aliases.</param>
    public TemplateBuilder(IShortStringHelper shortStringHelper)
    {
        _shortStringHelper = shortStringHelper;
        _template = new Template(_shortStringHelper, string.Empty, string.Empty);
    }

    /// <summary>
    /// Sets the name of the template.
    /// </summary>
    /// <param name="name">The name to set.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TemplateBuilder WithName(string name)
    {
        _template.Name = name;
        return this;
    }

    /// <summary>
    /// Sets the alias of the template.
    /// </summary>
    /// <param name="alias">The alias to set.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TemplateBuilder WithAlias(string alias)
    {
        _template.Alias = alias;
        return this;
    }

    /// <summary>
    /// Sets the content (markup) of the template.
    /// </summary>
    /// <param name="content">The template content to set.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TemplateBuilder WithContent(string content)
    {
        _template.Content = content;
        return this;
    }

    /// <summary>
    /// Sets the master template alias for template inheritance.
    /// </summary>
    /// <param name="masterTemplateAlias">The master template alias to set.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TemplateBuilder WithMasterTemplate(string masterTemplateAlias)
    {
        _template.MasterTemplateAlias = masterTemplateAlias;
        return this;
    }

    /// <summary>
    /// Sets the master template for template inheritance.
    /// </summary>
    /// <param name="masterTemplate">The master template to inherit from.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TemplateBuilder WithMasterTemplate(ITemplate masterTemplate)
    {
        _template.MasterTemplateAlias = masterTemplate.Alias;
        return this;
    }

    /// <summary>
    /// Sets the virtual path of the template.
    /// </summary>
    /// <param name="virtualPath">The virtual path to set.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TemplateBuilder WithVirtualPath(string virtualPath)
    {
        _template.VirtualPath = virtualPath;
        return this;
    }

    /// <summary>
    /// Adds basic HTML structure to the template content.
    /// </summary>
    /// <param name="title">The page title.</param>
    /// <param name="bodyContent">The body content.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TemplateBuilder WithBasicHtmlStructure(string title = "Page Title", string bodyContent = "")
    {
        var content = $@"@{{
    Layout = ""master"";
}}

<h1>{title}</h1>
{bodyContent}";
        _template.Content = content;
        return this;
    }

    /// <summary>
    /// Adds a master page layout structure to the template.
    /// </summary>
    /// <param name="includeHead">Whether to include head section.</param>
    /// <param name="includeFooter">Whether to include footer section.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TemplateBuilder AsMasterTemplate(bool includeHead = true, bool includeFooter = true)
    {
        var headSection = includeHead ? @"<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>@ViewBag.Title</title>
    @RenderSection(""head"", false)
</head>" : "<head><title>@ViewBag.Title</title></head>";

        var footerSection = includeFooter ? @"    <footer>
        @RenderSection(""footer"", false)
    </footer>" : "";

        var content = $@"<!DOCTYPE html>
<html>
{headSection}
<body>
    <header>
        @RenderSection(""header"", false)
    </header>
    
    <main>
        @RenderBody()
    </main>
    
{footerSection}
    
    @RenderSection(""scripts"", false)
</body>
</html>";

        _template.Content = content;
        return this;
    }

    /// <summary>
    /// Adds Umbraco-specific template helpers and model binding.
    /// </summary>
    /// <param name="modelType">The strongly-typed model type.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TemplateBuilder WithUmbracoModel(string modelType = "ContentModel")
    {
        var existingContent = _template.Content ?? "";
        var modelDirective = $"@model {modelType}";
        
        if (!existingContent.Contains("@model"))
        {
            _template.Content = $"{modelDirective}\n{existingContent}";
        }
        
        return this;
    }

    /// <summary>
    /// Adds common Umbraco property rendering to the template.
    /// </summary>
    /// <param name="propertyAliases">The property aliases to render.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TemplateBuilder WithPropertyRendering(params string[] propertyAliases)
    {
        if (propertyAliases == null || propertyAliases.Length == 0)
            return this;

        var existingContent = _template.Content ?? "";
        var propertyContent = string.Join("\n", propertyAliases.Select(alias => 
            $"@Model.Value(\"{alias}\")"));

        _template.Content = existingContent + "\n" + propertyContent;
        return this;
    }

    /// <summary>
    /// Adds navigation rendering to the template.
    /// </summary>
    /// <param name="startLevel">The level to start navigation from.</param>
    /// <param name="maxLevels">Maximum levels to show.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TemplateBuilder WithNavigation(int startLevel = 1, int maxLevels = 3)
    {
        var navContent = $@"
<nav>
    @foreach(var item in Model.Root().Children.Where(x => x.IsVisible()))
    {{
        <a href=""@item.Url"">@item.Name</a>
    }}
</nav>";

        var existingContent = _template.Content ?? "";
        _template.Content = existingContent + navContent;
        return this;
    }

    /// <summary>
    /// Adds a content section to the template that renders child pages.
    /// </summary>
    /// <param name="sectionName">The name of the section.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TemplateBuilder WithChildrenSection(string sectionName = "Children")
    {
        var childrenContent = $@"
@if(Model.Children.Any())
{{
    <section class=""{sectionName.ToLower()}"">
        <h2>{sectionName}</h2>
        @foreach(var child in Model.Children.Where(x => x.IsVisible()))
        {{
            <article>
                <h3><a href=""@child.Url"">@child.Name</a></h3>
                @if(child.HasValue(""summary""))
                {{
                    <p>@child.Value(""summary"")</p>
                }}
            </article>
        }}
    </section>
}}";

        var existingContent = _template.Content ?? "";
        _template.Content = existingContent + childrenContent;
        return this;
    }

    /// <summary>
    /// Adds CSS and JavaScript references to the template.
    /// </summary>
    /// <param name="cssFiles">CSS file paths to include.</param>
    /// <param name="jsFiles">JavaScript file paths to include.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TemplateBuilder WithAssets(string[]? cssFiles = null, string[]? jsFiles = null)
    {
        var assetContent = "";

        if (cssFiles?.Length > 0)
        {
            assetContent += string.Join("\n", cssFiles.Select(css => 
                $"<link rel=\"stylesheet\" href=\"{css}\">"));
        }

        if (jsFiles?.Length > 0)
        {
            assetContent += "\n" + string.Join("\n", jsFiles.Select(js => 
                $"<script src=\"{js}\"></script>"));
        }

        if (!string.IsNullOrEmpty(assetContent))
        {
            var existingContent = _template.Content ?? "";
            _template.Content = existingContent + "\n" + assetContent;
        }

        return this;
    }

    /// <summary>
    /// Adds SEO meta tags to the template.
    /// </summary>
    /// <param name="includeOpenGraph">Whether to include Open Graph tags.</param>
    /// <param name="includeTwitterCard">Whether to include Twitter Card tags.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public TemplateBuilder WithSeoTags(bool includeOpenGraph = true, bool includeTwitterCard = true)
    {
        var seoContent = @"
@if(Model.HasValue(""metaTitle""))
{
    <title>@Model.Value(""metaTitle"")</title>
}
else
{
    <title>@Model.Name</title>
}

@if(Model.HasValue(""metaDescription""))
{
    <meta name=""description"" content=""@Model.Value(""metaDescription"")"">
}";

        if (includeOpenGraph)
        {
            seoContent += @"

<meta property=""og:title"" content=""@(Model.HasValue(""metaTitle"") ? Model.Value(""metaTitle"") : Model.Name)"">
@if(Model.HasValue(""metaDescription""))
{
    <meta property=""og:description"" content=""@Model.Value(""metaDescription"")"">
}
<meta property=""og:url"" content=""@Model.UrlAbsolute()"">";
        }

        if (includeTwitterCard)
        {
            seoContent += @"

<meta name=""twitter:card"" content=""summary"">
<meta name=""twitter:title"" content=""@(Model.HasValue(""metaTitle"") ? Model.Value(""metaTitle"") : Model.Name)"">
@if(Model.HasValue(""metaDescription""))
{
    <meta name=""twitter:description"" content=""@Model.Value(""metaDescription"")"">
}";
        }

        var existingContent = _template.Content ?? "";
        _template.Content = existingContent + seoContent;
        return this;
    }

    /// <summary>
    /// Builds and returns the Template instance.
    /// </summary>
    /// <returns>The built Template.</returns>
    public Template Build()
    {
        return _template;
    }
}