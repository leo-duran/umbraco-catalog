using FluentAssertions;
using Moq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;
using Umbraco.DocTypeBuilder;
using Xunit;

namespace Umbraco.DocTypeBuilder.Tests;

/// <summary>
/// Unit tests for TabBuilder class - testing in isolation
/// </summary>
public class TabBuilderTests
{
    private readonly Mock<IShortStringHelper> _mockShortStringHelper;

    public TabBuilderTests()
    {
        _mockShortStringHelper = new Mock<IShortStringHelper>();
        // Setup common mock behavior
        _mockShortStringHelper
            .Setup(x => x.CleanStringForSafeAlias(It.IsAny<string>()))
            .Returns<string>(input => input?.ToLowerInvariant().Replace(" ", "") ?? string.Empty);
    }

    [Fact]
    public void TabBuilder_Should_Create_Basic_Tab_With_Required_Fields()
    {
        // Arrange
        const string tabName = "Content Tab";

        // Act
        var builder = new TabBuilder(tabName, _mockShortStringHelper.Object);
        var tab = builder.Build();

        // Assert
        tab.Should().NotBeNull();
        tab.Name.Should().Be(tabName);
        tab.Type.Should().Be(PropertyGroupType.Tab);
        tab.PropertyTypes.Should().NotBeNull();
        tab.PropertyTypes.Should().BeEmpty();
    }

    [Fact]
    public void TabBuilder_Should_Set_Alias_Correctly()
    {
        // Arrange
        const string alias = "contentTab";
        var builder = new TabBuilder("Content Tab", _mockShortStringHelper.Object);

        // Act
        var tab = builder
            .WithAlias(alias)
            .Build();

        // Assert
        tab.Alias.Should().Be(alias);
    }

    [Fact]
    public void TabBuilder_Should_Set_SortOrder_Correctly()
    {
        // Arrange
        const int sortOrder = 42;
        var builder = new TabBuilder("Content Tab", _mockShortStringHelper.Object);

        // Act
        var tab = builder
            .WithSortOrder(sortOrder)
            .Build();

        // Assert
        tab.SortOrder.Should().Be(sortOrder);
    }

    [Fact]
    public void TabBuilder_Should_Add_Custom_Property()
    {
        // Arrange
        const string propertyName = "Custom Property";
        const string propertyAlias = "customProperty";
        const string editorAlias = "Custom.Editor";
        var builder = new TabBuilder("Test Tab", _mockShortStringHelper.Object);

        // Act
        var tab = builder
            .AddProperty(propertyName, propertyAlias, editorAlias, prop => prop
                .WithDescription("Custom property description")
                .IsMandatory())
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        var property = tab.PropertyTypes!.First();
        property.Name.Should().Be(propertyName);
        property.Alias.Should().Be(propertyAlias);
        property.PropertyEditorAlias.Should().Be(editorAlias);
        property.Description.Should().Be("Custom property description");
        property.Mandatory.Should().BeTrue();
    }

    [Fact]
    public void TabBuilder_Should_Add_TextBox_Property()
    {
        // Arrange
        const string propertyName = "Title";
        const string propertyAlias = "title";
        var builder = new TabBuilder("Content", _mockShortStringHelper.Object);

        // Act
        var tab = builder
            .AddTextBoxProperty(propertyName, propertyAlias, prop => prop
                .IsMandatory()
                .WithDescription("Page title"))
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        var property = tab.PropertyTypes!.First();
        property.Name.Should().Be(propertyName);
        property.Alias.Should().Be(propertyAlias);
        property.PropertyEditorAlias.Should().Be("Umbraco.TextBox");
        property.Mandatory.Should().BeTrue();
        property.Description.Should().Be("Page title");
    }

    [Fact]
    public void TabBuilder_Should_Add_TextArea_Property()
    {
        // Arrange
        var builder = new TabBuilder("Content", _mockShortStringHelper.Object);

        // Act
        var tab = builder
            .AddTextAreaProperty("Summary", "summary")
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        var property = tab.PropertyTypes!.First();
        property.PropertyEditorAlias.Should().Be("Umbraco.TextArea");
    }

