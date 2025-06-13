using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.DocTypeBuilder;

namespace Umbraco.DocTypeBuilder.Examples;

/// <summary>
/// Practical examples showing how to use the DocTypeBuilder in real Umbraco scenarios.
/// These examples demonstrate common document type patterns and best practices.
/// </summary>
public class DocTypeBuilderExamples
{
    private readonly IContentTypeService _contentTypeService;
    private readonly IShortStringHelper _shortStringHelper;

    public DocTypeBuilderExamples(IContentTypeService contentTypeService, IShortStringHelper shortStringHelper)
    {
        _contentTypeService = contentTypeService;
        _shortStringHelper = shortStringHelper;
    }

    #region Basic Examples

    /// <summary>
    /// Example 1: Creating a simple home page document type
    /// This demonstrates the most basic usage of the DocTypeBuilder
    /// </summary>
    public void CreateHomePage()
    {
        var homePage = new DocumentTypeBuilder(_shortStringHelper)
            .WithAlias("homePage")                           // URL-friendly identifier
            .WithName("Home Page")                           // Human-readable name
            .WithDescription("The main landing page")        // Help text for editors
            .WithIcon("icon-home")                          // Icon in Umbraco backoffice
            .AllowAtRoot(true)                              // Can be created at root level
            .AddTab("Content", tab => tab                   // Create a content tab
                .WithAlias("content")
                .WithSortOrder(1)
                .AddTextBoxProperty("Page Title", "pageTitle", prop => prop
                    .IsMandatory()                          // Required field
                    .WithDescription("The main title displayed on the page"))
                .AddRichTextProperty("Main Content", "mainContent", prop => prop
                    .WithDescription("The main content area"))
                .AddMediaPickerProperty("Hero Image", "heroImage", prop => prop
                    .WithDescription("Large image displayed at the top")))
            .Build();

        // Save the document type to Umbraco
        _contentTypeService.Save(homePage);
    }

    /// <summary>
    /// Example 2: Creating a news article document type
    /// Shows more advanced property configuration
    /// </summary>
    public void CreateNewsArticle()
    {
        var newsArticle = new DocumentTypeBuilder(_shortStringHelper)
            .WithAlias("newsArticle")
            .WithName("News Article")
            .WithDescription("Individual news story")
            .WithIcon("icon-newspaper")
            .AllowAtRoot(false)                             // Cannot be created at root
            .AddTab("Article Content", tab => tab
                .WithAlias("articleContent")
                .WithSortOrder(1)
                .AddTextBoxProperty("Headline", "headline", prop => prop
                    .IsMandatory()
                    .WithDescription("The article headline")
                    .WithSortOrder(1)
                    .WithValidationRegex("^.{10,100}$"))    // Between 10-100 characters
                .AddTextAreaProperty("Summary", "summary", prop => prop
                    .IsMandatory()
                    .WithDescription("Brief summary for listings")
                    .WithSortOrder(2))
                .AddRichTextProperty("Article Body", "articleBody", prop => prop
                    .IsMandatory()
                    .WithDescription("The full article content")
                    .WithSortOrder(3))
                .AddMediaPickerProperty("Featured Image", "featuredImage", prop => prop
                    .WithDescription("Main image for the article")
                    .WithSortOrder(4)))
            .AddTab("Metadata", tab => tab
                .WithAlias("metadata")
                .WithSortOrder(2)
                .AddDatePickerProperty("Publish Date", "publishDate", prop => prop
                    .IsMandatory()
                    .WithDescription("When this article should be published"))
                .AddTextBoxProperty("Author", "author", prop => prop
                    .WithDescription("Article author name"))
                .AddCheckboxProperty("Featured Article", "featuredArticle", prop => prop
                    .WithDescription("Mark as featured on homepage")))
            .Build();

        _contentTypeService.Save(newsArticle);
    }

    #endregion

    #region E-commerce Examples

