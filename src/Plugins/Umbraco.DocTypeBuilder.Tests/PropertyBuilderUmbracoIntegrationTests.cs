using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.DocTypeBuilder;
using FluentAssertions;
using Moq;

namespace Umbraco.DocTypeBuilder.Tests;

/// <summary>
/// Simplified integration tests that verify PropertyBuilder works correctly with Umbraco types.
/// These tests focus on the integration between our PropertyBuilder and Umbraco's model classes.
/// </summary>
public class PropertyBuilderUmbracoIntegrationTests
{
    private readonly Mock<IShortStringHelper> _mockShortStringHelper;

    public PropertyBuilderUmbracoIntegrationTests()
    {
        _mockShortStringHelper = new Mock<IShortStringHelper>();
        _mockShortStringHelper.Setup(x => x.CleanString(It.IsAny<string>(), It.IsAny<CleanStringType>()))
                              .Returns<string, CleanStringType>((input, type) => input?.ToLowerInvariant().Replace(" ", "") ?? "");
    }

    [Fact]
    public void PropertyBuilder_Should_CreatePropertyType_ThatIntegratesWithContentType()
    {
        // Arrange
        const string propertyName = "Integration Test Property";
        const string propertyAlias = "integrationTestProperty";
        const string editorAlias = "Umbraco.TextBox";

        // Act - Create PropertyType using our PropertyBuilder
        var propertyBuilder = new PropertyBuilder(propertyName, propertyAlias, editorAlias, _mockShortStringHelper.Object)
            .WithDescription("A property created for integration testing")
            .IsMandatory(true)
            .WithValueStorageType(ValueStorageType.Nvarchar);

        var property = propertyBuilder.Build();

        // Create a ContentType and add our PropertyBuilder-created property
        var contentType = new ContentType(_mockShortStringHelper.Object, -1)
        {
            Alias = "integrationTestType",
            Name = "Integration Test Document Type",
            Description = "A document type for testing PropertyBuilder integration",
            Icon = "icon-document",
            AllowedAsRoot = true
        };

        // Add property to a property group
        var propertyGroup = new PropertyGroup(new PropertyTypeCollection(true))
        {
            Alias = "content",
            Name = "Content",
            SortOrder = 1
        };

        propertyGroup.PropertyTypes!.Add(property);
        contentType.PropertyGroups.Add(propertyGroup);

        // Assert - Verify the PropertyBuilder-created property integrates correctly
        property.Should().NotBeNull("because PropertyBuilder should create a valid PropertyType");
        property.Name.Should().Be(propertyName, "because the property name should be preserved");
        property.PropertyEditorAlias.Should().Be(editorAlias, "because the editor alias should be preserved");
        property.Mandatory.Should().BeTrue("because we set the property as mandatory");
        property.ValueStorageType.Should().Be(ValueStorageType.Nvarchar, "because we specified Nvarchar storage");

        // Verify the property integrates with ContentType
        contentType.PropertyGroups.Should().HaveCount(1, "because we added one property group");
        var addedPropertyGroup = contentType.PropertyGroups.First();
        addedPropertyGroup.PropertyTypes.Should().HaveCount(1, "because we added one property");
        
        var addedProperty = addedPropertyGroup.PropertyTypes!.First();
        addedProperty.Should().BeSameAs(property, "because the same PropertyType instance should be added");
        addedProperty.Name.Should().Be(propertyName, "because the property name should match");
        addedProperty.PropertyEditorAlias.Should().Be(editorAlias, "because the editor alias should match");
    }

