using FluentAssertions;
using Moq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;
using Umbraco.DocTypeBuilder;
using Xunit;

namespace Umbraco.DocTypeBuilder.Tests;

/// <summary>
/// Unit tests for TemplateBuilder class - testing in isolation
/// </summary>
public class TemplateBuilderTests
{
    private readonly Mock<IShortStringHelper> _mockShortStringHelper;

    public TemplateBuilderTests()
    {
        _mockShortStringHelper = new Mock<IShortStringHelper>();
        // Setup common mock behavior
        _mockShortStringHelper
            .Setup(x => x.CleanStringForSafeAlias(It.IsAny<string>()))
            .Returns<string>(input => input?.ToLowerInvariant().Replace(" ", "") ?? string.Empty);
    }

    [Fact]
    public void TemplateBuilder_Should_Create_Basic_Template()
    {
        // Arrange & Act
        var builder = new TemplateBuilder(_mockShortStringHelper.Object);
        var template = builder.Build();

        // Assert
        template.Should().NotBeNull();
        template.Should().BeOfType<Template>();
        template.Name.Should().Be(string.Empty); // Default empty
        template.Alias.Should().Be(string.Empty); // Default empty
        template.Content.Should().Be(string.Empty); // Default empty
    }

    [Fact]
    public void TemplateBuilder_Should_Set_Name_Correctly()
    {
        // Arrange
        const string name = "Home Page Template";

        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithName(name)
            .Build();

        // Assert
        template.Name.Should().Be(name);
    }

    [Fact]
    public void TemplateBuilder_Should_Set_Alias_Correctly()
    {
        // Arrange
        const string alias = "homePageTemplate";

        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithAlias(alias)
            .Build();

        // Assert
        template.Alias.Should().Be(alias);
    }

    [Fact]
    public void TemplateBuilder_Should_Set_Content_Correctly()
    {
        // Arrange
        const string content = "<h1>Hello World</h1>";

        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithContent(content)
            .Build();

        // Assert
        template.Content.Should().Be(content);
    }

    [Fact]
    public void TemplateBuilder_Should_Set_MasterTemplate_By_Alias()
    {
        // Arrange
        const string masterAlias = "masterTemplate";

        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithMasterTemplate(masterAlias)
            .Build();

        // Assert
        template.MasterTemplateAlias.Should().Be(masterAlias);
    }

    [Fact]
    public void TemplateBuilder_Should_Set_MasterTemplate_By_ITemplate()
    {
        // Arrange
        var mockMasterTemplate = new Mock<ITemplate>();
        mockMasterTemplate.Setup(t => t.Alias).Returns("masterTemplate");

        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithMasterTemplate(mockMasterTemplate.Object)
            .Build();

        // Assert
        template.MasterTemplateAlias.Should().Be("masterTemplate");
    }

    [Fact]
    public void TemplateBuilder_Should_Set_VirtualPath_Correctly()
    {
        // Arrange
        const string virtualPath = "~/Views/HomePage.cshtml";

        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithVirtualPath(virtualPath)
            .Build();

        // Assert
        template.VirtualPath.Should().Be(virtualPath);
    }

    [Fact]
    public void TemplateBuilder_Should_Create_Basic_Html_Structure()
    {
        // Arrange
        const string title = "Test Page";
        const string bodyContent = "<p>Test content</p>";

        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithBasicHtmlStructure(title, bodyContent)
            .Build();

        // Assert
        template.Content.Should().Contain(title);
        template.Content.Should().Contain(bodyContent);
        template.Content.Should().Contain("Layout = \"master\"");
    }

    [Fact]
    public void TemplateBuilder_Should_Create_Basic_Html_Structure_With_Defaults()
    {
        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithBasicHtmlStructure()
            .Build();

        // Assert
        template.Content.Should().Contain("Page Title");
        template.Content.Should().Contain("Layout = \"master\"");
    }

    [Fact]
    public void TemplateBuilder_Should_Create_Master_Template_With_Full_Structure()
    {
        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .AsMasterTemplate(includeHead: true, includeFooter: true)
            .Build();

        // Assert
        template.Content.Should().Contain("<!DOCTYPE html>");
        template.Content.Should().Contain("<html>");
        template.Content.Should().Contain("<head>");
        template.Content.Should().Contain("<body>");
        template.Content.Should().Contain("@RenderBody()");
        template.Content.Should().Contain("<footer>");
        template.Content.Should().Contain("@RenderSection(\"header\", false)");
        template.Content.Should().Contain("@RenderSection(\"footer\", false)");
        template.Content.Should().Contain("@RenderSection(\"scripts\", false)");
    }

