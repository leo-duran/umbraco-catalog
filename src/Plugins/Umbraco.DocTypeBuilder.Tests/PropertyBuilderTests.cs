using FluentAssertions;
using Moq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;
using Umbraco.DocTypeBuilder;
using Xunit;

namespace Umbraco.DocTypeBuilder.Tests;

/// <summary>
/// Unit tests for PropertyBuilder class - testing in isolation
/// </summary>
public class PropertyBuilderTests
{
    private readonly Mock<IShortStringHelper> _mockShortStringHelper;

    public PropertyBuilderTests()
    {
        _mockShortStringHelper = new Mock<IShortStringHelper>();
        // Setup common mock behavior
        _mockShortStringHelper
            .Setup(x => x.CleanStringForSafeAlias(It.IsAny<string>()))
            .Returns<string>(input => input?.ToLowerInvariant().Replace(" ", "") ?? string.Empty);
    }

    [Fact]
    public void PropertyBuilder_Should_Create_Basic_Property_With_Required_Fields()
    {
        // Arrange
        const string name = "Test Property";
        const string alias = "testProperty";
        const string editorAlias = "Umbraco.TextBox";

        // Act
        var builder = new PropertyBuilder(name, alias, editorAlias, _mockShortStringHelper.Object);
        var property = builder.Build();

        // Assert
        property.Should().NotBeNull();
        property.Name.Should().Be(name);
        property.Alias.Should().Be(alias);
        property.PropertyEditorAlias.Should().Be(editorAlias);
        property.ValueStorageType.Should().Be(ValueStorageType.Nvarchar); // Default value
    }

    [Fact]
    public void PropertyBuilder_Should_Set_Description_Correctly()
    {
        // Arrange
        const string description = "This is a test property description";
        var builder = new PropertyBuilder("Test", "test", "Umbraco.TextBox", _mockShortStringHelper.Object);

        // Act
        var property = builder
            .WithDescription(description)
            .Build();

        // Assert
        property.Description.Should().Be(description);
    }

    [Fact]
    public void PropertyBuilder_Should_Set_Mandatory_Flag_Correctly()
    {
        // Arrange
        var builder = new PropertyBuilder("Test", "test", "Umbraco.TextBox", _mockShortStringHelper.Object);

        // Act
        var mandatoryProperty = builder
            .IsMandatory(true)
            .Build();

        var optionalProperty = new PropertyBuilder("Test2", "test2", "Umbraco.TextBox", _mockShortStringHelper.Object)
            .IsMandatory(false)
            .Build();

        // Assert
        mandatoryProperty.Mandatory.Should().BeTrue();
        optionalProperty.Mandatory.Should().BeFalse();
    }

    [Fact]
    public void PropertyBuilder_Should_Set_Mandatory_Flag_Default_True_When_No_Parameter()
    {
        // Arrange
        var builder = new PropertyBuilder("Test", "test", "Umbraco.TextBox", _mockShortStringHelper.Object);

        // Act
        var property = builder
            .IsMandatory() // No parameter - should default to true
            .Build();

        // Assert
        property.Mandatory.Should().BeTrue();
    }

    [Theory]
    [InlineData(ValueStorageType.Nvarchar)]
    [InlineData(ValueStorageType.Ntext)]
    [InlineData(ValueStorageType.Integer)]
    [InlineData(ValueStorageType.Decimal)]
    [InlineData(ValueStorageType.Date)]
    public void PropertyBuilder_Should_Set_ValueStorageType_Correctly(ValueStorageType storageType)
    {
        // Arrange
        var builder = new PropertyBuilder("Test", "test", "Umbraco.TextBox", _mockShortStringHelper.Object);

        // Act
        var property = builder
            .WithValueStorageType(storageType)
            .Build();

        // Assert
        property.ValueStorageType.Should().Be(storageType);
    }

    [Fact]
    public void PropertyBuilder_Should_Set_SortOrder_Correctly()
    {
        // Arrange
        const int sortOrder = 42;
        var builder = new PropertyBuilder("Test", "test", "Umbraco.TextBox", _mockShortStringHelper.Object);

        // Act
        var property = builder
            .WithSortOrder(sortOrder)
            .Build();

        // Assert
        property.SortOrder.Should().Be(sortOrder);
    }

    [Fact]
    public void PropertyBuilder_Should_Set_ValidationRegex_Correctly()
    {
        // Arrange
        const string regex = "^[a-zA-Z0-9]*$";
        var builder = new PropertyBuilder("Test", "test", "Umbraco.TextBox", _mockShortStringHelper.Object);

        // Act
        var property = builder
            .WithValidationRegex(regex)
            .Build();

        // Assert
        property.ValidationRegExp.Should().Be(regex);
    }

    [Fact]
    public void PropertyBuilder_Should_Set_DataTypeDefinitionId_Correctly()
    {
        // Arrange
        const int dataTypeId = 123;
        var builder = new PropertyBuilder("Test", "test", "Umbraco.TextBox", _mockShortStringHelper.Object);

        // Act
        var property = builder
            .WithDataTypeDefinitionId(dataTypeId)
            .Build();

        // Assert
        property.DataTypeId.Should().Be(dataTypeId);
    }

