using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;
using Umbraco.DocTypeBuilder;
using FluentAssertions;
using Moq;

namespace Umbraco.DocTypeBuilder.Tests;

/// <summary>
/// Integration tests for PropertyBuilder using xUnit.
/// 
/// These tests focus on demonstrating the PropertyBuilder functionality
/// by creating PropertyType objects and testing their configuration.
/// While they don't use a full Umbraco database, they test with real
/// Umbraco types and services to ensure compatibility.
/// </summary>
public class PropertyBuilderIntegrationTests
{
    [Fact]
    public void PropertyBuilder_Should_CreatePropertyTypeWithCorrectConfiguration()
    {
        // Arrange
        var shortStringHelper = CreateMockShortStringHelper();
        const string propertyName = "Integration Test Property";
        const string propertyAlias = "integrationTestProperty";
        const string editorAlias = "Umbraco.TextBox";

        // Act
        var propertyBuilder = new PropertyBuilder(propertyName, propertyAlias, editorAlias, shortStringHelper)
            .WithDescription("A property created for integration testing")
            .IsMandatory(true)
            .WithValueStorageType(ValueStorageType.Nvarchar)
            .WithSortOrder(5)
            .WithDataTypeDefinitionId(123);

        var property = propertyBuilder.Build();

        // Assert - Verify all PropertyBuilder settings are applied correctly
        property.Should().NotBeNull("because PropertyBuilder should create a valid PropertyType");
        property.Name.Should().Be(propertyName, "because the property name should be set correctly");
        property.PropertyEditorAlias.Should().Be(editorAlias, "because the editor alias should be set correctly");
        property.Description.Should().Be("A property created for integration testing", "because the description should be set");
        property.Mandatory.Should().BeTrue("because we set it as mandatory");
        property.ValueStorageType.Should().Be(ValueStorageType.Nvarchar, "because we specified Nvarchar storage");
        property.SortOrder.Should().Be(5, "because we set the sort order to 5");
        property.DataTypeId.Should().Be(123, "because we specified data type ID 123");
        
        // Verify PropertyType is a real Umbraco type
        property.Should().BeOfType<PropertyType>("because PropertyBuilder should create a real Umbraco PropertyType");
        property.Id.Should().Be(0, "because new PropertyTypes start with ID 0 until saved to database");
    }

    [Fact]
    public void PropertyBuilder_Should_CreateMultiplePropertiesWithDifferentConfigurations()
    {
        // Arrange
        var shortStringHelper = CreateMockShortStringHelper();

        // Act - Create three different properties using PropertyBuilder
        var titleProperty = new PropertyBuilder("Title", "title", "Umbraco.TextBox", shortStringHelper)
            .WithDescription("Page title")
            .IsMandatory(true)
            .WithSortOrder(1)
            .WithValueStorageType(ValueStorageType.Nvarchar)
            .Build();

        var bodyProperty = new PropertyBuilder("Body", "body", "Umbraco.TextArea", shortStringHelper)
            .WithDescription("Page content")
            .IsMandatory(false)
            .WithSortOrder(2)
            .WithValueStorageType(ValueStorageType.Ntext)
            .Build();

        var isPublishedProperty = new PropertyBuilder("Is Published", "isPublished", "Umbraco.TrueFalse", shortStringHelper)
            .WithDescription("Should this page be visible?")
            .IsMandatory(false)
            .WithSortOrder(3)
            .WithValueStorageType(ValueStorageType.Integer)
            .Build();

        // Assert - Verify each property has the correct configuration
        // Title property assertions
        titleProperty.Name.Should().Be("Title");
        titleProperty.PropertyEditorAlias.Should().Be("Umbraco.TextBox");
        titleProperty.Mandatory.Should().BeTrue();
        titleProperty.SortOrder.Should().Be(1);
        titleProperty.ValueStorageType.Should().Be(ValueStorageType.Nvarchar);
        titleProperty.Description.Should().Be("Page title");

        // Body property assertions
        bodyProperty.Name.Should().Be("Body");
        bodyProperty.PropertyEditorAlias.Should().Be("Umbraco.TextArea");
        bodyProperty.Mandatory.Should().BeFalse();
        bodyProperty.SortOrder.Should().Be(2);
        bodyProperty.ValueStorageType.Should().Be(ValueStorageType.Ntext);
        bodyProperty.Description.Should().Be("Page content");

        // Is Published property assertions
        isPublishedProperty.Name.Should().Be("Is Published");
        isPublishedProperty.PropertyEditorAlias.Should().Be("Umbraco.TrueFalse");
        isPublishedProperty.Mandatory.Should().BeFalse();
        isPublishedProperty.SortOrder.Should().Be(3);
        isPublishedProperty.ValueStorageType.Should().Be(ValueStorageType.Integer);
        isPublishedProperty.Description.Should().Be("Should this page be visible?");

        // Verify all are valid PropertyType objects
        titleProperty.Should().BeOfType<PropertyType>();
        bodyProperty.Should().BeOfType<PropertyType>();
        isPublishedProperty.Should().BeOfType<PropertyType>();
    }