    [Fact]
    public void TemplateBuilder_Should_Create_Master_Template_Without_Head_And_Footer()
    {
        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .AsMasterTemplate(includeHead: false, includeFooter: false)
            .Build();

        // Assert
        template.Content.Should().Contain("<!DOCTYPE html>");
        template.Content.Should().Contain("@RenderBody()");
        template.Content.Should().NotContain("@RenderSection(\"head\", false)");
        template.Content.Should().NotContain("@RenderSection(\"footer\", false)");
    }

    [Fact]
    public void TemplateBuilder_Should_Add_Umbraco_Model_Directive()
    {
        // Arrange
        const string modelType = "MyCustomModel";

        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithContent("<h1>Test</h1>")
            .WithUmbracoModel(modelType)
            .Build();

        // Assert
        template.Content.Should().Contain($"@model {modelType}");
        template.Content.Should().Contain("<h1>Test</h1>");
    }

    [Fact]
    public void TemplateBuilder_Should_Add_Default_Umbraco_Model()
    {
        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithContent("<h1>Test</h1>")
            .WithUmbracoModel()
            .Build();

        // Assert
        template.Content.Should().Contain("@model ContentModel");
    }

    [Fact]
    public void TemplateBuilder_Should_Not_Duplicate_Model_Directive()
    {
        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithContent("@model ExistingModel\n<h1>Test</h1>")
            .WithUmbracoModel("NewModel")
            .Build();

        // Assert
        template.Content.Should().Contain("@model ExistingModel");
        template.Content.Should().NotContain("@model NewModel");
    }

    [Fact]
    public void TemplateBuilder_Should_Add_Property_Rendering()
    {
        // Arrange
        var properties = new[] { "title", "content", "summary" };

        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithContent("<h1>Header</h1>")
            .WithPropertyRendering(properties)
            .Build();

        // Assert
        template.Content.Should().Contain("<h1>Header</h1>");
        template.Content.Should().Contain("@Model.Value(\"title\")");
        template.Content.Should().Contain("@Model.Value(\"content\")");
        template.Content.Should().Contain("@Model.Value(\"summary\")");
    }

    [Fact]
    public void TemplateBuilder_Should_Handle_Empty_Property_Array()
    {
        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithContent("<h1>Test</h1>")
            .WithPropertyRendering()
            .Build();

        // Assert
        template.Content.Should().Be("<h1>Test</h1>");
    }

    [Fact]
    public void TemplateBuilder_Should_Handle_Null_Property_Array()
    {
        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithContent("<h1>Test</h1>")
            .WithPropertyRendering(null!)
            .Build();

        // Assert
        template.Content.Should().Be("<h1>Test</h1>");
    }

    [Fact]
    public void TemplateBuilder_Should_Add_Navigation()
    {
        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithNavigation(startLevel: 1, maxLevels: 3)
            .Build();

        // Assert
        template.Content.Should().Contain("<nav>");
        template.Content.Should().Contain("@foreach(var item in Model.Root().Children.Where(x => x.IsVisible()))");
        template.Content.Should().Contain("@item.Url");
        template.Content.Should().Contain("@item.Name");
    }

    [Fact]
    public void TemplateBuilder_Should_Add_Children_Section()
    {
        // Arrange
        const string sectionName = "Related Articles";

        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithChildrenSection(sectionName)
            .Build();

        // Assert
        template.Content.Should().Contain($"<h2>{sectionName}</h2>");
        template.Content.Should().Contain("class=\"related articles\"");
        template.Content.Should().Contain("@foreach(var child in Model.Children.Where(x => x.IsVisible()))");
        template.Content.Should().Contain("@child.Url");
        template.Content.Should().Contain("@child.Name");
    }

    [Fact]
    public void TemplateBuilder_Should_Add_Children_Section_With_Default_Name()
    {
        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithChildrenSection()
            .Build();

        // Assert
        template.Content.Should().Contain("<h2>Children</h2>");
        template.Content.Should().Contain("class=\"children\"");
    }

    [Fact]
    public void TemplateBuilder_Should_Add_CSS_Assets()
    {
        // Arrange
        var cssFiles = new[] { "/css/main.css", "/css/theme.css" };

        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithAssets(cssFiles: cssFiles)
            .Build();

        // Assert
        template.Content.Should().Contain("<link rel=\"stylesheet\" href=\"/css/main.css\">");
        template.Content.Should().Contain("<link rel=\"stylesheet\" href=\"/css/theme.css\">");
    }

