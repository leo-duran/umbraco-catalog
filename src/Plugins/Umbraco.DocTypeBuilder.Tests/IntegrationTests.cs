using System.Linq;
using FluentAssertions;
using Moq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;
using Umbraco.DocTypeBuilder;
using Xunit;

namespace Umbraco.DocTypeBuilder.Tests;

/// <summary>
/// Integration tests that validate all components working together
/// Tests real-world scenarios and component interactions
/// </summary>
public class IntegrationTests
{
    private readonly Mock<IShortStringHelper> _mockShortStringHelper;

    public IntegrationTests()
    {
        _mockShortStringHelper = new Mock<IShortStringHelper>();
        // Setup realistic mock behavior
        _mockShortStringHelper
            .Setup(x => x.CleanStringForSafeAlias(It.IsAny<string>()))
            .Returns<string>(input => input?.ToLowerInvariant().Replace(" ", "").Replace("-", "") ?? string.Empty);
    }

    [Fact]
    public void Integration_Should_Create_Complete_Blog_Post_Document_Type()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act - Create a realistic blog post document type
        var blogPost = builder
            .SetAlias("blogPost")
            .SetName("Blog Post")
            .SetDescription("A blog post with content, media, and metadata")
            .SetIcon("icon-edit")
            .AddTab("Content", tab => tab
                .AddTextBoxProperty("title", "Title", prop => prop
                    .SetMandatory(true)
                    .SetDescription("The blog post title"))
                .AddTextAreaProperty("excerpt", "Excerpt", prop => prop
                    .SetDescription("Short summary of the blog post"))
                .AddRichTextProperty("content", "Content", prop => prop
                    .SetMandatory(true)
                    .SetDescription("Main blog post content")))
            .AddTab("Media", tab => tab
                .AddMediaPickerProperty("featuredImage", "Featured Image", prop => prop
                    .SetDescription("Main image for the blog post")))
            .AddTab("Settings", tab => tab
                .AddDatePickerProperty("publishDate", "Publish Date", prop => prop
                    .SetDescription("When to publish this post"))
                .AddCheckboxProperty("featured", "Featured", prop => prop
                    .SetDescription("Mark as featured post"))
                .AddIntegerProperty("readTime", "Read Time (minutes)", prop => prop
                    .SetDescription("Estimated reading time")))
                .Build();

        // Assert
        blogPost.Should().NotBeNull();
        blogPost.Alias.Should().Be("blogPost");
        blogPost.Name.Should().Be("Blog Post");
        blogPost.PropertyGroups.Should().HaveCount(3);

        // Verify Content tab
        var contentTab = blogPost.PropertyGroups.First(g => g.Name == "Content");
        contentTab.PropertyTypes.Should().HaveCount(3);
        contentTab.PropertyTypes.Should().Contain(p => p.Alias == "title" && p.Mandatory);
        contentTab.PropertyTypes.Should().Contain(p => p.Alias == "excerpt");
        contentTab.PropertyTypes.Should().Contain(p => p.Alias == "content" && p.Mandatory);

        // Verify Media tab
        var mediaTab = blogPost.PropertyGroups.First(g => g.Name == "Media");
        mediaTab.PropertyTypes.Should().HaveCount(1);
        mediaTab.PropertyTypes.Should().Contain(p => p.Alias == "featuredImage");

