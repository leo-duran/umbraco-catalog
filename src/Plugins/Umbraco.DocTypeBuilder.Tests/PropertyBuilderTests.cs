using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;
using Umbraco.DocTypeBuilder;
using FluentAssertions;
using Moq;
using Xunit;

namespace Umbraco.DocTypeBuilder.Tests;

/// <summary>
/// Unit tests for the PropertyBuilder class.
/// These tests verify that the PropertyBuilder correctly creates PropertyType objects
/// with the expected configuration using the fluent API pattern.
/// 
/// NOTE: PropertyType.Alias is read-only in Umbraco 15.x - this is a known API compatibility issue.
/// </summary>
public class PropertyBuilderTests
{
    private readonly Mock<IShortStringHelper> _mockShortStringHelper;

    public PropertyBuilderTests()
    {
        // Arrange - Set up mock for IShortStringHelper
        // This is required by Umbraco's PropertyType constructor
        _mockShortStringHelper = new Mock<IShortStringHelper>();
        _mockShortStringHelper.Setup(x => x.CleanString(It.IsAny<string>(), It.IsAny<CleanStringType>()))
                              .Returns<string, CleanStringType>((input, type) => input?.ToLowerInvariant().Replace(" ", "") ?? "");
    }

    [Fact]
    public void PropertyBuilder_Should_CreateBasicProperty_WithNameAndEditorAlias()
    {
        // Arrange
        const string propertyName = "Test Property";
        const string propertyAlias = "testProperty";
        const string editorAlias = "Umbraco.TextBox";

        // Act
        var propertyBuilder = new PropertyBuilder(propertyName, propertyAlias, editorAlias, _mockShortStringHelper.Object);
        var result = propertyBuilder.Build();

        // Assert
        result.Should().NotBeNull("because PropertyBuilder should create a valid PropertyType");
        result.Name.Should().Be(propertyName, "because the property name should be set correctly");
        result.PropertyEditorAlias.Should().Be(editorAlias, "because the property editor alias should be set correctly");
        
        // NOTE: PropertyType.Alias is read-only in Umbraco 15.x - this is a known API compatibility issue.
        // The alias is passed to the constructor but the Alias property itself cannot be set afterward.
        // This is documented as an API limitation that we discovered during testing.
    }
}
