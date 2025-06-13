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
        // Arrange & Act
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);
        var contentType = builder.Build();

        // Assert
        contentType.Should().NotBeNull();
        contentType.Should().BeOfType<ContentType>();
        contentType.Name.Should().Be(string.Empty); // Default empty
        contentType.Alias.Should().Be(string.Empty); // Default empty
        contentType.Description.Should().BeNullOrEmpty();
        contentType.AllowedAsRoot.Should().BeFalse(); // Default false
        contentType.IsElement.Should().BeFalse(); // Default false
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Set_Alias_Correctly()
    {
        // Arrange
        const string alias = "testAlias";

        // Act
        var contentType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias(alias)
            .Build();

        // Assert
        contentType.Alias.Should().Be(alias);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Set_Name_Correctly()
    {
        // Arrange
        const string name = "Test Document Type";

        // Act
        var contentType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithName(name)
            .Build();

        // Assert
        contentType.Name.Should().Be(name);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Set_Description_Correctly()
    {
        // Arrange
        const string description = "Test description for document type";

        // Act
        var contentType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithDescription(description)
            .Build();

        // Assert
        contentType.Description.Should().Be(description);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Set_Icon_Correctly()
    {
        // Arrange
        const string icon = "icon-document";

        // Act
        var contentType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithIcon(icon)
            .Build();

        // Assert
        contentType.Icon.Should().Be(icon);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Allow_At_Root_When_True()
    {
        // Act
        var contentType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .AllowAtRoot(true)
            .Build();

        // Assert
        contentType.AllowedAsRoot.Should().BeTrue();
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Allow_At_Root_With_Default_Parameter()
    {
        // Act
        var contentType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .AllowAtRoot()
            .Build();

        // Assert
        contentType.AllowedAsRoot.Should().BeTrue();
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Not_Allow_At_Root_When_False()
    {
        // Act
        var contentType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .AllowAtRoot(false)
            .Build();

        // Assert
        contentType.AllowedAsRoot.Should().BeFalse();
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Set_IsElement_True()
    {
        // Act
        var contentType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .IsElement(true)
            .Build();

        // Assert
        contentType.IsElement.Should().BeTrue();
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Set_IsElement_With_Default_Parameter()
    {
        // Act
        var contentType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .IsElement()
            .Build();

        // Assert
        contentType.IsElement.Should().BeTrue();
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Set_IsElement_False()
    {
        // Act
        var contentType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .IsElement(false)
            .Build();

        // Assert
        contentType.IsElement.Should().BeFalse();
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Add_Single_Tab()
    {
        // Act
        var contentType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .AddTab("Content", tab => tab
                .WithAlias("content")
                .WithSortOrder(1))
            .Build();

        // Assert
        contentType.PropertyGroups.Should().HaveCount(1);
        var tab = contentType.PropertyGroups.First();
        tab.Name.Should().Be("Content");
        tab.Alias.Should().Be("content");
        tab.SortOrder.Should().Be(1);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Add_Multiple_Tabs()
    {
        // Act
        var contentType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .AddTab("Content", tab => tab
                .WithAlias("content")
                .WithSortOrder(1))
            .AddTab("Settings", tab => tab
                .WithAlias("settings")
                .WithSortOrder(2))
            .Build();

        // Assert
        contentType.PropertyGroups.Should().HaveCount(2);
        contentType.PropertyGroups.Should().Contain(t => t.Name == "Content");
        contentType.PropertyGroups.Should().Contain(t => t.Name == "Settings");
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Add_Tab_With_Properties()
    {
        // Act
        var contentType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .AddTab("Content", tab => tab
                .WithAlias("content")
                .AddTextBoxProperty("Title", "title", prop => prop
                    .IsMandatory()
                    .WithDescription("Page title"))
                .AddTextAreaProperty("Summary", "summary"))
            .Build();

        // Assert
        contentType.PropertyGroups.Should().HaveCount(1);
        var tab = contentType.PropertyGroups.First();
        tab.PropertyTypes.Should().HaveCount(2);
        tab.PropertyTypes!.Should().Contain(p => p.Alias == "title");
        tab.PropertyTypes!.Should().Contain(p => p.Alias == "summary");
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Add_Composition()
    {
        // Arrange
        var baseContentType = new Mock<IContentTypeComposition>();
        baseContentType.Setup(x => x.Alias).Returns("baseType");

        // Act
        var contentType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("derivedType")
            .AddComposition(baseContentType.Object)
            .Build();

        // Assert
        contentType.ContentTypeComposition.Should().HaveCount(1);
        contentType.ContentTypeComposition.Should().Contain(baseContentType.Object);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Chain_All_Configuration_Methods()
    {
        // Arrange
        const string alias = "testContentType";
        const string name = "Test Content Type";
        const string description = "A test content type";
        const string icon = "icon-document";
        var mockComposition = new Mock<IContentTypeComposition>();

        // Act
        var contentType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias(alias)
            .WithName(name)
            .WithDescription(description)
            .WithIcon(icon)
            .AllowAtRoot(true)
            .IsElement(false)
            .AddTab("Content", tab => tab
                .WithAlias("content")
                .AddTextBoxProperty("Title", "title"))
            .AddComposition(mockComposition.Object)
            .Build();

        // Assert
        contentType.Alias.Should().Be(alias);
        contentType.Name.Should().Be(name);
        contentType.Description.Should().Be(description);
        contentType.Icon.Should().Be(icon);
        contentType.AllowedAsRoot.Should().BeTrue();
        contentType.IsElement.Should().BeFalse();
        contentType.PropertyGroups.Should().HaveCount(1);
        contentType.ContentTypeComposition.Should().HaveCount(1);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Return_Same_Instance_For_Method_Chaining()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act & Assert - Each method should return the same builder instance
        builder.WithAlias("test").Should().BeSameAs(builder);
        builder.WithName("Test").Should().BeSameAs(builder);
        builder.WithDescription("Test").Should().BeSameAs(builder);
        builder.WithIcon("icon-test").Should().BeSameAs(builder);
        builder.AllowAtRoot().Should().BeSameAs(builder);
        builder.IsElement().Should().BeSameAs(builder);
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Handle_Empty_String_Values()
    {
        // Act
        var contentType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("")
            .WithName("")
            .WithDescription("")
            .WithIcon("")
            .Build();

        // Assert
        contentType.Alias.Should().Be("");
        contentType.Name.Should().Be("");
        contentType.Description.Should().Be("");
        contentType.Icon.Should().Be("");
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Build_Multiple_Different_ContentTypes()
    {
        // Arrange
        var builder = new DocumentTypeBuilder(_mockShortStringHelper.Object);

        // Act - Build multiple different content types
        var contentType1 = builder
            .WithAlias("type1")
            .WithName("Type 1")
            .Build();

        // Create a new builder for the second content type
        var builder2 = new DocumentTypeBuilder(_mockShortStringHelper.Object);
        var contentType2 = builder2
            .WithAlias("type2")
            .WithName("Type 2")
            .Build();

        // Assert - Should be different instances with different properties
        contentType1.Should().NotBeSameAs(contentType2);
        contentType1.Alias.Should().Be("type1");
        contentType2.Alias.Should().Be("type2");
    }

    [Fact]
    public void DocumentTypeBuilder_Should_Create_Element_Type_For_Block_Grid()
    {
        // Act
        var elementType = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias("heroBlock")
            .WithName("Hero Block")
            .WithDescription("Hero banner element for Block Grid")
            .WithIcon("icon-picture")
            .IsElement(true)
            .AddTab("Content", tab => tab
                .WithAlias("content")
                .AddTextBoxProperty("Headline", "headline", prop => prop
                    .IsMandatory()
                    .WithDescription("Hero headline"))
                .AddMediaPickerProperty("Background Image", "backgroundImage"))
            .Build();

        // Assert
        elementType.IsElement.Should().BeTrue();
        elementType.AllowedAsRoot.Should().BeFalse(); // Element types should not be at root
        elementType.Alias.Should().Be("heroBlock");
        elementType.Name.Should().Be("Hero Block");
        elementType.PropertyGroups.Should().HaveCount(1);
        
        var contentTab = elementType.PropertyGroups.First();
        contentTab.PropertyTypes.Should().HaveCount(2);
        contentTab.PropertyTypes!.Should().Contain(p => p.Alias == "headline");
        contentTab.PropertyTypes!.Should().Contain(p => p.Alias == "backgroundImage");
    }
}