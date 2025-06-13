using System.Linq;
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
    public void DocumentTypeBuilder_Should_Create_Basic_ContentType()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act
        var contentType = builder
            .SetAlias("testAlias")
            .SetName("Test Content Type")
            .Build();

        // Assert
        contentType.Should().NotBeNull();
        contentType.Alias.Should().Be("testAlias");
        contentType.Name.Should().Be("Test Content Type");
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Set_Name()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act
        var contentType = builder
            .SetAlias("page")
            .SetName("Page")
            .Build();

        // Assert
        contentType.Name.Should().Be("Page");
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Set_Description()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act
        var contentType = builder
            .SetAlias("page")
            .SetName("Page")
            .SetDescription("A basic page content type")
            .Build();

        // Assert
        contentType.Description.Should().Be("A basic page content type");
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Set_Icon()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act
        var contentType = builder
            .SetAlias("page")
            .SetName("Page")
            .SetIcon("icon-document")
            .Build();

        // Assert
        contentType.Icon.Should().Be("icon-document");
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Set_AllowedAtRoot()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act
        var contentType = builder
            .SetAlias("homePage")
            .SetName("Home Page")
            .SetAllowedAtRoot(true)
            .Build();

        // Assert
        contentType.AllowedAsRoot.Should().BeTrue();
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Set_AllowedAtRoot_False()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act
        var contentType = builder
            .SetAlias("subPage")
            .SetName("Sub Page")
            .SetAllowedAtRoot(false)
            .Build();

        // Assert
        contentType.AllowedAsRoot.Should().BeFalse();
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Set_IsElement_True()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act
        var contentType = builder
            .SetAlias("blockElement")
            .SetName("Block Element")
            .SetIsElement(true)
            .Build();

        // Assert
        contentType.IsElement.Should().BeTrue();
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Set_IsElement_False()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act
        var contentType = builder
            .SetAlias("regularPage")
            .SetName("Regular Page")
            .SetIsElement(false)
            .Build();

        // Assert
        contentType.IsElement.Should().BeFalse();
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Add_Tab_With_Properties()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act
        var contentType = builder
            .SetAlias("page")
            .SetName("Page")
            .AddTab("Content", tab => tab
                .AddTextBoxProperty("title", "Title")
                .AddTextAreaProperty("description", "Description"))
            .Build();

        // Assert
        contentType.PropertyGroups.Should().HaveCount(1);
        var tab = contentType.PropertyGroups.First();
        tab.Name.Should().Be("Content");
        tab.PropertyTypes.Should().HaveCount(2);
        
        var titleProperty = tab.PropertyTypes.FirstOrDefault(p => p.Alias == "title");
        titleProperty.Should().NotBeNull();
        titleProperty!.Name.Should().Be("Title");
        
        var descriptionProperty = tab.PropertyTypes.FirstOrDefault(p => p.Alias == "description");
        descriptionProperty.Should().NotBeNull();
        descriptionProperty!.Name.Should().Be("Description");
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Add_Multiple_Tabs()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act
        var contentType = builder
            .SetAlias("page")
            .SetName("Page")
            .AddTab("Content", tab => tab
                .AddTextBoxProperty("title", "Title"))
            .AddTab("Settings", tab => tab
                .AddCheckboxProperty("hideFromNavigation", "Hide from Navigation"))
            .Build();

        // Assert
        contentType.PropertyGroups.Should().HaveCount(2);
        
        var contentTab = contentType.PropertyGroups.FirstOrDefault(g => g.Name == "Content");
        contentTab.Should().NotBeNull();
        contentTab!.PropertyTypes.Should().HaveCount(1);
        
        var settingsTab = contentType.PropertyGroups.FirstOrDefault(g => g.Name == "Settings");
        settingsTab.Should().NotBeNull();
        settingsTab!.PropertyTypes.Should().HaveCount(1);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Chain_All_Configuration_Methods()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act
        var contentType = builder
            .SetAlias("complexPage")
            .SetName("Complex Page")
            .SetDescription("A complex page with all features")
            .SetIcon("icon-layout")
            .SetAllowedAtRoot(true)
            .SetIsElement(false)
            .Build();

        // Assert
        contentType.Should().NotBeNull();
        contentType.Alias.Should().Be("complexPage");
        contentType.Name.Should().Be("Complex Page");
        contentType.Description.Should().Be("A complex page with all features");
        contentType.Icon.Should().Be("icon-layout");
        contentType.AllowedAsRoot.Should().BeTrue();
        contentType.IsElement.Should().BeFalse();
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Have_Default_Values()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act
        var contentType = builder.Build();

        // Assert
        contentType.Should().NotBeNull();
        contentType.Alias.Should().Be("defaultAlias");
        contentType.Name.Should().Be(string.Empty);
        contentType.Icon.Should().Be("icon-document");
        contentType.AllowedAsRoot.Should().BeFalse();
        contentType.IsElement.Should().BeFalse();
        contentType.PropertyGroups.Should().BeEmpty();
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Create_Document_Type_With_Complex_Structure()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act
        var contentType = builder
            .SetAlias("blogPost")
            .SetName("Blog Post")
            .SetDescription("A blog post content type")
            .SetIcon("icon-edit")
            .AddTab("Content", tab => tab
                .AddTextBoxProperty("title", "Title", prop => prop.SetMandatory(true))
                .AddTextAreaProperty("excerpt", "Excerpt")
                .AddRichTextProperty("content", "Content", prop => prop.SetMandatory(true)))
            .AddTab("Media", tab => tab
                .AddMediaPickerProperty("featuredImage", "Featured Image"))
            .AddTab("Settings", tab => tab
                .AddDatePickerProperty("publishDate", "Publish Date")
                .AddCheckboxProperty("featured", "Featured"))
            .Build();

        // Assert
        contentType.Should().NotBeNull();
        contentType.Alias.Should().Be("blogPost");
        contentType.Name.Should().Be("Blog Post");
        contentType.PropertyGroups.Should().HaveCount(3);
        
        // Verify Content tab
        var contentTab = contentType.PropertyGroups.First(g => g.Name == "Content");
        contentTab.PropertyTypes.Should().HaveCount(3);
        
        // Verify Media tab
        var mediaTab = contentType.PropertyGroups.First(g => g.Name == "Media");
        mediaTab.PropertyTypes.Should().HaveCount(1);
        
        // Verify Settings tab
        var settingsTab = contentType.PropertyGroups.First(g => g.Name == "Settings");
        settingsTab.PropertyTypes.Should().HaveCount(2);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Handle_Empty_Tab_Configuration()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act
        var contentType = builder
            .SetAlias("simplePage")
            .SetName("Simple Page")
            .AddTab("Empty Tab")
            .Build();

        // Assert
        contentType.PropertyGroups.Should().HaveCount(1);
        var tab = contentType.PropertyGroups.First();
        tab.Name.Should().Be("Empty Tab");
        tab.PropertyTypes.Should().BeEmpty();
    }
}