    [Fact]
    public void TemplateBuilder_Should_Add_JavaScript_Assets()
    {
        // Arrange
        var jsFiles = new[] { "/js/app.js", "/js/analytics.js" };

        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithAssets(jsFiles: jsFiles)
            .Build();

        // Assert
        template.Content.Should().Contain("<script src=\"/js/app.js\"></script>");
        template.Content.Should().Contain("<script src=\"/js/analytics.js\"></script>");
    }

    [Fact]
    public void TemplateBuilder_Should_Add_Both_CSS_And_JavaScript_Assets()
    {
        // Arrange
        var cssFiles = new[] { "/css/main.css" };
        var jsFiles = new[] { "/js/app.js" };

        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithAssets(cssFiles: cssFiles, jsFiles: jsFiles)
            .Build();

        // Assert
        template.Content.Should().Contain("<link rel=\"stylesheet\" href=\"/css/main.css\">");
        template.Content.Should().Contain("<script src=\"/js/app.js\"></script>");
    }

    [Fact]
    public void TemplateBuilder_Should_Handle_Null_Assets()
    {
        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithContent("<h1>Test</h1>")
            .WithAssets(cssFiles: null, jsFiles: null)
            .Build();

        // Assert
        template.Content.Should().Be("<h1>Test</h1>");
    }

    [Fact]
    public void TemplateBuilder_Should_Add_Basic_SEO_Tags()
    {
        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithSeoTags(includeOpenGraph: false, includeTwitterCard: false)
            .Build();

        // Assert
        template.Content.Should().Contain("@if(Model.HasValue(\"metaTitle\"))");
        template.Content.Should().Contain("<title>@Model.Value(\"metaTitle\")</title>");
        template.Content.Should().Contain("<title>@Model.Name</title>");
        template.Content.Should().Contain("@if(Model.HasValue(\"metaDescription\"))");
        template.Content.Should().Contain("<meta name=\"description\"");
    }

    [Fact]
    public void TemplateBuilder_Should_Add_SEO_Tags_With_OpenGraph()
    {
        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithSeoTags(includeOpenGraph: true, includeTwitterCard: false)
            .Build();

        // Assert
        template.Content.Should().Contain("<meta property=\"og:title\"");
        template.Content.Should().Contain("<meta property=\"og:description\"");
        template.Content.Should().Contain("<meta property=\"og:url\"");
        template.Content.Should().NotContain("twitter:card");
    }

    [Fact]
    public void TemplateBuilder_Should_Add_SEO_Tags_With_Twitter_Card()
    {
        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithSeoTags(includeOpenGraph: false, includeTwitterCard: true)
            .Build();

        // Assert
        template.Content.Should().Contain("<meta name=\"twitter:card\"");
        template.Content.Should().Contain("<meta name=\"twitter:title\"");
        template.Content.Should().Contain("<meta name=\"twitter:description\"");
        template.Content.Should().NotContain("og:title");
    }

    [Fact]
    public void TemplateBuilder_Should_Add_All_SEO_Tags()
    {
        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithSeoTags(includeOpenGraph: true, includeTwitterCard: true)
            .Build();

        // Assert
        template.Content.Should().Contain("<meta property=\"og:title\"");
        template.Content.Should().Contain("<meta name=\"twitter:card\"");
    }

    [Fact]
    public void TemplateBuilder_Should_Chain_All_Configuration_Methods()
    {
        // Arrange
        const string name = "Complete Template";
        const string alias = "completeTemplate";
        const string masterAlias = "masterLayout";
        const string virtualPath = "~/Views/Complete.cshtml";

        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithName(name)
            .WithAlias(alias)
            .WithMasterTemplate(masterAlias)
            .WithVirtualPath(virtualPath)
            .WithBasicHtmlStructure("Test Page", "<p>Content</p>")
            .WithUmbracoModel("CustomModel")
            .WithPropertyRendering("title", "content")
            .WithNavigation()
            .WithChildrenSection("Related")
            .WithAssets(new[] { "/css/style.css" }, new[] { "/js/script.js" })
            .WithSeoTags()
            .Build();

        // Assert
        template.Name.Should().Be(name);
        template.Alias.Should().Be(alias);
        template.MasterTemplateAlias.Should().Be(masterAlias);
        template.VirtualPath.Should().Be(virtualPath);
        template.Content.Should().Contain("Test Page");
        template.Content.Should().Contain("@model CustomModel");
        template.Content.Should().Contain("@Model.Value(\"title\")");
        template.Content.Should().Contain("<nav>");
        template.Content.Should().Contain("<h2>Related</h2>");
        template.Content.Should().Contain("/css/style.css");
        template.Content.Should().Contain("/js/script.js");
        template.Content.Should().Contain("og:title");
    }