    [Fact]
    public void TabBuilder_Should_Add_RichText_Property()
    {
        // Arrange
        var builder = new TabBuilder("Content", _mockShortStringHelper.Object);

        // Act
        var tab = builder
            .AddRichTextProperty("Content", "content")
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        var property = tab.PropertyTypes!.First();
        property.PropertyEditorAlias.Should().Be("Umbraco.TinyMCE");
    }

    [Fact]
    public void TabBuilder_Should_Add_MediaPicker_Property()
    {
        // Arrange
        var builder = new TabBuilder("Media", _mockShortStringHelper.Object);

        // Act
        var tab = builder
            .AddMediaPickerProperty("Image", "image")
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        var property = tab.PropertyTypes!.First();
        property.PropertyEditorAlias.Should().Be("Umbraco.MediaPicker3");
    }

    [Fact]
    public void TabBuilder_Should_Add_ContentPicker_Property()
    {
        // Arrange
        var builder = new TabBuilder("Links", _mockShortStringHelper.Object);

        // Act
        var tab = builder
            .AddContentPickerProperty("Related Page", "relatedPage")
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        var property = tab.PropertyTypes!.First();
        property.PropertyEditorAlias.Should().Be("Umbraco.ContentPicker");
    }

    [Fact]
    public void TabBuilder_Should_Add_Numeric_Property()
    {
        // Arrange
        var builder = new TabBuilder("Settings", _mockShortStringHelper.Object);

        // Act
        var tab = builder
            .AddNumericProperty("Price", "price")
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        var property = tab.PropertyTypes!.First();
        property.PropertyEditorAlias.Should().Be("Umbraco.Integer");
    }

    [Fact]
    public void TabBuilder_Should_Add_Checkbox_Property()
    {
        // Arrange
        var builder = new TabBuilder("Settings", _mockShortStringHelper.Object);

        // Act
        var tab = builder
            .AddCheckboxProperty("Featured", "featured")
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        var property = tab.PropertyTypes!.First();
        property.PropertyEditorAlias.Should().Be("Umbraco.TrueFalse");
    }

    [Fact]
    public void TabBuilder_Should_Add_DatePicker_Property()
    {
        // Arrange
        var builder = new TabBuilder("Dates", _mockShortStringHelper.Object);

        // Act
        var tab = builder
            .AddDatePickerProperty("Publish Date", "publishDate")
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        var property = tab.PropertyTypes!.First();
        property.PropertyEditorAlias.Should().Be("Umbraco.DateTime");
    }

    [Fact]
    public void TabBuilder_Should_Add_Multiple_Properties()
    {
        // Arrange
        var builder = new TabBuilder("All Properties", _mockShortStringHelper.Object);

        // Act
        var tab = builder
            .AddTextBoxProperty("Title", "title")
            .AddTextAreaProperty("Summary", "summary")
            .AddRichTextProperty("Content", "content")
            .AddMediaPickerProperty("Image", "image")
            .AddContentPickerProperty("Related", "related")
            .AddNumericProperty("Price", "price")
            .AddCheckboxProperty("Featured", "featured")
            .AddDatePickerProperty("Date", "date")
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(8);
        
        var propertyAliases = tab.PropertyTypes!.Select(p => p.Alias).ToList();
        propertyAliases.Should().Contain("title");
        propertyAliases.Should().Contain("summary");
        propertyAliases.Should().Contain("content");
        propertyAliases.Should().Contain("image");
        propertyAliases.Should().Contain("related");
        propertyAliases.Should().Contain("price");
        propertyAliases.Should().Contain("featured");
        propertyAliases.Should().Contain("date");
    }

    [Fact]
    public void TabBuilder_Should_Handle_Null_Configuration_Action()
    {
        // Arrange
        var builder = new TabBuilder("Test", _mockShortStringHelper.Object);

        // Act
        var tab = builder
            .AddTextBoxProperty("Title", "title", null) // Null configuration action
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        var property = tab.PropertyTypes!.First();
        property.Name.Should().Be("Title");
        property.Alias.Should().Be("title");
        property.PropertyEditorAlias.Should().Be("Umbraco.TextBox");
        // Should have default values when no configuration is provided
        property.Mandatory.Should().BeFalse();
        property.Description.Should().BeNullOrEmpty();
    }