    [Fact]
    public void PropertyBuilder_Should_CreatePropertyTypeCompatibleWithContentType()
    {
        // Arrange
        var shortStringHelper = CreateMockShortStringHelper();

        // Act - Create a property and add it to a ContentType
        var property = new PropertyBuilder("Content Property", "contentProperty", "Umbraco.TextBox", shortStringHelper)
            .WithDescription("A property for demonstrating ContentType compatibility")
            .IsMandatory(true)
            .WithSortOrder(1)
            .Build();

        // Create a ContentType and add our PropertyBuilder-created property
        var contentType = new ContentType(shortStringHelper, -1)
        {
            Alias = "integrationTestContentType",
            Name = "Integration Test Content Type",
            Description = "Testing PropertyBuilder integration with ContentType"
        };

        var propertyGroup = new PropertyGroup(new PropertyTypeCollection(true))
        {
            Alias = "content",
            Name = "Content",
            SortOrder = 1
        };

        // Add PropertyBuilder-created property to the property group
        propertyGroup.PropertyTypes!.Add(property);
        contentType.PropertyGroups.Add(propertyGroup);

        // Assert - Verify the property is correctly integrated into ContentType
        contentType.Should().NotBeNull("because we created a valid ContentType");
        contentType.PropertyGroups.Should().HaveCount(1, "because we added one property group");
        
        var retrievedPropertyGroup = contentType.PropertyGroups.First();
        retrievedPropertyGroup.PropertyTypes.Should().HaveCount(1, "because we added one property");
        
        var retrievedProperty = retrievedPropertyGroup.PropertyTypes!.First();
        retrievedProperty.Should().Be(property, "because it should be the exact same PropertyType object");
        retrievedProperty.Name.Should().Be("Content Property", "because that's what we named it");
        retrievedProperty.PropertyEditorAlias.Should().Be("Umbraco.TextBox", "because that's the editor we specified");
        retrievedProperty.Mandatory.Should().BeTrue("because we set it as mandatory");
        retrievedProperty.SortOrder.Should().Be(1, "because we set the sort order to 1");
        retrievedProperty.Description.Should().Be("A property for demonstrating ContentType compatibility");
    }