    /// <summary>
    /// Example 3: Creating a product document type for e-commerce
    /// Demonstrates complex multi-tab structure
    /// </summary>
    public void CreateProduct()
    {
        var product = new DocumentTypeBuilder(_shortStringHelper)
            .WithAlias("product")
            .WithName("Product")
            .WithDescription("E-commerce product")
            .WithIcon("icon-shopping-basket")
            .AllowAtRoot(false)
            .AddTab("Basic Information", tab => tab
                .WithAlias("basicInfo")
                .WithSortOrder(1)
                .AddTextBoxProperty("Product Name", "productName", prop => prop
                    .IsMandatory()
                    .WithDescription("The product name"))
                .AddTextAreaProperty("Short Description", "shortDescription", prop => prop
                    .IsMandatory()
                    .WithDescription("Brief description for product listings"))
                .AddRichTextProperty("Full Description", "fullDescription", prop => prop
                    .WithDescription("Detailed product description"))
                .AddMediaPickerProperty("Product Images", "productImages", prop => prop
                    .WithDescription("Product photos")))
            .AddTab("Pricing & Inventory", tab => tab
                .WithAlias("pricing")
                .WithSortOrder(2)
                .AddNumericProperty("Price", "price", prop => prop
                    .IsMandatory()
                    .WithDescription("Product price")
                    .WithValueStorageType(ValueStorageType.Decimal))
                .AddNumericProperty("Sale Price", "salePrice", prop => prop
                    .WithDescription("Discounted price if on sale")
                    .WithValueStorageType(ValueStorageType.Decimal))
                .AddCheckboxProperty("On Sale", "onSale", prop => prop
                    .WithDescription("Is this product currently on sale?"))
                .AddNumericProperty("Stock Quantity", "stockQuantity", prop => prop
                    .WithDescription("Number of items in stock")
                    .WithValueStorageType(ValueStorageType.Integer)))
            .AddTab("SEO", tab => tab
                .WithAlias("seo")
                .WithSortOrder(3)
                .AddTextBoxProperty("Meta Title", "metaTitle", prop => prop
                    .WithDescription("SEO title for search engines"))
                .AddTextAreaProperty("Meta Description", "metaDescription", prop => prop
                    .WithDescription("SEO description for search engines")))
            .Build();

        _contentTypeService.Save(product);
    }

    #endregion

    #region Element Type Examples

    /// <summary>
    /// Example 4: Creating element types for Block Grid/List
    /// Element types are used for reusable content blocks
    /// </summary>
    public void CreateContentBlocks()
    {
        // Hero Block Element Type
        var heroBlock = new DocumentTypeBuilder(_shortStringHelper)
            .WithAlias("heroBlock")
            .WithName("Hero Block")
            .WithDescription("Large promotional banner block")
            .WithIcon("icon-picture")
            .IsElement(true)                                // This makes it an element type
            .AllowAtRoot(false)                            // Element types can't be at root
            .AddTab("Content", tab => tab
                .AddTextBoxProperty("Headline", "headline", prop => prop
                    .IsMandatory()
                    .WithDescription("Large headline text"))
                .AddTextAreaProperty("Subheading", "subheading", prop => prop
                    .WithDescription("Supporting text"))
                .AddMediaPickerProperty("Background Image", "backgroundImage", prop => prop
                    .WithDescription("Hero background image"))
                .AddTextBoxProperty("Button Text", "buttonText", prop => prop
                    .WithDescription("Call-to-action button text"))
                .AddContentPickerProperty("Button Link", "buttonLink", prop => prop
                    .WithDescription("Page to link to")))
            .Build();

        // Text Block Element Type
        var textBlock = new DocumentTypeBuilder(_shortStringHelper)
            .WithAlias("textBlock")
            .WithName("Text Block")
            .WithDescription("Simple text content block")
            .WithIcon("icon-document")
            .IsElement(true)
            .AddTab("Content", tab => tab
                .AddTextBoxProperty("Heading", "heading", prop => prop
                    .WithDescription("Optional heading"))
                .AddRichTextProperty("Text Content", "textContent", prop => prop
                    .IsMandatory()
                    .WithDescription("The text content")))
            .Build();

        // Save both element types
        _contentTypeService.Save(heroBlock);
        _contentTypeService.Save(textBlock);
    }

    #endregion

    #region Composition Examples

