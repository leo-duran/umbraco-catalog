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
            .Returns<string>(input => input?.ToLowerInvariant().Replace(" ", "") ?? string.Empty);
    }

    [Fact]
    public void Integration_Should_Create_Complete_Blog_Structure()
    {
        // Act - Create a complete blog structure with multiple document types
        var blogContainer = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("blog")
            .WithName("Blog")
            .WithDescription("Blog container for posts")
            .WithIcon("icon-newspaper-alt")
            .AllowAtRoot(true)
            .AddTab("Settings", tab => tab
                .WithAlias("settings")
                .AddTextBoxProperty("Blog Title", "blogTitle", prop => prop
                    .IsMandatory()
                    .WithDescription("Main title for the blog"))
                .AddTextAreaProperty("Blog Description", "blogDescription")
                .AddNumericProperty("Posts Per Page", "postsPerPage", prop => prop
                    .WithValueStorageType(ValueStorageType.Integer)))
            .Build();

        var blogPost = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("blogPost")
            .WithName("Blog Post")
            .WithDescription("Individual blog entry")
            .WithIcon("icon-edit")
            .AllowAtRoot(false)
            .AddTab("Content", tab => tab
                .WithAlias("content")
                .WithSortOrder(1)
                .AddTextBoxProperty("Post Title", "postTitle", prop => prop
                    .IsMandatory()
                    .WithDescription("The blog post title"))
                .AddTextAreaProperty("Summary", "summary", prop => prop
                    .IsMandatory()
                    .WithDescription("Brief summary for listings"))
                .AddRichTextProperty("Post Content", "postContent", prop => prop
                    .IsMandatory()
                    .WithDescription("The full post content"))
                .AddMediaPickerProperty("Featured Image", "featuredImage"))
            .AddTab("Metadata", tab => tab
                .WithAlias("metadata")
                .WithSortOrder(2)
                .AddDatePickerProperty("Publish Date", "publishDate", prop => prop
                    .IsMandatory())
                .AddTextBoxProperty("Author", "author")
                .AddCheckboxProperty("Featured Post", "featuredPost"))
            .Build();

        // Assert - Validate both document types
        blogContainer.Should().NotBeNull();
        blogContainer.Alias.Should().Be("blog");
        blogContainer.AllowedAsRoot.Should().BeTrue();
        blogContainer.PropertyGroups.Should().HaveCount(1);

        blogPost.Should().NotBeNull();
        blogPost.Alias.Should().Be("blogPost");
        blogPost.AllowedAsRoot.Should().BeFalse();
        blogPost.PropertyGroups.Should().HaveCount(2);

        // Validate complex property configuration
        var contentTab = blogPost.PropertyGroups.First(t => t.Alias == "content");
        contentTab.PropertyTypes.Should().HaveCount(4);
        
        var titleProperty = contentTab.PropertyTypes!.First(p => p.Alias == "postTitle");
        titleProperty.Mandatory.Should().BeTrue();
        titleProperty.PropertyEditorAlias.Should().Be("Umbraco.TextBox");
    }

    [Fact]
    public void Integration_Should_Create_E_Commerce_Product_With_Inheritance()
    {
        // Arrange - Create base product type
        var baseProduct = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("baseProduct")
            .WithName("Base Product")
            .WithDescription("Base properties for all products")
            .AddTab("Basic Info", tab => tab
                .WithAlias("basicInfo")
                .WithSortOrder(1)
                .AddTextBoxProperty("Product Name", "productName", prop => prop
                    .IsMandatory()
                    .WithDescription("The product name"))
                .AddTextAreaProperty("Short Description", "shortDescription", prop => prop
                    .IsMandatory())
                .AddMediaPickerProperty("Product Images", "productImages"))
            .AddTab("SEO", tab => tab
                .WithAlias("seo")
                .WithSortOrder(99)
                .AddTextBoxProperty("Meta Title", "metaTitle")
                .AddTextAreaProperty("Meta Description", "metaDescription"))
            .Build();

        // Act - Create specific product types that inherit from base
        var physicalProduct = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("physicalProduct")
            .WithName("Physical Product")
            .WithDescription("Product that requires shipping")
            .WithIcon("icon-box")
            .AddComposition(baseProduct)
            .AddTab("Shipping", tab => tab
                .WithAlias("shipping")
                .WithSortOrder(2)
                .AddNumericProperty("Weight", "weight", prop => prop
                    .WithValueStorageType(ValueStorageType.Decimal)
                    .WithDescription("Product weight in kg"))
                .AddNumericProperty("Length", "length", prop => prop
                    .WithValueStorageType(ValueStorageType.Decimal))
                .AddNumericProperty("Width", "width", prop => prop
                    .WithValueStorageType(ValueStorageType.Decimal))
                .AddNumericProperty("Height", "height", prop => prop
                    .WithValueStorageType(ValueStorageType.Decimal)))
            .AddTab("Pricing", tab => tab
                .WithAlias("pricing")
                .WithSortOrder(3)
                .AddNumericProperty("Price", "price", prop => prop
                    .IsMandatory()
                    .WithValueStorageType(ValueStorageType.Decimal))
                .AddNumericProperty("Sale Price", "salePrice", prop => prop
                    .WithValueStorageType(ValueStorageType.Decimal))
                .AddCheckboxProperty("On Sale", "onSale"))
            .Build();

        var digitalProduct = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("digitalProduct")
            .WithName("Digital Product")
            .WithDescription("Downloadable digital product")
            .WithIcon("icon-download")
            .AddComposition(baseProduct)
            .AddTab("Digital Content", tab => tab
                .WithAlias("digitalContent")
                .WithSortOrder(2)
                .AddMediaPickerProperty("Download File", "downloadFile", prop => prop
                    .IsMandatory()
                    .WithDescription("The file to download"))
                .AddNumericProperty("File Size", "fileSize", prop => prop
                    .WithValueStorageType(ValueStorageType.Integer)
                    .WithDescription("File size in MB"))
                .AddTextBoxProperty("License Type", "licenseType"))
            .AddTab("Pricing", tab => tab
                .WithAlias("pricing")
                .WithSortOrder(3)
                .AddNumericProperty("Price", "price", prop => prop
                    .IsMandatory()
                    .WithValueStorageType(ValueStorageType.Decimal))
                .AddCheckboxProperty("Limited Time Offer", "limitedTimeOffer"))
            .Build();

        // Assert - Validate inheritance and composition
        baseProduct.Should().NotBeNull();
        baseProduct.PropertyGroups.Should().HaveCount(2);

        physicalProduct.Should().NotBeNull();
        physicalProduct.ContentTypeComposition.Should().HaveCount(1);
        physicalProduct.ContentTypeComposition.Should().Contain(baseProduct);
        physicalProduct.PropertyGroups.Should().HaveCount(2); // Only tabs added directly

        digitalProduct.Should().NotBeNull();
        digitalProduct.ContentTypeComposition.Should().HaveCount(1);
        digitalProduct.ContentTypeComposition.Should().Contain(baseProduct);
        digitalProduct.PropertyGroups.Should().HaveCount(2); // Only tabs added directly

        // Verify specific properties
        var shippingTab = physicalProduct.PropertyGroups.First(t => t.Alias == "shipping");
        shippingTab.PropertyTypes.Should().HaveCount(4);
        
        var digitalTab = digitalProduct.PropertyGroups.First(t => t.Alias == "digitalContent");
        digitalTab.PropertyTypes.Should().HaveCount(3);
    }

    [Fact]
    public void Integration_Should_Create_Block_Grid_Element_Types()
    {
        // Act - Create multiple element types for a Block Grid setup
        var heroBlock = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("heroBlock")
            .WithName("Hero Block")
            .WithDescription("Large promotional banner")
            .WithIcon("icon-picture")
            .IsElement(true)
            .AddTab("Content", tab => tab
                .WithAlias("content")
                .AddTextBoxProperty("Headline", "headline", prop => prop
                    .IsMandatory()
                    .WithDescription("Main headline text")
                    .WithValidationRegex("^.{5,100}$"))
                .AddTextAreaProperty("Subheading", "subheading", prop => prop
                    .WithDescription("Supporting text"))
                .AddMediaPickerProperty("Background Image", "backgroundImage", prop => prop
                    .WithDescription("Hero background image"))
                .AddTextBoxProperty("Button Text", "buttonText")
                .AddContentPickerProperty("Button Link", "buttonLink"))
            .Build();

        var textBlock = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("textBlock")
            .WithName("Text Block")
            .WithDescription("Simple text content block")
            .WithIcon("icon-document")
            .IsElement(true)
            .AddTab("Content", tab => tab
                .WithAlias("content")
                .AddTextBoxProperty("Heading", "heading", prop => prop
                    .WithDescription("Optional section heading"))
                .AddRichTextProperty("Text Content", "textContent", prop => prop
                    .IsMandatory()
                    .WithDescription("The main text content")))
            .Build();

        var imageBlock = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("imageBlock")
            .WithName("Image Block")
            .WithDescription("Image with optional caption")
            .WithIcon("icon-picture")
            .IsElement(true)
            .AddTab("Content", tab => tab
                .WithAlias("content")
                .AddMediaPickerProperty("Image", "image", prop => prop
                    .IsMandatory()
                    .WithDescription("The image to display"))
                .AddTextBoxProperty("Caption", "caption", prop => prop
                    .WithDescription("Optional image caption"))
                .AddTextBoxProperty("Alt Text", "altText", prop => prop
                    .IsMandatory()
                    .WithDescription("Alternative text for accessibility")))
            .Build();

        // Assert - Validate all element types
        var elementTypes = new[] { heroBlock, textBlock, imageBlock };
        
        foreach (var elementType in elementTypes)
        {
            elementType.Should().NotBeNull();
            elementType.IsElement.Should().BeTrue();
            elementType.AllowedAsRoot.Should().BeFalse();
            elementType.PropertyGroups.Should().HaveCount(1);
            elementType.PropertyGroups.First().Alias.Should().Be("content");
        }

        // Verify specific element properties
        heroBlock.PropertyGroups.First().PropertyTypes.Should().HaveCount(5);
        textBlock.PropertyGroups.First().PropertyTypes.Should().HaveCount(2);
        imageBlock.PropertyGroups.First().PropertyTypes.Should().HaveCount(3);

        // Verify mandatory properties
        var heroHeadline = heroBlock.PropertyGroups.First().PropertyTypes!.First(p => p.Alias == "headline");
        heroHeadline.Mandatory.Should().BeTrue();
        heroHeadline.ValidationRegExp.Should().Be("^.{5,100}$");

        var imageProperty = imageBlock.PropertyGroups.First().PropertyTypes!.First(p => p.Alias == "image");
        imageProperty.Mandatory.Should().BeTrue();
        imageProperty.PropertyEditorAlias.Should().Be("Umbraco.MediaPicker3");
    }

    [Fact]
    public void Integration_Should_Create_Complex_Landing_Page_With_All_Features()
    {
        // Act - Create a comprehensive landing page using all builder features
        var landingPage = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("marketingLandingPage")
            .WithName("Marketing Landing Page")
            .WithDescription("High-conversion landing page with tracking")
            .WithIcon("icon-trophy")
            .AllowAtRoot(true)
            .IsElement(false)
            .AddTab("Hero Section", tab => tab
                .WithAlias("heroSection")
                .WithSortOrder(1)
                .AddTextBoxProperty("Hero Title", "heroTitle", prop => prop
                    .IsMandatory()
                    .WithDescription("Main headline - keep under 60 chars")
                    .WithSortOrder(1)
                    .WithValidationRegex("^.{1,60}$")
                    .WithLabelOnTop())
                .AddTextAreaProperty("Hero Subtitle", "heroSubtitle", prop => prop
                    .WithDescription("Supporting text under headline")
                    .WithSortOrder(2))
                .AddMediaPickerProperty("Hero Background", "heroBackground", prop => prop
                    .WithDescription("Background image or video")
                    .WithSortOrder(3))
                .AddTextBoxProperty("Primary CTA Text", "primaryCtaText", prop => prop
                    .IsMandatory()
                    .WithDescription("Main call-to-action button")
                    .WithSortOrder(4))
                .AddContentPickerProperty("Primary CTA Link", "primaryCtaLink", prop => prop
                    .WithDescription("Page to link to")
                    .WithSortOrder(5)))
            .AddTab("Content Sections", tab => tab
                .WithAlias("contentSections")
                .WithSortOrder(2)
                .AddRichTextProperty("Main Content", "mainContent", prop => prop
                    .WithDescription("Primary content area"))
                .AddTextAreaProperty("Value Proposition", "valueProposition", prop => prop
                    .WithDescription("Key benefits statement"))
                .AddMediaPickerProperty("Feature Images", "featureImages", prop => prop
                    .WithDescription("Images showcasing features"))
                .AddTextAreaProperty("Customer Testimonial", "testimonial", prop => prop
                    .WithDescription("Customer quote or review"))
                .AddTextBoxProperty("Testimonial Author", "testimonialAuthor"))
            .AddTab("Conversion Tracking", tab => tab
                .WithAlias("tracking")
                .WithSortOrder(3)
                .AddTextBoxProperty("Google Analytics ID", "gaId", prop => prop
                    .WithDescription("GA4 measurement ID"))
                .AddTextBoxProperty("Facebook Pixel ID", "fbPixelId", prop => prop
                    .WithDescription("Facebook conversion tracking"))
                .AddTextBoxProperty("Google Ads Conversion ID", "googleAdsId")
                .AddCheckboxProperty("Enable Heat Mapping", "enableHeatMap", prop => prop
                    .WithDescription("Track user mouse movements"))
                .AddCheckboxProperty("Enable A/B Testing", "enableAbTest"))
            .AddTab("Campaign Settings", tab => tab
                .WithAlias("campaignSettings")
                .WithSortOrder(4)
                .AddDatePickerProperty("Campaign Start Date", "campaignStart", prop => prop
                    .IsMandatory()
                    .WithDescription("When campaign goes live"))
                .AddDatePickerProperty("Campaign End Date", "campaignEnd", prop => prop
                    .WithDescription("When to disable page"))
                .AddTextBoxProperty("Campaign Code", "campaignCode", prop => prop
                    .WithDescription("Internal tracking code"))
                .AddNumericProperty("A/B Test Variant", "abTestVariant", prop => prop
                    .WithValueStorageType(ValueStorageType.Integer)
                    .WithDescription("Version number for testing"))
                .AddCheckboxProperty("Show Navigation", "showNavigation", prop => prop
                    .WithDescription("Include site navigation"))
                .AddCheckboxProperty("Show Footer", "showFooter"))
            .Build();

        // Assert - Comprehensive validation
        landingPage.Should().NotBeNull();
        landingPage.Alias.Should().Be("marketingLandingPage");
        landingPage.Name.Should().Be("Marketing Landing Page");
        landingPage.AllowedAsRoot.Should().BeTrue();
        landingPage.IsElement.Should().BeFalse();
        landingPage.PropertyGroups.Should().HaveCount(4);

        // Validate tab structure and ordering
        var tabs = landingPage.PropertyGroups.OrderBy(t => t.SortOrder).ToList();
        tabs[0].Alias.Should().Be("heroSection");
        tabs[0].SortOrder.Should().Be(1);
        tabs[1].Alias.Should().Be("contentSections");
        tabs[2].Alias.Should().Be("tracking");
        tabs[3].Alias.Should().Be("campaignSettings");

        // Validate hero section properties
        var heroTab = tabs[0];
        heroTab.PropertyTypes.Should().HaveCount(5);
        var heroTitle = heroTab.PropertyTypes!.First(p => p.Alias == "heroTitle");
        heroTitle.Mandatory.Should().BeTrue();
        heroTitle.ValidationRegExp.Should().Be("^.{1,60}$");
        heroTitle.LabelOnTop.Should().BeTrue();
        heroTitle.SortOrder.Should().Be(1);

        // Validate content sections
        var contentTab = tabs[1];
        contentTab.PropertyTypes.Should().HaveCount(5);

        // Validate tracking tab
        var trackingTab = tabs[2];
        trackingTab.PropertyTypes.Should().HaveCount(5);
        var heatMapProperty = trackingTab.PropertyTypes!.First(p => p.Alias == "enableHeatMap");
        heatMapProperty.PropertyEditorAlias.Should().Be("Umbraco.TrueFalse");

        // Validate campaign settings
        var campaignTab = tabs[3];
        campaignTab.PropertyTypes.Should().HaveCount(6);
        var startDate = campaignTab.PropertyTypes!.First(p => p.Alias == "campaignStart");
        startDate.Mandatory.Should().BeTrue();
        startDate.PropertyEditorAlias.Should().Be("Umbraco.DateTime");
    }

    [Fact]
    public void Integration_Should_Handle_Multiple_Document_Types_With_Different_Configurations()
    {
        // Act - Create various document types with different configurations
        var homePage = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("homePage")
            .WithName("Home Page")
            .AllowAtRoot(true)
            .IsElement(false)
            .Build();

        var elementType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("contentBlock")
            .WithName("Content Block")
            .IsElement(true)
            .AllowAtRoot(false)
            .Build();

        var restrictedPage = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("restrictedPage")
            .WithName("Restricted Page")
            .AllowAtRoot(false)
            .IsElement(false)
            .Build();

        // Assert - Validate different configurations work correctly
        homePage.AllowedAsRoot.Should().BeTrue();
        homePage.IsElement.Should().BeFalse();

        elementType.IsElement.Should().BeTrue();
        elementType.AllowedAsRoot.Should().BeFalse();

        restrictedPage.AllowedAsRoot.Should().BeFalse();
        restrictedPage.IsElement.Should().BeFalse();

        // All should be valid ContentType instances
        var allTypes = new ContentType[] { homePage, elementType, restrictedPage };
        allTypes.Should().AllBeOfType<ContentType>();
        allTypes.Should().AllSatisfy(ct => ct.Should().NotBeNull());
    }

    [Fact]
    public void Integration_Should_Create_Document_Types_With_All_Property_Editor_Types()
    {
        // Act - Create a document type that uses every supported property editor
        var comprehensiveType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("comprehensiveType")
            .WithName("Comprehensive Type")
            .WithDescription("Uses all supported property editors")
            .AddTab("Text Properties", tab => tab
                .WithAlias("textProperties")
                .WithSortOrder(1)
                .AddTextBoxProperty("Text Box", "textBox")
                .AddTextAreaProperty("Text Area", "textArea")
                .AddRichTextProperty("Rich Text", "richText"))
            .AddTab("Media & Content", tab => tab
                .WithAlias("mediaContent")
                .WithSortOrder(2)
                .AddMediaPickerProperty("Media Picker", "mediaPicker")
                .AddContentPickerProperty("Content Picker", "contentPicker"))
            .AddTab("Data Input", tab => tab
                .WithAlias("dataInput")
                .WithSortOrder(3)
                .AddNumericProperty("Numeric", "numeric")
                .AddCheckboxProperty("Checkbox", "checkbox")
                .AddDatePickerProperty("Date Picker", "datePicker"))
            .AddTab("Custom Properties", tab => tab
                .WithAlias("customProperties")
                .WithSortOrder(4)
                .AddProperty("Custom Editor 1", "custom1", "Custom.Editor.One", prop => prop
                    .WithDescription("Custom property editor"))
                .AddProperty("Custom Editor 2", "custom2", "Custom.Editor.Two", prop => prop
                    .IsMandatory()
                    .WithValueStorageType(ValueStorageType.Ntext)))
            .Build();

        // Assert - Validate all property types are present and correctly configured
        comprehensiveType.Should().NotBeNull();
        comprehensiveType.PropertyGroups.Should().HaveCount(4);

        // Check text properties tab
        var textTab = comprehensiveType.PropertyGroups.First(t => t.Alias == "textProperties");
        textTab.PropertyTypes.Should().HaveCount(3);
        var editors = textTab.PropertyTypes!.Select(p => p.PropertyEditorAlias).ToList();
        editors.Should().Contain("Umbraco.TextBox");
        editors.Should().Contain("Umbraco.TextArea");
        editors.Should().Contain("Umbraco.TinyMCE");

        // Check media & content tab
        var mediaTab = comprehensiveType.PropertyGroups.First(t => t.Alias == "mediaContent");
        mediaTab.PropertyTypes.Should().HaveCount(2);
        var mediaEditors = mediaTab.PropertyTypes!.Select(p => p.PropertyEditorAlias).ToList();
        mediaEditors.Should().Contain("Umbraco.MediaPicker3");
        mediaEditors.Should().Contain("Umbraco.ContentPicker");

        // Check data input tab
        var dataTab = comprehensiveType.PropertyGroups.First(t => t.Alias == "dataInput");
        dataTab.PropertyTypes.Should().HaveCount(3);
        var dataEditors = dataTab.PropertyTypes!.Select(p => p.PropertyEditorAlias).ToList();
        dataEditors.Should().Contain("Umbraco.Integer");
        dataEditors.Should().Contain("Umbraco.TrueFalse");
        dataEditors.Should().Contain("Umbraco.DateTime");

        // Check custom properties tab
        var customTab = comprehensiveType.PropertyGroups.First(t => t.Alias == "customProperties");
        customTab.PropertyTypes.Should().HaveCount(2);
        var customEditors = customTab.PropertyTypes!.Select(p => p.PropertyEditorAlias).ToList();
        customEditors.Should().Contain("Custom.Editor.One");
        customEditors.Should().Contain("Custom.Editor.Two");

        // Verify custom property configuration
        var custom2 = customTab.PropertyTypes!.First(p => p.Alias == "custom2");
        custom2.Mandatory.Should().BeTrue();
        custom2.ValueStorageType.Should().Be(ValueStorageType.Ntext);
    }
}