    [Fact]
    public void PropertyBuilder_Should_HandleDifferentPropertyEditorTypes()
    {
        // Arrange
        var shortStringHelper = CreateMockShortStringHelper();

        // Act - Create properties with different common Umbraco editor types
        var properties = new Dictionary<string, PropertyType>
        {
            ["TextBox"] = new PropertyBuilder("Text", "text", "Umbraco.TextBox", shortStringHelper)
                .WithValueStorageType(ValueStorageType.Nvarchar)
                .Build(),
                
            ["TextArea"] = new PropertyBuilder("TextArea", "textArea", "Umbraco.TextArea", shortStringHelper)
                .WithValueStorageType(ValueStorageType.Ntext)
                .Build(),
                
            ["TrueFalse"] = new PropertyBuilder("Boolean", "boolean", "Umbraco.TrueFalse", shortStringHelper)
                .WithValueStorageType(ValueStorageType.Integer)
                .Build(),
                
            ["Numeric"] = new PropertyBuilder("Number", "number", "Umbraco.Integer", shortStringHelper)
                .WithValueStorageType(ValueStorageType.Integer)
                .Build(),
                
            ["DateTime"] = new PropertyBuilder("Date", "date", "Umbraco.DateTime", shortStringHelper)
                .WithValueStorageType(ValueStorageType.Date)
                .Build()
        };

        // Assert - Verify each property has the correct editor alias and storage type
        properties["TextBox"].PropertyEditorAlias.Should().Be("Umbraco.TextBox");
        properties["TextBox"].ValueStorageType.Should().Be(ValueStorageType.Nvarchar);
        
        properties["TextArea"].PropertyEditorAlias.Should().Be("Umbraco.TextArea");
        properties["TextArea"].ValueStorageType.Should().Be(ValueStorageType.Ntext);
        
        properties["TrueFalse"].PropertyEditorAlias.Should().Be("Umbraco.TrueFalse");
        properties["TrueFalse"].ValueStorageType.Should().Be(ValueStorageType.Integer);
        
        properties["Numeric"].PropertyEditorAlias.Should().Be("Umbraco.Integer");
        properties["Numeric"].ValueStorageType.Should().Be(ValueStorageType.Integer);
        
        properties["DateTime"].PropertyEditorAlias.Should().Be("Umbraco.DateTime");
        properties["DateTime"].ValueStorageType.Should().Be(ValueStorageType.Date);

        // Verify all are valid PropertyType objects
        foreach (var kvp in properties)
        {
            kvp.Value.Should().BeOfType<PropertyType>($"because {kvp.Key} should create a valid PropertyType");
            kvp.Value.Id.Should().Be(0, $"because new {kvp.Key} PropertyTypes start with ID 0");
        }
    }

    [Fact]
    public void PropertyBuilder_Should_WorkWithFluentApiChaining()
    {
        // Arrange
        var shortStringHelper = CreateMockShortStringHelper();

        // Act - Demonstrate fluent API chaining
        var property = new PropertyBuilder("Chained Property", "chainedProperty", "Umbraco.TextBox", shortStringHelper)
            .WithDescription("Property created using fluent API")
            .IsMandatory(true)
            .WithValueStorageType(ValueStorageType.Nvarchar)
            .WithSortOrder(10)
            .WithDataTypeDefinitionId(456)
            .Build();

        // Assert
        property.Should().NotBeNull("because fluent API should create a valid PropertyType");
        property.Name.Should().Be("Chained Property", "because fluent API should preserve the property name");
        property.Description.Should().Be("Property created using fluent API", "because description should be set via fluent API");
        property.Mandatory.Should().BeTrue("because mandatory was set via fluent API");
        property.ValueStorageType.Should().Be(ValueStorageType.Nvarchar, "because storage type was set via fluent API");
        property.SortOrder.Should().Be(10, "because sort order was set via fluent API");
        property.DataTypeId.Should().Be(456, "because data type ID was set via fluent API");
        property.PropertyEditorAlias.Should().Be("Umbraco.TextBox", "because editor alias should be preserved");
    }

    /// <summary>
    /// Creates a mock IShortStringHelper for testing purposes.
    /// This simulates the Umbraco string cleaning behavior.
    /// </summary>
    private IShortStringHelper CreateMockShortStringHelper()
    {
        var mockShortStringHelper = new Mock<IShortStringHelper>();
        
        // Set up the CleanString method to behave like Umbraco's implementation
        mockShortStringHelper.Setup(x => x.CleanString(It.IsAny<string>(), It.IsAny<CleanStringType>()))
                           .Returns<string, CleanStringType>((input, type) => 
                           {
                               if (string.IsNullOrEmpty(input)) return string.Empty;
                               
                               // Simple alias cleaning - lowercase, remove spaces and special chars
                               return input.ToLowerInvariant()
                                         .Replace(" ", "")
                                         .Replace("-", "")
                                         .Replace("_", "");
                           });

        return mockShortStringHelper.Object;
    }
}

/// <summary>
/// No-op hosted service to prevent background services from interfering with tests
/// </summary>
public class NoopHostedService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}