    [Fact]
    public void PropertyBuilder_Should_CreateMultiplePropertiesWithDifferentTypes_ThatIntegrateCorrectly()
    {
        // Arrange - Create multiple properties with different types
        var titleProperty = new PropertyBuilder("Title", "title", "Umbraco.TextBox", _mockShortStringHelper.Object)
            .WithDescription("The main title")
            .IsMandatory(true)
            .WithSortOrder(1)
            .Build();

        var bodyProperty = new PropertyBuilder("Body Content", "bodyContent", "Umbraco.TinyMCE", _mockShortStringHelper.Object)
            .WithDescription("The main content area")
            .IsMandatory(false)
            .WithSortOrder(2)
            .Build();

        var isPublishedProperty = new PropertyBuilder("Is Published", "isPublished", "Umbraco.TrueFalse", _mockShortStringHelper.Object)
            .WithDescription("Should this content be visible?")
            .IsMandatory(false)
            .WithSortOrder(3)
            .Build();

        var publishDateProperty = new PropertyBuilder("Publish Date", "publishDate", "Umbraco.DateTime", _mockShortStringHelper.Object)
            .WithDescription("When should this be published?")
            .IsMandatory(false)
            .WithSortOrder(4)
            .Build();

        // Act - Create ContentType with multiple PropertyBuilder-created properties
        var contentType = new ContentType(_mockShortStringHelper.Object, -1)
        {
            Alias = "multiPropertyTestType",
            Name = "Multi Property Test",
            Description = "Testing multiple PropertyBuilder properties",
            Icon = "icon-layout",
            AllowedAsRoot = true
        };

        // Add properties to different property groups
        var contentGroup = new PropertyGroup(new PropertyTypeCollection(true))
        {
            Alias = "content",
            Name = "Content",
            SortOrder = 1
        };
        contentGroup.PropertyTypes!.Add(titleProperty);
        contentGroup.PropertyTypes!.Add(bodyProperty);

        var settingsGroup = new PropertyGroup(new PropertyTypeCollection(true))
        {
            Alias = "settings",
            Name = "Settings",
            SortOrder = 2
        };
        settingsGroup.PropertyTypes!.Add(isPublishedProperty);
        settingsGroup.PropertyTypes!.Add(publishDateProperty);

        contentType.PropertyGroups.Add(contentGroup);
        contentType.PropertyGroups.Add(settingsGroup);

        // Assert - Verify all properties integrate correctly
        contentType.PropertyGroups.Should().HaveCount(2, "because we added two property groups");

        // Verify Content group
        var contentTab = contentType.PropertyGroups.Single(pg => pg.Name == "Content");
        contentTab.PropertyTypes.Should().HaveCount(2, "because we added two properties to Content group");
        
        var savedTitleProperty = contentTab.PropertyTypes!.Single(p => p.Name == "Title");
        savedTitleProperty.PropertyEditorAlias.Should().Be("Umbraco.TextBox");
        savedTitleProperty.Mandatory.Should().BeTrue();
        savedTitleProperty.SortOrder.Should().Be(1);

        var savedBodyProperty = contentTab.PropertyTypes!.Single(p => p.Name == "Body Content");
        savedBodyProperty.PropertyEditorAlias.Should().Be("Umbraco.TinyMCE");
        savedBodyProperty.Mandatory.Should().BeFalse();
        savedBodyProperty.SortOrder.Should().Be(2);

        // Verify Settings group
        var settingsTab = contentType.PropertyGroups.Single(pg => pg.Name == "Settings");
        settingsTab.PropertyTypes.Should().HaveCount(2, "because we added two properties to Settings group");

        var savedIsPublishedProperty = settingsTab.PropertyTypes!.Single(p => p.Name == "Is Published");
        savedIsPublishedProperty.PropertyEditorAlias.Should().Be("Umbraco.TrueFalse");
        savedIsPublishedProperty.Mandatory.Should().BeFalse();
        savedIsPublishedProperty.SortOrder.Should().Be(3);

        var savedPublishDateProperty = settingsTab.PropertyTypes!.Single(p => p.Name == "Publish Date");
        savedPublishDateProperty.PropertyEditorAlias.Should().Be("Umbraco.DateTime");
        savedPublishDateProperty.Mandatory.Should().BeFalse();
        savedPublishDateProperty.SortOrder.Should().Be(4);
    }