    /// <summary>
    /// Example 5: Using composition to share properties between document types
    /// Composition allows inheritance of properties from other document types
    /// </summary>
    public void CreateWithComposition()
    {
        // First, create a base page type with common properties
        var basePage = new DocumentTypeBuilder(_shortStringHelper)
            .WithAlias("basePage")
            .WithName("Base Page")
            .WithDescription("Base properties shared by all pages")
            .WithIcon("icon-document")
            .AddTab("SEO", tab => tab
                .WithAlias("seo")
                .WithSortOrder(99)                          // Put SEO tab last
                .AddTextBoxProperty("Meta Title", "metaTitle", prop => prop
                    .WithDescription("Page title for search engines"))
                .AddTextAreaProperty("Meta Description", "metaDescription", prop => prop
                    .WithDescription("Page description for search engines"))
                .AddCheckboxProperty("Hide from Search", "hideFromSearch", prop => prop
                    .WithDescription("Prevent search engines from indexing this page")))
            .Build();

        // Save the base page first
        _contentTypeService.Save(basePage);

        // Now create a content page that inherits from base page
        var contentPage = new DocumentTypeBuilder(_shortStringHelper)
            .WithAlias("contentPage")
            .WithName("Content Page")
            .WithDescription("General content page with SEO properties")
            .WithIcon("icon-document")
            .AllowAtRoot(true)
            .AddComposition(basePage)                       // Inherit properties from base page
            .AddTab("Content", tab => tab
                .WithAlias("content")
                .WithSortOrder(1)                          // Content tab appears before SEO
                .AddTextBoxProperty("Page Title", "pageTitle", prop => prop
                    .IsMandatory()
                    .WithDescription("The main page title"))
                .AddRichTextProperty("Main Content", "mainContent", prop => prop
                    .WithDescription("The main page content")))
            .Build();

        _contentTypeService.Save(contentPage);
    }

    #endregion

    #region Advanced Examples

    /// <summary>
    /// Example 6: Creating a complex landing page with all features
    /// Demonstrates advanced usage with multiple tabs and property types
    /// </summary>
    public void CreateAdvancedLandingPage()
    {
        var landingPage = new DocumentTypeBuilder(_shortStringHelper)
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
                    .WithDescription("Main headline - keep under 60 characters")
                    .WithSortOrder(1)
                    .WithValidationRegex("^.{1,60}$"))
                .AddTextAreaProperty("Hero Subtitle", "heroSubtitle", prop => prop
                    .WithDescription("Supporting text under the headline")
                    .WithSortOrder(2))
                .AddMediaPickerProperty("Hero Background", "heroBackground", prop => prop
                    .WithDescription("Background image or video")
                    .WithSortOrder(3))
                .AddTextBoxProperty("Primary CTA Text", "primaryCtaText", prop => prop
                    .IsMandatory()
                    .WithDescription("Main call-to-action button text")
                    .WithSortOrder(4))
                .AddContentPickerProperty("Primary CTA Link", "primaryCtaLink", prop => prop
                    .WithDescription("Where the CTA button links to")
                    .WithSortOrder(5)))
            .AddTab("Content Sections", tab => tab
                .WithAlias("contentSections")
                .WithSortOrder(2)
                .AddRichTextProperty("Main Content", "mainContent", prop => prop
                    .WithDescription("Primary content area"))
                .AddMediaPickerProperty("Feature Images", "featureImages", prop => prop
                    .WithDescription("Images showcasing features"))
                .AddTextAreaProperty("Customer Testimonial", "testimonial", prop => prop
                    .WithDescription("Customer quote or testimonial")))
            .AddTab("Conversion Tracking", tab => tab
                .WithAlias("tracking")
                .WithSortOrder(3)
                .AddTextBoxProperty("Google Analytics ID", "gaId", prop => prop
                    .WithDescription("GA tracking code for this page"))
                .AddTextBoxProperty("Facebook Pixel ID", "fbPixelId", prop => prop
                    .WithDescription("Facebook conversion tracking"))
                .AddCheckboxProperty("Enable Heat Mapping", "enableHeatMap", prop => prop
                    .WithDescription("Track user interactions")))
            .AddTab("Settings", tab => tab
                .WithAlias("settings")
                .WithSortOrder(4)
                .AddDatePickerProperty("Campaign Start Date", "campaignStart", prop => prop
                    .WithDescription("When this landing page goes live"))
                .AddDatePickerProperty("Campaign End Date", "campaignEnd", prop => prop
                    .WithDescription("When to disable this landing page"))
                .AddCheckboxProperty("Show Navigation", "showNavigation", prop => prop
                    .WithDescription("Include site navigation on this page"))
                .AddNumericProperty("A/B Test Variant", "abTestVariant", prop => prop
                    .WithDescription("Version number for A/B testing")
                    .WithValueStorageType(ValueStorageType.Integer)))
            .Build();

        _contentTypeService.Save(landingPage);
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Example 7: Helper method to create multiple related document types
    /// Shows how you might organize document type creation in a real project
    /// </summary>
    public void CreateBlogStructure()
    {
        // Blog Container
        var blogContainer = new DocumentTypeBuilder(_shortStringHelper)
            .WithAlias("blogContainer")
            .WithName("Blog")
            .WithDescription("Container for blog posts")
            .WithIcon("icon-newspaper-alt")
            .AllowAtRoot(true)
            .AddTab("Settings", tab => tab
                .AddTextBoxProperty("Blog Title", "blogTitle", prop => prop
                    .IsMandatory()
                    .WithDescription("Title for the blog section"))
                .AddTextAreaProperty("Blog Description", "blogDescription")
                .AddNumericProperty("Posts Per Page", "postsPerPage", prop => prop
                    .WithDescription("Number of posts to show per page")))
            .Build();

        // Blog Category
        var blogCategory = new DocumentTypeBuilder(_shortStringHelper)
            .WithAlias("blogCategory")
            .WithName("Blog Category")
            .WithDescription("Category for organizing blog posts")
            .WithIcon("icon-folder")
            .AllowAtRoot(false)
            .AddTab("Category Info", tab => tab
                .AddTextBoxProperty("Category Name", "categoryName", prop => prop
                    .IsMandatory())
                .AddTextAreaProperty("Category Description", "categoryDescription")
                .AddMediaPickerProperty("Category Image", "categoryImage"))
            .Build();

        // Blog Post (already created in earlier example, but shown for completeness)
        var blogPost = new DocumentTypeBuilder(_shortStringHelper)
            .WithAlias("blogPost")
            .WithName("Blog Post")
            .WithDescription("Individual blog entry")
            .WithIcon("icon-edit")
            .AllowAtRoot(false)
            .AddTab("Post Content", tab => tab
                .AddTextBoxProperty("Post Title", "postTitle", prop => prop.IsMandatory())
                .AddRichTextProperty("Post Content", "postContent", prop => prop.IsMandatory())
                .AddMediaPickerProperty("Featured Image", "featuredImage"))
            .AddTab("Post Settings", tab => tab
                .AddDatePickerProperty("Publish Date", "publishDate", prop => prop.IsMandatory())
                .AddTextBoxProperty("Author", "author")
                .AddCheckboxProperty("Featured Post", "featuredPost"))
            .Build();

        // Save all document types
        _contentTypeService.Save(blogContainer);
        _contentTypeService.Save(blogCategory);
        _contentTypeService.Save(blogPost);
    }

    #endregion
}

