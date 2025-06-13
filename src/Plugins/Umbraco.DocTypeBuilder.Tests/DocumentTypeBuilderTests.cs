using FluentAssertions;
using Moq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;
using Umbraco.DocTypeBuilder;
using Xunit;

namespace Umbraco.DocTypeBuilder.Tests;

/// <summary>
/// Unit tests for DocumentTypeBuilder class - testing in isolation
/// </summary>
public class DocumentTypeBuilderTests
{
    private readonly Mock<IShortStringHelper> _mockShortStringHelper;

    public DocumentTypeBuilderTests()
    {
        _mockShortStringHelper = new Mock<IShortStringHelper>();
        // Setup common mock behavior
        _mockShortStringHelper
            .Setup(x => x.CleanStringForSafeAlias(It.IsAny<string>()))
            .Returns<string>(input => input?.ToLowerInvariant().Replace(" ", "") ?? string.Empty);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Create_Basic_Document_Type()
    {
        // Arrange & Act
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);
        var docType = builder.Build();

        // Assert
        docType.Should().NotBeNull();
        docType.Should().BeOfType<ContentType>();
        docType.IsElement.Should().BeFalse(); // Default is not an element
        docType.AllowedAsRoot.Should().BeFalse(); // Default is not allowed at root
        docType.PropertyGroups.Should().BeEmpty();
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Set_Alias_Correctly()
    {
        // Arrange
        const string alias = "homePage";

        // Act
        var docType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias(alias)
            .Build();

        // Assert
        docType.Alias.Should().Be(alias);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Set_Name_Correctly()
    {
        // Arrange
        const string name = "Home Page";

        // Act
        var docType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithName(name)
            .Build();

        // Assert
        docType.Name.Should().Be(name);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Set_Description_Correctly()
    {
        // Arrange
        const string description = "The main landing page";

        // Act
        var docType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithDescription(description)
            .Build();

        // Assert
        docType.Description.Should().Be(description);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Set_Icon_Correctly()
    {
        // Arrange
        const string icon = "icon-home";

        // Act
        var docType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithIcon(icon)
            .Build();

        // Assert
        docType.Icon.Should().Be(icon);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Set_AllowAtRoot_Correctly()
    {
        // Act
        var allowedAtRoot = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .AllowAtRoot(true)
            .Build();

        var notAllowedAtRoot = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .AllowAtRoot(false)
            .Build();

        var defaultAllowAtRoot = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .AllowAtRoot() // Should default to true
            .Build();

        // Assert
        allowedAtRoot.AllowedAsRoot.Should().BeTrue();
        notAllowedAtRoot.AllowedAsRoot.Should().BeFalse();
        defaultAllowAtRoot.AllowedAsRoot.Should().BeTrue();
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Set_IsElement_Correctly()
    {
        // Act
        var elementType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .IsElement(true)
            .Build();

        var documentType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .IsElement(false)
            .Build();

        var defaultElementType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .IsElement() // Should default to true
            .Build();

        // Assert
        elementType.IsElement.Should().BeTrue();
        documentType.IsElement.Should().BeFalse();
        defaultElementType.IsElement.Should().BeTrue();
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Add_Single_Tab()
    {
        // Arrange
        const string tabName = "Content";

        // Act
        var docType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .AddTab(tabName, tab => tab
                .WithAlias("content")
                .WithSortOrder(1))
            .Build();

        // Assert
        docType.PropertyGroups.Should().HaveCount(1);
        var tab = docType.PropertyGroups.First();
        tab.Name.Should().Be(tabName);
        tab.Alias.Should().Be("content");
        tab.SortOrder.Should().Be(1);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Add_Multiple_Tabs()
    {
        // Act
        var docType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .AddTab("Content", tab => tab.WithSortOrder(1))
            .AddTab("Settings", tab => tab.WithSortOrder(2))
            .AddTab("SEO", tab => tab.WithSortOrder(3))
            .Build();

        // Assert
        docType.PropertyGroups.Should().HaveCount(3);
        var tabNames = docType.PropertyGroups.Select(t => t.Name).ToList();
        tabNames.Should().Contain("Content");
        tabNames.Should().Contain("Settings");
        tabNames.Should().Contain("SEO");
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Add_Tab_With_Properties()
    {
        // Act
        var docType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .AddTab("Content", tab => tab
                .WithAlias("content")
                .AddTextBoxProperty("Title", "title", prop => prop.IsMandatory())
                .AddRichTextProperty("Content", "content")
                .AddMediaPickerProperty("Image", "image"))
            .Build();

        // Assert
        docType.PropertyGroups.Should().HaveCount(1);
        var tab = docType.PropertyGroups.First();
        tab.PropertyTypes.Should().HaveCount(3);
        
        var propertyAliases = tab.PropertyTypes!.Select(p => p.Alias).ToList();
        propertyAliases.Should().Contain("title");
        propertyAliases.Should().Contain("content");
        propertyAliases.Should().Contain("image");
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Add_Composition()
    {
        // Arrange - Create a base document type first
        var baseDocType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("basePage")
            .WithName("Base Page")
            .Build();

        // Act
        var docType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("contentPage")
            .WithName("Content Page")
            .AddComposition(baseDocType)
            .Build();

        // Assert
        docType.ContentTypeComposition.Should().HaveCount(1);
        docType.ContentTypeComposition.Should().Contain(baseDocType);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Add_Multiple_Compositions()
    {
        // Arrange - Create base document types
        var seoBase = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("seoBase")
            .WithName("SEO Base")
            .Build();

        var trackingBase = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("trackingBase")
            .WithName("Tracking Base")
            .Build();

        // Act
        var docType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("advancedPage")
            .WithName("Advanced Page")
            .AddComposition(seoBase)
            .AddComposition(trackingBase)
            .Build();

        // Assert
        docType.ContentTypeComposition.Should().HaveCount(2);
        docType.ContentTypeComposition.Should().Contain(seoBase);
        docType.ContentTypeComposition.Should().Contain(trackingBase);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Create_Complete_Document_Type()
    {
        // Act
        var docType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("productPage")
            .WithName("Product Page")
            .WithDescription("A page for displaying products")
            .WithIcon("icon-shopping-basket")
            .AllowAtRoot(false)
            .IsElement(false)
            .AddTab("Basic Info", tab => tab
                .WithAlias("basicInfo")
                .WithSortOrder(1)
                .AddTextBoxProperty("Product Name", "productName", prop => prop
                    .IsMandatory()
                    .WithDescription("The name of the product"))
                .AddTextAreaProperty("Short Description", "shortDescription")
                .AddRichTextProperty("Full Description", "fullDescription"))
            .AddTab("Pricing", tab => tab
                .WithAlias("pricing")
                .WithSortOrder(2)
                .AddNumericProperty("Price", "price", prop => prop
                    .IsMandatory()
                    .WithValueStorageType(ValueStorageType.Decimal))
                .AddCheckboxProperty("On Sale", "onSale"))
            .Build();

        // Assert
        docType.Alias.Should().Be("productPage");
        docType.Name.Should().Be("Product Page");
        docType.Description.Should().Be("A page for displaying products");
        docType.Icon.Should().Be("icon-shopping-basket");
        docType.AllowedAsRoot.Should().BeFalse();
        docType.IsElement.Should().BeFalse();
        docType.PropertyGroups.Should().HaveCount(2);

        // Check first tab
        var basicInfoTab = docType.PropertyGroups.First(t => t.Alias == "basicInfo");
        basicInfoTab.Name.Should().Be("Basic Info");
        basicInfoTab.SortOrder.Should().Be(1);
        basicInfoTab.PropertyTypes.Should().HaveCount(3);

        // Check second tab
        var pricingTab = docType.PropertyGroups.First(t => t.Alias == "pricing");
        pricingTab.Name.Should().Be("Pricing");
        pricingTab.SortOrder.Should().Be(2);
        pricingTab.PropertyTypes.Should().HaveCount(2);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Create_Element_Type_For_Block_Grid()
    {
        // Act
        var elementType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("heroBlock")
            .WithName("Hero Block")
            .WithDescription("A hero banner block")
            .WithIcon("icon-picture")
            .IsElement(true)
            .AllowAtRoot(false)
            .AddTab("Content", tab => tab
                .AddTextBoxProperty("Headline", "headline", prop => prop.IsMandatory())
                .AddTextAreaProperty("Subheading", "subheading")
                .AddMediaPickerProperty("Background Image", "backgroundImage"))
            .Build();

        // Assert
        elementType.IsElement.Should().BeTrue();
        elementType.AllowedAsRoot.Should().BeFalse();
        elementType.Alias.Should().Be("heroBlock");
        elementType.Name.Should().Be("Hero Block");
        elementType.PropertyGroups.Should().HaveCount(1);
        elementType.PropertyGroups.First().PropertyTypes.Should().HaveCount(3);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Chain_All_Configuration_Methods()
    {
        // Arrange
        const string alias = "testPage";
        const string name = "Test Page";
        const string description = "A test page";
        const string icon = "icon-document";

        // Act
        var docType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias(alias)
            .WithName(name)
            .WithDescription(description)
            .WithIcon(icon)
            .AllowAtRoot(true)
            .IsElement(false)
            .AddTab("Tab1", tab => tab.AddTextBoxProperty("Prop1", "prop1"))
            .AddTab("Tab2", tab => tab.AddTextAreaProperty("Prop2", "prop2"))
            .Build();

        // Assert
        docType.Alias.Should().Be(alias);
        docType.Name.Should().Be(name);
        docType.Description.Should().Be(description);
        docType.Icon.Should().Be(icon);
        docType.AllowedAsRoot.Should().BeTrue();
        docType.IsElement.Should().BeFalse();
        docType.PropertyGroups.Should().HaveCount(2);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Return_Same_Instance_For_Method_Chaining()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act & Assert - Each method should return the same builder instance
        var result1 = builder.WithAlias("test");
        var result2 = result1.WithName("Test");
        var result3 = result2.WithDescription("Description");
        var result4 = result3.AddTab("Tab", tab => { });

        result1.Should().BeSameAs(builder);
        result2.Should().BeSameAs(builder);
        result3.Should().BeSameAs(builder);
        result4.Should().BeSameAs(builder);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Handle_Empty_Tab_Configuration()
    {
        // Act
        var docType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .AddTab("Empty Tab", tab => { }) // Empty tab configuration
            .Build();

        // Assert
        docType.PropertyGroups.Should().HaveCount(1);
        var tab = docType.PropertyGroups.First();
        tab.Name.Should().Be("Empty Tab");
        tab.PropertyTypes.Should().BeEmpty();
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Handle_Null_Values_Gracefully()
    {
        // Act
        var docType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("")
            .WithName("")
            .WithDescription("")
            .WithIcon("")
            .Build();

        // Assert
        docType.Alias.Should().Be("");
        docType.Name.Should().Be("");
        docType.Description.Should().Be("");
        docType.Icon.Should().Be("");
    }

    [Theory]
    [InlineData(true, false)]  // Element type, not allowed at root
    [InlineData(false, true)]  // Document type, allowed at root
    [InlineData(false, false)] // Document type, not allowed at root
    public void DocumentTypeBuilder_Should_Handle_Different_Type_Configurations(bool isElement, bool allowAtRoot)
    {
        // Act
        var docType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .IsElement(isElement)
            .AllowAtRoot(allowAtRoot)
            .Build();

        // Assert
        docType.IsElement.Should().Be(isElement);
        docType.AllowedAsRoot.Should().Be(allowAtRoot);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Support_Complex_Inheritance_Scenario()
    {
        // Arrange - Create a hierarchy of base types
        var baseContentType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("baseContent")
            .WithName("Base Content")
            .AddTab("Meta", tab => tab
                .AddTextBoxProperty("Meta Title", "metaTitle")
                .AddTextAreaProperty("Meta Description", "metaDescription"))
            .Build();

        var seoType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("seoMixin")
            .WithName("SEO Mixin")
            .AddTab("SEO", tab => tab
                .AddCheckboxProperty("Hide From Search", "hideFromSearch")
                .AddTextBoxProperty("Canonical URL", "canonicalUrl"))
            .Build();

        // Act - Create a type that inherits from both
        var complexType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("complexPage")
            .WithName("Complex Page")
            .AddComposition(baseContentType)
            .AddComposition(seoType)
            .AddTab("Content", tab => tab
                .AddTextBoxProperty("Page Title", "pageTitle")
                .AddRichTextProperty("Page Content", "pageContent"))
            .Build();

        // Assert
        complexType.ContentTypeComposition.Should().HaveCount(2);
        complexType.ContentTypeComposition.Should().Contain(baseContentType);
        complexType.ContentTypeComposition.Should().Contain(seoType);
        complexType.PropertyGroups.Should().HaveCount(1); // Only the tab we added directly
        complexType.PropertyGroups.First().Name.Should().Be("Content");
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Create_Real_World_Landing_Page_Example()
    {
        // Act - Create a realistic landing page document type
        var landingPage = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("landingPage")
            .WithName("Landing Page")
            .WithDescription("Marketing landing page with conversion tracking")
            .WithIcon("icon-trophy")
            .AllowAtRoot(true)
            .AddTab("Hero Section", tab => tab
                .WithAlias("heroSection")
                .WithSortOrder(1)
                .AddTextBoxProperty("Hero Title", "heroTitle", prop => prop
                    .IsMandatory()
                    .WithDescription("Main headline")
                    .WithValidationRegex("^.{1,60}$"))
                .AddTextAreaProperty("Hero Subtitle", "heroSubtitle")
                .AddMediaPickerProperty("Hero Background", "heroBackground")
                .AddTextBoxProperty("CTA Text", "ctaText", prop => prop.IsMandatory())
                .AddContentPickerProperty("CTA Link", "ctaLink"))
            .AddTab("Content", tab => tab
                .WithAlias("content")
                .WithSortOrder(2)
                .AddRichTextProperty("Main Content", "mainContent")
                .AddTextAreaProperty("Testimonial", "testimonial"))
            .AddTab("Tracking", tab => tab
                .WithAlias("tracking")
                .WithSortOrder(3)
                .AddTextBoxProperty("GA Tracking ID", "gaTrackingId")
                .AddCheckboxProperty("Enable Heat Mapping", "enableHeatMapping"))
            .Build();

        // Assert
        landingPage.Should().NotBeNull();
        landingPage.Alias.Should().Be("landingPage");
        landingPage.AllowedAsRoot.Should().BeTrue();
        landingPage.IsElement.Should().BeFalse();
        landingPage.PropertyGroups.Should().HaveCount(3);

        // Verify all tabs are present with correct sort order
        var heroTab = landingPage.PropertyGroups.First(t => t.Alias == "heroSection");
        var contentTab = landingPage.PropertyGroups.First(t => t.Alias == "content");
        var trackingTab = landingPage.PropertyGroups.First(t => t.Alias == "tracking");

        heroTab.SortOrder.Should().Be(1);
        contentTab.SortOrder.Should().Be(2);
        trackingTab.SortOrder.Should().Be(3);

        // Verify properties in hero section
        heroTab.PropertyTypes.Should().HaveCount(5);
        var heroTitle = heroTab.PropertyTypes!.First(p => p.Alias == "heroTitle");
        heroTitle.Mandatory.Should().BeTrue();
        heroTitle.ValidationRegExp.Should().Be("^.{1,60}$");
    }
}