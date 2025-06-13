using System.Linq;
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
    public void TabBuilder_Should_Create_Basic_Tab()
    {
        // Arrange
        var builder = new TabBuilder(_mockShortStringHelper.Object);

        // Act
        var tab = builder
            .SetName("Content")
            .Build();

        // Assert
        tab.Should().NotBeNull();
        tab.Name.Should().Be("Content");
    }

    [Fact]
    public void TabBuilder_Should_Set_SortOrder()
    {
        // Arrange
        var builder = new TabBuilder(_mockShortStringHelper.Object);

        // Act
        var tab = builder
            .SetName("Settings")
            .SetSortOrder(5)
            .Build();

        // Assert
        tab.SortOrder.Should().Be(5);
    }

    [Fact]
    public void TabBuilder_Should_Add_TextBox_Property()
    {
        // Arrange
        var builder = new TabBuilder(_mockShortStringHelper.Object);

        // Act
        var tab = builder
            .SetName("Content")
            .AddTextBoxProperty("title", "Title")
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        var property = tab.PropertyTypes.First();
        property.Alias.Should().Be("title");
        property.Name.Should().Be("Title");
        property.PropertyEditorAlias.Should().Be("Umbraco.TextBox");
    }

    [Fact]
    public void TabBuilder_Should_Add_TextArea_Property()
    {
        // Arrange
        var builder = new TabBuilder(_mockShortStringHelper.Object);

        // Act
        var tab = builder
            .SetName("Content")
            .AddTextAreaProperty("description", "Description")
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        var property = tab.PropertyTypes.First();
        property.Alias.Should().Be("description");
        property.Name.Should().Be("Description");
        property.PropertyEditorAlias.Should().Be("Umbraco.TextArea");
    }

    [Fact]
    public void TabBuilder_Should_Add_RichText_Property()
    {
        // Arrange
        var builder = new TabBuilder(_mockShortStringHelper.Object);

        // Act
        var tab = builder
            .SetName("Content")
            .AddRichTextProperty("bodyText", "Body Text")
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        var property = tab.PropertyTypes.First();
        property.Alias.Should().Be("bodyText");
        property.Name.Should().Be("Body Text");
        property.PropertyEditorAlias.Should().Be("Umbraco.RichText");
    }

    [Fact]
    public void TabBuilder_Should_Add_MediaPicker_Property()
    {
        // Arrange
        var builder = new TabBuilder(_mockShortStringHelper.Object);

        // Act
        var tab = builder
            .SetName("Media")
            .AddMediaPickerProperty("heroImage", "Hero Image")
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        var property = tab.PropertyTypes.First();
        property.Alias.Should().Be("heroImage");
        property.Name.Should().Be("Hero Image");
        property.PropertyEditorAlias.Should().Be("Umbraco.MediaPicker3");
    }

    [Fact]
    public void TabBuilder_Should_Add_ContentPicker_Property()
    {
        // Arrange
        var builder = new TabBuilder(_mockShortStringHelper.Object);

        // Act
        var tab = builder
            .SetName("Links")
            .AddContentPickerProperty("relatedPage", "Related Page")
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        var property = tab.PropertyTypes.First();
        property.Alias.Should().Be("relatedPage");
        property.Name.Should().Be("Related Page");
        property.PropertyEditorAlias.Should().Be("Umbraco.ContentPicker");
    }

    [Fact]
    public void TabBuilder_Should_Add_Integer_Property()
    {
        // Arrange
        var builder = new TabBuilder(_mockShortStringHelper.Object);

        // Act
        var tab = builder
            .SetName("Settings")
            .AddIntegerProperty("sortOrder", "Sort Order")
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        var property = tab.PropertyTypes.First();
        property.Alias.Should().Be("sortOrder");
        property.Name.Should().Be("Sort Order");
        property.PropertyEditorAlias.Should().Be("Umbraco.Integer");
        property.ValueStorageType.Should().Be(ValueStorageType.Integer);
    }

    [Fact]
    public void TabBuilder_Should_Add_Checkbox_Property()
    {
        // Arrange
        var builder = new TabBuilder(_mockShortStringHelper.Object);

        // Act
        var tab = builder
            .SetName("Settings")
            .AddCheckboxProperty("isActive", "Is Active")
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        var property = tab.PropertyTypes.First();
        property.Alias.Should().Be("isActive");
        property.Name.Should().Be("Is Active");
        property.PropertyEditorAlias.Should().Be("Umbraco.TrueFalse");
        property.ValueStorageType.Should().Be(ValueStorageType.Integer);
    }

    [Fact]
    public void TabBuilder_Should_Add_DatePicker_Property()
    {
        // Arrange
        var builder = new TabBuilder(_mockShortStringHelper.Object);

        // Act
        var tab = builder
            .SetName("Dates")
            .AddDatePickerProperty("publishDate", "Publish Date")
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        var property = tab.PropertyTypes.First();
        property.Alias.Should().Be("publishDate");
        property.Name.Should().Be("Publish Date");
        property.PropertyEditorAlias.Should().Be("Umbraco.DateTime");
        property.ValueStorageType.Should().Be(ValueStorageType.Date);
    }

    [Fact]
    public void TabBuilder_Should_Add_Multiple_Properties()
    {
        // Arrange
        var builder = new TabBuilder(_mockShortStringHelper.Object);

        // Act
        var tab = builder
            .SetName("Content")
            .AddTextBoxProperty("title", "Title")
            .AddTextAreaProperty("description", "Description")
            .AddRichTextProperty("bodyText", "Body Text")
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(3);
        
        var titleProperty = tab.PropertyTypes.FirstOrDefault(p => p.Alias == "title");
        titleProperty.Should().NotBeNull();
        titleProperty!.PropertyEditorAlias.Should().Be("Umbraco.TextBox");

        var descriptionProperty = tab.PropertyTypes.FirstOrDefault(p => p.Alias == "description");
        descriptionProperty.Should().NotBeNull();
        descriptionProperty!.PropertyEditorAlias.Should().Be("Umbraco.TextArea");

        var bodyTextProperty = tab.PropertyTypes.FirstOrDefault(p => p.Alias == "bodyText");
        bodyTextProperty.Should().NotBeNull();
        bodyTextProperty!.PropertyEditorAlias.Should().Be("Umbraco.RichText");
    }

    [Fact]
    public void TabBuilder_Should_Configure_Property_With_Action()
    {
        // Arrange
        var builder = new TabBuilder(_mockShortStringHelper.Object);

        // Act
        var tab = builder
            .SetName("Content")
            .AddTextBoxProperty("title", "Title", prop => prop
                .SetMandatory(true)
                .SetDescription("The page title"))
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(1);
        var property = tab.PropertyTypes.First();
        property.Mandatory.Should().BeTrue();
        property.Description.Should().Be("The page title");
    }

    [Fact]
    public void TabBuilder_Should_Handle_All_Property_Types_With_Configuration()
    {
        // Arrange
        var builder = new TabBuilder(_mockShortStringHelper.Object);

        // Act
        var tab = builder
            .SetName("All Properties")
            .AddTextBoxProperty("textBox", "Text Box", p => p.SetMandatory(true))
            .AddTextAreaProperty("textArea", "Text Area", p => p.SetDescription("Large text"))
            .AddRichTextProperty("richText", "Rich Text", p => p.SetLabelOnTop(true))
            .AddMediaPickerProperty("media", "Media", p => p.SetSortOrder(1))
            .AddContentPickerProperty("content", "Content", p => p.SetSortOrder(2))
            .AddIntegerProperty("integer", "Integer", p => p.SetSortOrder(3))
            .AddCheckboxProperty("checkbox", "Checkbox", p => p.SetSortOrder(4))
            .AddDatePickerProperty("date", "Date", p => p.SetSortOrder(5))
            .Build();

        // Assert
        tab.PropertyTypes.Should().HaveCount(8);
        
        // Verify each property type exists and has correct configuration
        var textBoxProp = tab.PropertyTypes.First(p => p.Alias == "textBox");
        textBoxProp.Mandatory.Should().BeTrue();
        
        var textAreaProp = tab.PropertyTypes.First(p => p.Alias == "textArea");
        textAreaProp.Description.Should().Be("Large text");
        
        var richTextProp = tab.PropertyTypes.First(p => p.Alias == "richText");
        richTextProp.LabelOnTop.Should().BeTrue();
        
        var mediaProp = tab.PropertyTypes.First(p => p.Alias == "media");
        mediaProp.SortOrder.Should().Be(1);
        
        var contentProp = tab.PropertyTypes.First(p => p.Alias == "content");
        contentProp.SortOrder.Should().Be(2);
        
        var integerProp = tab.PropertyTypes.First(p => p.Alias == "integer");
        integerProp.SortOrder.Should().Be(3);
        integerProp.ValueStorageType.Should().Be(ValueStorageType.Integer);
        
        var checkboxProp = tab.PropertyTypes.First(p => p.Alias == "checkbox");
        checkboxProp.SortOrder.Should().Be(4);
        checkboxProp.ValueStorageType.Should().Be(ValueStorageType.Integer);
        
        var dateProp = tab.PropertyTypes.First(p => p.Alias == "date");
        dateProp.SortOrder.Should().Be(5);
        dateProp.ValueStorageType.Should().Be(ValueStorageType.Date);
    }

    [Fact]
    public void TabBuilder_Should_Chain_All_Configuration_Methods()
    {
        // Arrange
        var builder = new TabBuilder(_mockShortStringHelper.Object);

        // Act
        var tab = builder
            .SetName("Complex Tab")
            .SetSortOrder(10)
            .AddTextBoxProperty("prop1", "Property 1")
            .AddTextAreaProperty("prop2", "Property 2")
            .Build();

        // Assert
        tab.Should().NotBeNull();
        tab.Name.Should().Be("Complex Tab");
        tab.SortOrder.Should().Be(10);
        tab.PropertyTypes.Should().HaveCount(2);
    }
}