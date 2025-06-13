using System.Linq;
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
    public void PropertyBuilder_Should_Create_Basic_Property()
    {
        // Arrange
        var builder = new PropertyBuilder(_mockShortStringHelper.Object);

        // Act
        var property = builder
            .SetAlias("testProperty")
            .SetName("Test Property")
            .Build();

        // Assert
        property.Should().NotBeNull();
        property.Alias.Should().Be("testProperty");
        property.Name.Should().Be("Test Property");
    }

    [Fact]
    public void PropertyBuilder_Should_Set_Description()
    {
        // Arrange
        var builder = new PropertyBuilder(_mockShortStringHelper.Object);

        // Act
        var property = builder
            .SetAlias("customProperty")
            .SetName("Custom Property")
            .SetDescription("This is a custom property")
            .Build();

        // Assert
        property.Description.Should().Be("This is a custom property");
    }

    [Fact]
    public void PropertyBuilder_Should_Set_Mandatory()
    {
        // Arrange
        var builder = new PropertyBuilder(_mockShortStringHelper.Object);

        // Act
        var property = builder
            .SetAlias("mandatoryProperty")
            .SetName("Mandatory Property")
            .SetMandatory(true)
            .Build();

        // Assert
        property.Mandatory.Should().BeTrue();
    }

    [Fact]
    public void PropertyBuilder_Should_Set_ValueStorageType()
    {
        // Arrange
        var builder = new PropertyBuilder(_mockShortStringHelper.Object);

        // Act
        var property = builder
            .SetAlias("integerProperty")
            .SetName("Integer Property")
            .SetValueStorageType(ValueStorageType.Integer)
            .Build();

        // Assert
        property.ValueStorageType.Should().Be(ValueStorageType.Integer);
    }

    [Fact]
    public void PropertyBuilder_Should_Set_SortOrder()
    {
        // Arrange
        var builder = new PropertyBuilder(_mockShortStringHelper.Object);

        // Act
        var property = builder
            .SetAlias("sortedProperty")
            .SetName("Sorted Property")
            .SetSortOrder(5)
            .Build();

        // Assert
        property.SortOrder.Should().Be(5);
    }

    [Fact]
    public void PropertyBuilder_Should_Set_ValidationRegex()
    {
        // Arrange
        var builder = new PropertyBuilder(_mockShortStringHelper.Object);

        // Act
        var property = builder
            .SetAlias("validatedProperty")
            .SetName("Validated Property")
            .SetValidation(@"^\d+$", "Must be numeric")
            .Build();

        // Assert
        property.ValidationRegExp.Should().Be(@"^\d+$");
        property.ValidationRegExpMessage.Should().Be("Must be numeric");
    }

    [Fact]
    public void PropertyBuilder_Should_Set_PropertyEditorAlias()
    {
        // Arrange
        var builder = new PropertyBuilder(_mockShortStringHelper.Object);

        // Act
        var property = builder
            .SetAlias("richTextProperty")
            .SetName("Rich Text Property")
            .SetPropertyEditorAlias("Umbraco.RichText")
            .Build();

        // Assert
        property.PropertyEditorAlias.Should().Be("Umbraco.RichText");
    }

    [Fact]
    public void PropertyBuilder_Should_Set_LabelOnTop()
    {
        // Arrange
        var builder = new PropertyBuilder(_mockShortStringHelper.Object);

        // Act
        var property = builder
            .SetAlias("labelTopProperty")
            .SetName("Label Top Property")
            .SetLabelOnTop(true)
            .Build();

        // Assert
        property.LabelOnTop.Should().BeTrue();
    }

    [Fact]
    public void PropertyBuilder_Should_Chain_All_Configuration_Methods()
    {
        // Arrange
        var builder = new PropertyBuilder(_mockShortStringHelper.Object);

        // Act
        var property = builder
            .SetAlias("complexProperty")
            .SetName("Complex Property")
            .SetDescription("A complex property with all options")
            .SetPropertyEditorAlias("Umbraco.TextBox")
            .SetMandatory(true)
            .SetLabelOnTop(true)
            .SetValidation(@"^[A-Za-z]+$", "Letters only")
            .SetValueStorageType(ValueStorageType.Nvarchar)
            .SetSortOrder(10)
            .Build();

        // Assert
        property.Should().NotBeNull();
        property.Alias.Should().Be("complexProperty");
        property.Name.Should().Be("Complex Property");
        property.Description.Should().Be("A complex property with all options");
        property.PropertyEditorAlias.Should().Be("Umbraco.TextBox");
        property.Mandatory.Should().BeTrue();
        property.LabelOnTop.Should().BeTrue();
        property.ValidationRegExp.Should().Be(@"^[A-Za-z]+$");
        property.ValidationRegExpMessage.Should().Be("Letters only");
        property.ValueStorageType.Should().Be(ValueStorageType.Nvarchar);
        property.SortOrder.Should().Be(10);
    }

    [Fact]
    public void PropertyBuilder_Should_Have_Default_Values()
    {
        // Arrange
        var builder = new PropertyBuilder(_mockShortStringHelper.Object);

        // Act
        var property = builder.Build();

        // Assert
        property.Should().NotBeNull();
        property.Alias.Should().Be("defaultProperty");
        property.Name.Should().Be(string.Empty);
        property.PropertyEditorAlias.Should().Be("Umbraco.TextBox");
        property.Mandatory.Should().BeFalse();
        property.LabelOnTop.Should().BeFalse();
        property.ValueStorageType.Should().Be(ValueStorageType.Nvarchar);
        property.SortOrder.Should().Be(0);
    }

    [Theory]
    [InlineData("Umbraco.TextBox", ValueStorageType.Nvarchar)]
    [InlineData("Umbraco.TextArea", ValueStorageType.Nvarchar)]
    [InlineData("Umbraco.RichText", ValueStorageType.Ntext)]
    [InlineData("Umbraco.MediaPicker3", ValueStorageType.Ntext)]
    [InlineData("Umbraco.ContentPicker", ValueStorageType.Nvarchar)]
    [InlineData("Umbraco.Integer", ValueStorageType.Integer)]
    [InlineData("Umbraco.TrueFalse", ValueStorageType.Integer)]
    [InlineData("Umbraco.DateTime", ValueStorageType.Date)]
    public void PropertyBuilder_Should_Support_Different_Property_Types(string editorAlias, ValueStorageType expectedStorageType)
    {
        // Arrange
        var builder = new PropertyBuilder(_mockShortStringHelper.Object);

        // Act
        var property = builder
            .SetAlias($"test{editorAlias.Replace(".", "")}")
            .SetName($"Test {editorAlias}")
            .SetPropertyEditorAlias(editorAlias)
            .SetValueStorageType(expectedStorageType)
            .Build();

        // Assert
        property.PropertyEditorAlias.Should().Be(editorAlias);
        property.ValueStorageType.Should().Be(expectedStorageType);
    }

    [Fact]
    public void PropertyBuilder_Should_Handle_Null_Values_Gracefully()
    {
        // Arrange
        var builder = new PropertyBuilder(_mockShortStringHelper.Object);

        // Act
        var property = builder
            .SetAlias("nullTestProperty")
            .SetName("Null Test Property")
            .SetDescription(null!)
            .SetValidation(null!, null!)
            .Build();

        // Assert
        property.Should().NotBeNull();
        property.Description.Should().BeEmpty();
        property.ValidationRegExp.Should().BeEmpty();
        property.ValidationRegExpMessage.Should().BeEmpty();
    }
}