    [Fact]
    public void TemplateBuilder_Should_Return_Same_Instance_For_Method_Chaining()
    {
        // Arrange
        var builder = new TemplateBuilder(_mockShortStringHelper.Object);

        // Act & Assert - Each method should return the same builder instance
        var result1 = builder.WithName("Test");
        var result2 = result1.WithAlias("test");
        var result3 = result2.WithContent("<h1>Test</h1>");
        var result4 = result3.WithMasterTemplate("master");

        result1.Should().BeSameAs(builder);
        result2.Should().BeSameAs(builder);
        result3.Should().BeSameAs(builder);
        result4.Should().BeSameAs(builder);
    }

    [Fact]
    public void TemplateBuilder_Should_Handle_Empty_String_Values()
    {
        // Act
        var template = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithName("")
            .WithAlias("")
            .WithContent("")
            .WithMasterTemplate("")
            .WithVirtualPath("")
            .Build();

        // Assert
        template.Name.Should().Be("");
        template.Alias.Should().Be("");
        template.Content.Should().Be("");
        template.MasterTemplateAlias.Should().Be("");
        template.VirtualPath.Should().Be("");
    }

    [Fact]
    public void TemplateBuilder_Should_Build_Multiple_Different_Templates()
    {
        // Act - Create different types of templates
        var masterTemplate = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithName("Master Layout")
            .WithAlias("masterLayout")
            .AsMasterTemplate()
            .Build();

        var pageTemplate = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithName("Page Template")
            .WithAlias("pageTemplate")
            .WithMasterTemplate("masterLayout")
            .WithBasicHtmlStructure("Page Title")
            .WithUmbracoModel()
            .Build();

        var blogTemplate = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithName("Blog Template")
            .WithAlias("blogTemplate")
            .WithPropertyRendering("title", "content", "publishDate")
            .WithChildrenSection("Related Posts")
            .Build();

        // Assert
        masterTemplate.Should().NotBeNull();
        masterTemplate.Content.Should().Contain("@RenderBody()");

        pageTemplate.Should().NotBeNull();
        pageTemplate.MasterTemplateAlias.Should().Be("masterLayout");
        pageTemplate.Content.Should().Contain("@model ContentModel");

        blogTemplate.Should().NotBeNull();
        blogTemplate.Content.Should().Contain("@Model.Value(\"publishDate\")");
        blogTemplate.Content.Should().Contain("Related Posts");
    }

    [Fact]
    public void TemplateBuilder_Should_Create_Real_World_Landing_Page_Template()
    {
        // Act - Create a realistic landing page template
        var landingPageTemplate = new TemplateBuilder(_mockShortStringHelper.Object)
            .WithName("Landing Page Template")
            .WithAlias("landingPageTemplate")
            .WithMasterTemplate("masterLayout")
            .WithUmbracoModel("ContentModel")
            .WithBasicHtmlStructure("@Model.Value(\"heroTitle\")", 
                "@Model.Value(\"heroSubtitle\")")
            .WithPropertyRendering("mainContent", "ctaText", "ctaLink")
            .WithAssets(
                cssFiles: new[] { "/css/landing.css", "/css/animations.css" },
                jsFiles: new[] { "/js/tracking.js", "/js/forms.js" })
            .WithSeoTags(includeOpenGraph: true, includeTwitterCard: true)
            .Build();

        // Assert
        landingPageTemplate.Should().NotBeNull();
        landingPageTemplate.Name.Should().Be("Landing Page Template");
        landingPageTemplate.Alias.Should().Be("landingPageTemplate");
        landingPageTemplate.MasterTemplateAlias.Should().Be("masterLayout");
        
        // Verify content includes all expected elements
        var content = landingPageTemplate.Content;
        content.Should().Contain("@model ContentModel");
        content.Should().Contain("@Model.Value(\"heroTitle\")");
        content.Should().Contain("@Model.Value(\"mainContent\")");
        content.Should().Contain("/css/landing.css");
        content.Should().Contain("/js/tracking.js");
        content.Should().Contain("og:title");
        content.Should().Contain("twitter:card");
    }
}