    [Fact]
    public void PropertyBuilder_Should_Set_LabelOnTop_Correctly()
    {
        // Arrange
        var builder = new PropertyBuilder("Test", "test", "Umbraco.TextBox", _mockShortStringHelper.Object);

        // Act
        var labelOnTopProperty = builder
            .WithLabelOnTop(true)
            .Build();

        var labelNotOnTopProperty = new PropertyBuilder("Test2", "test2", "Umbraco.TextBox", _mockShortStringHelper.Object)
            .WithLabelOnTop(false)
            .Build();

        // Assert
        labelOnTopProperty.LabelOnTop.Should().BeTrue();
        labelNotOnTopProperty.LabelOnTop.Should().BeFalse();
    }

    [Fact]
    public void PropertyBuilder_Should_Set_LabelOnTop_Default_True_When_No_Parameter()
    {
        // Arrange
        var builder = new PropertyBuilder("Test", "test", "Umbraco.TextBox", _mockShortStringHelper.Object);

        // Act
        var property = builder
            .WithLabelOnTop() // No parameter - should default to true
            .Build();

        // Assert
        property.LabelOnTop.Should().BeTrue();
    }

    [Fact]
    public void PropertyBuilder_Should_Chain_Multiple_Configuration_Methods()
    {
        // Arrange
        const string description = "Advanced property configuration";
        const int sortOrder = 5;
        const string regex = "^[a-zA-Z0-9]*$";

        var builder = new PropertyBuilder("Advanced Property", "advancedProperty", "Umbraco.TextBox", _mockShortStringHelper.Object);

        // Act
        var property = builder
            .WithDescription(description)
            .IsMandatory(true)
            .WithSortOrder(sortOrder)
            .WithValidationRegex(regex)
            .WithLabelOnTop(true)
            .WithValueStorageType(ValueStorageType.Ntext)
            .Build();

        // Assert
        property.Description.Should().Be(description);
        property.Mandatory.Should().BeTrue();
        property.SortOrder.Should().Be(sortOrder);
        property.ValidationRegExp.Should().Be(regex);
        property.LabelOnTop.Should().BeTrue();
        property.ValueStorageType.Should().Be(ValueStorageType.Ntext);
    }

    [Fact]
    public void PropertyBuilder_Should_Handle_Empty_String_Values_Gracefully()
    {
        // Arrange & Act
        var property = new PropertyBuilder("", "", "", _mockShortStringHelper.Object)
            .WithDescription("")
            .WithValidationRegex("")
            .Build();

        // Assert
        property.Name.Should().Be("");
        property.Alias.Should().Be("");
        property.PropertyEditorAlias.Should().Be("");
        property.Description.Should().Be("");
        property.ValidationRegExp.Should().Be("");
    }

    [Fact]
    public void PropertyBuilder_Should_Handle_Common_Umbraco_Editor_Aliases()
    {
        // Arrange & Act
        var textBoxProperty = new PropertyBuilder("TextBox", "textBox", "Umbraco.TextBox", _mockShortStringHelper.Object).Build();
        var textAreaProperty = new PropertyBuilder("TextArea", "textArea", "Umbraco.TextArea", _mockShortStringHelper.Object).Build();
        var richTextProperty = new PropertyBuilder("RichText", "richText", "Umbraco.TinyMCE", _mockShortStringHelper.Object).Build();
        var mediaPickerProperty = new PropertyBuilder("MediaPicker", "mediaPicker", "Umbraco.MediaPicker3", _mockShortStringHelper.Object).Build();
        var contentPickerProperty = new PropertyBuilder("ContentPicker", "contentPicker", "Umbraco.ContentPicker", _mockShortStringHelper.Object).Build();
        var numericProperty = new PropertyBuilder("Numeric", "numeric", "Umbraco.Integer", _mockShortStringHelper.Object).Build();
        var checkboxProperty = new PropertyBuilder("Checkbox", "checkbox", "Umbraco.TrueFalse", _mockShortStringHelper.Object).Build();
        var datePickerProperty = new PropertyBuilder("DatePicker", "datePicker", "Umbraco.DateTime", _mockShortStringHelper.Object).Build();

        // Assert
        textBoxProperty.PropertyEditorAlias.Should().Be("Umbraco.TextBox");
        textAreaProperty.PropertyEditorAlias.Should().Be("Umbraco.TextArea");
        richTextProperty.PropertyEditorAlias.Should().Be("Umbraco.TinyMCE");
        mediaPickerProperty.PropertyEditorAlias.Should().Be("Umbraco.MediaPicker3");
        contentPickerProperty.PropertyEditorAlias.Should().Be("Umbraco.ContentPicker");
        numericProperty.PropertyEditorAlias.Should().Be("Umbraco.Integer");
        checkboxProperty.PropertyEditorAlias.Should().Be("Umbraco.TrueFalse");
        datePickerProperty.PropertyEditorAlias.Should().Be("Umbraco.DateTime");
    }

    [Fact]
    public void PropertyBuilder_Should_Return_Same_Instance_For_Method_Chaining()
    {
        // Arrange
        var builder = new PropertyBuilder("Test", "test", "Umbraco.TextBox", _mockShortStringHelper.Object);

        // Act & Assert - Each method should return the same builder instance
        var result1 = builder.WithDescription("test");
        var result2 = result1.IsMandatory();
        var result3 = result2.WithSortOrder(1);

        result1.Should().BeSameAs(builder);
        result2.Should().BeSameAs(builder);
        result3.Should().BeSameAs(builder);
    }
}