    [Fact]
    public void TabBuilder_Should_Chain_Configuration_Methods()
    {
        // Arrange
        const string alias = "advancedTab";
        const int sortOrder = 5;
        var builder = new TabBuilder("Advanced Tab", _mockShortStringHelper.Object);

        // Act
        var tab = builder
            .WithAlias(alias)
            .WithSortOrder(sortOrder)
            .AddTextBoxProperty("Property 1", "prop1")
            .AddTextAreaProperty("Property 2", "prop2")
            .Build();

        // Assert
        tab.Alias.Should().Be(alias);
        tab.SortOrder.Should().Be(sortOrder);
        tab.PropertyTypes.Should().HaveCount(2);
    }

    [Fact]
    public void TabBuilder_Should_Return_Same_Instance_For_Method_Chaining()
    {
        // Arrange
        var builder = new TabBuilder("Test Tab", _mockShortStringHelper.Object);

        // Act & Assert - Each method should return the same builder instance
        var result1 = builder.WithAlias("test");
        var result2 = result1.WithSortOrder(1);
        var result3 = result2.AddTextBoxProperty("Test", "test");

        result1.Should().BeSameAs(builder);
        result2.Should().BeSameAs(builder);
        result3.Should().BeSameAs(builder);
    }

    [Fact]
    public void TabBuilder_Should_Handle_Properties_With_Complex_Configuration()
    {
        // Arrange
        var builder = new TabBuilder("Complex Tab", _mockShortStringHelper.Object);

        // Act
        var tab = builder
            .AddTextBoxProperty("Title", "title", prop => prop
                .IsMandatory()
                .WithDescription("The page title")
                .WithSortOrder(1)
                .WithValidationRegex("^.{1,100}$")
                .WithLabelOnTop())
            .AddNumericProperty("Price", "price", prop => prop
                .IsMandatory()
                .WithDescription("Product price")
                .WithValueStorageType(ValueStorageType.Decimal)
                .WithSortOrder(2))
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(2);
        
        var titleProperty = tab.PropertyTypes!.First(p => p.Alias == "title");
        titleProperty.Mandatory.Should().BeTrue();
        titleProperty.Description.Should().Be("The page title");
        titleProperty.SortOrder.Should().Be(1);
        titleProperty.ValidationRegExp.Should().Be("^.{1,100}$");
        titleProperty.LabelOnTop.Should().BeTrue();

        var priceProperty = tab.PropertyTypes!.First(p => p.Alias == "price");
        priceProperty.Mandatory.Should().BeTrue();
        priceProperty.Description.Should().Be("Product price");
        priceProperty.ValueStorageType.Should().Be(ValueStorageType.Decimal);
        priceProperty.SortOrder.Should().Be(2);
    }

    [Fact]
    public void TabBuilder_Should_Handle_Empty_Property_Configuration()
    {
        // Arrange
        var builder = new TabBuilder("Empty Config Tab", _mockShortStringHelper.Object);

        // Act
        var tab = builder
            .AddTextBoxProperty("Title", "title", prop => { }) // Empty configuration
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        var property = tab.PropertyTypes!.First();
        property.Name.Should().Be("Title");
        property.Alias.Should().Be("title");
        // Should have default values
        property.Mandatory.Should().BeFalse();
        property.SortOrder.Should().Be(0);
    }

    [Theory]
    [InlineData("Umbraco.TextBox")]
    [InlineData("Umbraco.TextArea")]  
    [InlineData("Umbraco.TinyMCE")]
    [InlineData("Umbraco.MediaPicker3")]
    [InlineData("Umbraco.ContentPicker")]
    [InlineData("Umbraco.Integer")]
    [InlineData("Umbraco.TrueFalse")]
    [InlineData("Umbraco.DateTime")]
    public void TabBuilder_Should_Support_All_Common_Umbraco_Property_Editors(string editorAlias)
    {
        // Arrange
        var builder = new TabBuilder("Test Tab", _mockShortStringHelper.Object);

        // Act
        var tab = builder
            .AddProperty("Test Property", "testProperty", editorAlias, prop => { })
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        tab.PropertyTypes!.First().PropertyEditorAlias.Should().Be(editorAlias);
    }
}