/// <summary>
/// Extension methods for common document type patterns
/// This shows how you might extend the builder for organization-specific needs
/// </summary>
public static class DocTypeBuilderExtensions
{
    /// <summary>
    /// Adds a standard SEO tab to any document type
    /// </summary>
    public static DocumentTypeBuilder AddSeoTab(this DocumentTypeBuilder builder, int sortOrder = 99)
    {
        return builder.AddTab("SEO", tab => tab
            .WithAlias("seo")
            .WithSortOrder(sortOrder)
            .AddTextBoxProperty("Meta Title", "metaTitle", prop => prop
                .WithDescription("Page title for search engines (50-60 characters)")
                .WithValidationRegex("^.{1,60}$"))
            .AddTextAreaProperty("Meta Description", "metaDescription", prop => prop
                .WithDescription("Page description for search engines (150-160 characters)")
                .WithValidationRegex("^.{1,160}$"))
            .AddTextBoxProperty("Open Graph Title", "ogTitle", prop => prop
                .WithDescription("Title when shared on social media"))
            .AddTextAreaProperty("Open Graph Description", "ogDescription", prop => prop
                .WithDescription("Description when shared on social media"))
            .AddMediaPickerProperty("Open Graph Image", "ogImage", prop => prop
                .WithDescription("Image when shared on social media")));
    }

    /// <summary>
    /// Adds standard content fields (title + rich text)
    /// </summary>
    public static TabBuilder AddStandardContent(this TabBuilder tabBuilder)
    {
        return tabBuilder
            .AddTextBoxProperty("Page Title", "pageTitle", prop => prop
                .IsMandatory()
                .WithDescription("The main page title"))
            .AddRichTextProperty("Main Content", "mainContent", prop => prop
                .WithDescription("The main page content"));
    }
}