    [Fact]
    public void PropertyBuilder_Should_CreatePropertiesWithDataTypeIds_ThatIntegrateCorrectly()
    {
        // Arrange
        const int customDataTypeId = 12345;
        const string propertyName = "Custom Data Type Property";
        const string propertyAlias = "customDataTypeProperty";
        const string editorAlias = "Umbraco.TextBox";

        // Act - Create property with specific data type ID
        var propertyBuilder = new PropertyBuilder(propertyName, propertyAlias, editorAlias, _mockShortStringHelper.Object)
            .WithDescription("Property using custom data type ID")
            .WithDataTypeDefinitionId(customDataTypeId)
            .IsMandatory(false);

        var property = propertyBuilder.Build();

        // Create ContentType with the property
        var contentType = new ContentType(_mockShortStringHelper.Object, -1)
        {
            Alias = "dataTypeTestType",
            Name = "Data Type Test",
            Description = "Testing PropertyBuilder with custom data type IDs"
        };

        var propertyGroup = new PropertyGroup(new PropertyTypeCollection(true))
        {
            Alias = "content",
            Name = "Content"
        };
        propertyGroup.PropertyTypes!.Add(property);

        contentType.PropertyGroups.Add(propertyGroup);

        // Assert - Verify the data type ID is properly set
        property.DataTypeId.Should().Be(customDataTypeId, "because we specified a custom data type ID");
        
        var addedProperty = contentType.PropertyGroups.First().PropertyTypes!.First();
        addedProperty.DataTypeId.Should().Be(customDataTypeId, "because the data type ID should be preserved when added to ContentType");
        addedProperty.Name.Should().Be(propertyName);
        addedProperty.PropertyEditorAlias.Should().Be(editorAlias);
    }

    [Fact]
    public void PropertyBuilder_Should_CreatePropertiesWithAllConfigurations_ThatIntegrateCorrectly()
    {
        // Arrange - Create a property with all possible configurations
        const string propertyName = "Fully Configured Property";
        const string propertyAlias = "fullyConfiguredProperty";
        const string editorAlias = "Umbraco.TextArea";
        const string description = "A property with all possible configurations set";
        const int dataTypeId = 98765;
        const int sortOrder = 42;

        // Act - Create fully configured property
        var propertyBuilder = new PropertyBuilder(propertyName, propertyAlias, editorAlias, _mockShortStringHelper.Object)
            .WithDescription(description)
            .WithDataTypeDefinitionId(dataTypeId)
            .IsMandatory(true)
            .WithSortOrder(sortOrder)
            .WithValueStorageType(ValueStorageType.Ntext)
            .WithLabelOnTop(true)
            .WithValidationRegex(@"^[A-Za-z0-9\s]+$");

        var property = propertyBuilder.Build();

        // Assert - Verify all configurations are correctly applied
        property.Name.Should().Be(propertyName, "because the property name should be preserved");
        property.PropertyEditorAlias.Should().Be(editorAlias, "because the editor alias should be preserved");
        property.Description.Should().Be(description, "because the description should be preserved");
        property.DataTypeId.Should().Be(dataTypeId, "because the data type ID should be preserved");
        property.Mandatory.Should().BeTrue("because we set the property as mandatory");
        property.SortOrder.Should().Be(sortOrder, "because we set a custom sort order");
        property.ValueStorageType.Should().Be(ValueStorageType.Ntext, "because we specified Ntext storage");
        property.LabelOnTop.Should().BeTrue("because we enabled label on top");
        property.ValidationRegExp.Should().Be(@"^[A-Za-z0-9\s]+$", "because we set a validation regex");

        // Verify it integrates with ContentType correctly
        var contentType = new ContentType(_mockShortStringHelper.Object, -1)
        {
            Alias = "fullyConfiguredTest",
            Name = "Fully Configured Test"
        };

        var propertyGroup = new PropertyGroup(new PropertyTypeCollection(true))
        {
            Alias = "content",
            Name = "Content"
        };
        propertyGroup.PropertyTypes!.Add(property);

        contentType.PropertyGroups.Add(propertyGroup);

        var addedProperty = contentType.PropertyGroups.First().PropertyTypes!.First();
        addedProperty.Should().BeSameAs(property, "because the same PropertyType instance should be referenced");
        addedProperty.Name.Should().Be(propertyName, "because all configurations should be preserved");
        addedProperty.Mandatory.Should().BeTrue("because the mandatory setting should be preserved");
        addedProperty.SortOrder.Should().Be(sortOrder, "because the sort order should be preserved");
    }
}