        // Verify Settings tab
        var settingsTab = blogPost.PropertyGroups.First(g => g.Name == "Settings");
        settingsTab.PropertyTypes.Should().HaveCount(3);
        settingsTab.PropertyTypes.Should().Contain(p => p.Alias == "publishDate");
        settingsTab.PropertyTypes.Should().Contain(p => p.Alias == "featured");
        settingsTab.PropertyTypes.Should().Contain(p => p.Alias == "readTime");
    }

    [Fact]
    public void Integration_Should_Create_E_Commerce_Product_Document_Type()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act - Create a realistic e-commerce product document type
        var product = builder
            .SetAlias("product")
            .SetName("Product")
            .SetDescription("E-commerce product with pricing and inventory")
            .SetIcon("icon-shopping-basket")
            .AddTab("Product Info", tab => tab
                .AddTextBoxProperty("productName", "Product Name", prop => prop
                    .SetMandatory(true))
                .AddTextAreaProperty("shortDescription", "Short Description", prop => prop
                    .SetMandatory(true))
                .AddRichTextProperty("fullDescription", "Full Description")
                .AddTextBoxProperty("sku", "SKU", prop => prop
                    .SetMandatory(true)
                    .SetDescription("Stock Keeping Unit")))
            .AddTab("Pricing", tab => tab
                .AddIntegerProperty("price", "Price (cents)", prop => prop
                    .SetMandatory(true)
                    .SetDescription("Price in cents"))
                .AddIntegerProperty("comparePrice", "Compare Price (cents)", prop => prop
                    .SetDescription("Original price for discount display"))
                .AddCheckboxProperty("onSale", "On Sale"))
            .AddTab("Inventory", tab => tab
                .AddIntegerProperty("stockQuantity", "Stock Quantity", prop => prop
                    .SetMandatory(true))
                .AddCheckboxProperty("trackInventory", "Track Inventory")
                .AddCheckboxProperty("allowBackorders", "Allow Backorders"))
            .AddTab("Media", tab => tab
                .AddMediaPickerProperty("productImages", "Product Images", prop => prop
                    .SetDescription("Product photo gallery")))
            .Build();

        // Assert
        product.Should().NotBeNull();
        product.Alias.Should().Be("product");
        product.Name.Should().Be("Product");
        product.PropertyGroups.Should().HaveCount(4);

        // Verify all tabs have expected properties
        var productInfoTab = product.PropertyGroups.First(g => g.Name == "Product Info");
        productInfoTab.PropertyTypes.Should().HaveCount(4);

        var pricingTab = product.PropertyGroups.First(g => g.Name == "Pricing");
        pricingTab.PropertyTypes.Should().HaveCount(3);

        var inventoryTab = product.PropertyGroups.First(g => g.Name == "Inventory");
        inventoryTab.PropertyTypes.Should().HaveCount(3);

        var mediaTab = product.PropertyGroups.First(g => g.Name == "Media");
        mediaTab.PropertyTypes.Should().HaveCount(1);
    }

    [Fact]
    public void Integration_Should_Create_Landing_Page_Document_Type()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act - Create a marketing landing page document type
        var landingPage = builder
            .SetAlias("landingPage")
            .SetName("Landing Page")
            .SetDescription("Marketing landing page with hero section and CTA")
            .SetIcon("icon-layout")
            .SetAllowedAtRoot(true)
            .AddTab("Hero Section", tab => tab
                .AddTextBoxProperty("heroTitle", "Hero Title", prop => prop
                    .SetMandatory(true))
                .AddTextAreaProperty("heroSubtitle", "Hero Subtitle")
                .AddMediaPickerProperty("heroBackground", "Hero Background Image")
                .AddTextBoxProperty("ctaText", "CTA Button Text", prop => prop
                    .SetMandatory(true))
                .AddTextBoxProperty("ctaLink", "CTA Link", prop => prop
                    .SetMandatory(true)))
            .AddTab("Content", tab => tab
                .AddRichTextProperty("mainContent", "Main Content")
                .AddRichTextProperty("features", "Features Section")
                .AddRichTextProperty("testimonials", "Testimonials"))
            .AddTab("SEO", tab => tab
                .AddTextBoxProperty("metaTitle", "Meta Title")
                .AddTextAreaProperty("metaDescription", "Meta Description")
                .AddTextBoxProperty("canonicalUrl", "Canonical URL"))
            .Build();

        // Assert
        landingPage.Should().NotBeNull();
        landingPage.Alias.Should().Be("landingPage");
        landingPage.Name.Should().Be("Landing Page");
        landingPage.AllowedAsRoot.Should().BeTrue();
        landingPage.PropertyGroups.Should().HaveCount(3);

        // Verify Hero Section
        var heroTab = landingPage.PropertyGroups.First(g => g.Name == "Hero Section");
        heroTab.PropertyTypes.Should().HaveCount(5);
        heroTab.PropertyTypes.Should().Contain(p => p.Alias == "heroTitle" && p.Mandatory);
        heroTab.PropertyTypes.Should().Contain(p => p.Alias == "ctaText" && p.Mandatory);

        // Verify Content tab
        var contentTab = landingPage.PropertyGroups.First(g => g.Name == "Content");
        contentTab.PropertyTypes.Should().HaveCount(3);

        // Verify SEO tab
        var seoTab = landingPage.PropertyGroups.First(g => g.Name == "SEO");
        seoTab.PropertyTypes.Should().HaveCount(3);
    }

    [Fact]
    public void Integration_Should_Create_Block_Grid_Element_Types()
    {
        // Arrange
        var textBlockBuilder = new DocumentTypeBuilder(_mockShortStringHelper.Object);
        var imageBlockBuilder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act - Create element types for Block Grid
        var textBlock = textBlockBuilder
            .SetAlias("textBlock")
            .SetName("Text Block")
            .SetDescription("Simple text block for Block Grid")
            .SetIcon("icon-edit")
            .SetIsElement(true)
            .AddTab("Content", tab => tab
                .AddTextBoxProperty("headline", "Headline")
                .AddRichTextProperty("text", "Text", prop => prop
                    .SetMandatory(true)))
            .Build();

        var imageBlock = imageBlockBuilder
            .SetAlias("imageBlock")
            .SetName("Image Block")
            .SetDescription("Image block for Block Grid")
            .SetIcon("icon-picture")
            .SetIsElement(true)
            .AddTab("Content", tab => tab
                .AddMediaPickerProperty("image", "Image", prop => prop
                    .SetMandatory(true))
                .AddTextBoxProperty("altText", "Alt Text", prop => prop
                    .SetMandatory(true))
                .AddTextAreaProperty("caption", "Caption"))
            .Build();

        // Assert
        textBlock.Should().NotBeNull();
        textBlock.IsElement.Should().BeTrue();
        textBlock.AllowedAsRoot.Should().BeFalse(); // Element types shouldn't be at root
        textBlock.PropertyGroups.Should().HaveCount(1);

        imageBlock.Should().NotBeNull();
        imageBlock.IsElement.Should().BeTrue();
        imageBlock.AllowedAsRoot.Should().BeFalse();
        imageBlock.PropertyGroups.Should().HaveCount(1);

        // Verify text block properties
        var textBlockTab = textBlock.PropertyGroups.First();
        textBlockTab.PropertyTypes.Should().HaveCount(2);
        textBlockTab.PropertyTypes.Should().Contain(p => p.Alias == "text" && p.Mandatory);

        // Verify image block properties
        var imageBlockTab = imageBlock.PropertyGroups.First();
        imageBlockTab.PropertyTypes.Should().HaveCount(3);
        imageBlockTab.PropertyTypes.Should().Contain(p => p.Alias == "image" && p.Mandatory);
        imageBlockTab.PropertyTypes.Should().Contain(p => p.Alias == "altText" && p.Mandatory);
    }

    [Fact]
    public void Integration_Should_Create_Complex_Multi_Tab_Document_Type()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act - Create a complex document type with many tabs and property types
        var complexType = builder
            .SetAlias("complexPage")
            .SetName("Complex Page")
            .SetDescription("A complex page demonstrating all property types")
            .SetIcon("icon-settings")
            .AddTab("Text Properties", tab => tab
                .AddTextBoxProperty("singleLineText", "Single Line Text", prop => prop
                    .SetMandatory(true)
                    .SetDescription("Required text field"))
                .AddTextAreaProperty("multiLineText", "Multi Line Text", prop => prop
                    .SetDescription("Large text area"))
                .AddRichTextProperty("richText", "Rich Text", prop => prop
                    .SetLabelOnTop(true)))
            .AddTab("Media Properties", tab => tab
                .AddMediaPickerProperty("singleImage", "Single Image")
                .AddMediaPickerProperty("imageGallery", "Image Gallery"))
            .AddTab("Picker Properties", tab => tab
                .AddContentPickerProperty("relatedPage", "Related Page")
                .AddContentPickerProperty("multiplePages", "Multiple Pages"))
            .AddTab("Data Properties", tab => tab
                .AddIntegerProperty("numberValue", "Number Value", prop => prop
                    .SetSortOrder(1))
                .AddCheckboxProperty("booleanValue", "Boolean Value", prop => prop
                    .SetSortOrder(2))
                .AddDatePickerProperty("dateValue", "Date Value", prop => prop
                    .SetSortOrder(3)))
            .Build();

        // Assert
        complexType.Should().NotBeNull();
        complexType.PropertyGroups.Should().HaveCount(4);

        // Verify each tab has the expected number of properties
        var textTab = complexType.PropertyGroups.First(g => g.Name == "Text Properties");
        textTab.PropertyTypes.Should().HaveCount(3);

        var mediaTab = complexType.PropertyGroups.First(g => g.Name == "Media Properties");
        mediaTab.PropertyTypes.Should().HaveCount(2);

        var pickerTab = complexType.PropertyGroups.First(g => g.Name == "Picker Properties");
        pickerTab.PropertyTypes.Should().HaveCount(2);

        var dataTab = complexType.PropertyGroups.First(g => g.Name == "Data Properties");
        dataTab.PropertyTypes.Should().HaveCount(3);

        // Verify property configurations
        var mandatoryProperty = textTab.PropertyTypes.First(p => p.Alias == "singleLineText");
        mandatoryProperty.Mandatory.Should().BeTrue();

        var labelOnTopProperty = textTab.PropertyTypes.First(p => p.Alias == "richText");
        labelOnTopProperty.LabelOnTop.Should().BeTrue();

        var sortedProperties = dataTab.PropertyTypes.OrderBy(p => p.SortOrder).ToList();
        sortedProperties[0].Alias.Should().Be("numberValue");
        sortedProperties[1].Alias.Should().Be("booleanValue");
        sortedProperties[2].Alias.Should().Be("dateValue");
    }

    [Fact]
    public void Integration_Should_Demonstrate_Builder_Pattern_Flexibility()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act - Demonstrate that builders can be configured step by step
        var stepByStepBuilder = builder
            .SetAlias("stepByStep")
            .SetName("Step By Step");

        // Add configuration conditionally
        stepByStepBuilder.SetDescription("Built step by step");
        stepByStepBuilder.SetIcon("icon-cog");

        // Add tabs conditionally
        stepByStepBuilder.AddTab("Basic Info", tab => tab
            .AddTextBoxProperty("title", "Title"));

        // Build the final result
        var result = stepByStepBuilder.Build();

        // Assert
        result.Should().NotBeNull();
        result.Alias.Should().Be("stepByStep");
        result.Name.Should().Be("Step By Step");
        result.Description.Should().Be("Built step by step");
        result.Icon.Should().Be("icon-cog");
        result.PropertyGroups.Should().HaveCount(1);
    }
}