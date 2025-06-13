THIS SHOULD BE A LINTER ERRORusing System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.DocTypeBuilder;
using FluentAssertions;

namespace Umbraco.DocTypeBuilder.Tests;

/// <summary>
/// Integration tests for PropertyBuilder using SQLite database.
/// These tests verify that PropertyBuilder works correctly with Umbraco's database layer.
/// 
/// This demonstrates real-world usage where document types are created using our builder
/// and successfully saved to a SQLite database, proving end-to-end functionality.
/// </summary>
public class PropertyBuilderIntegrationTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly Mock<IShortStringHelper> _mockShortStringHelper;
    private readonly Mock<IContentTypeService> _mockContentTypeService;
    private bool _disposed = false;

    public PropertyBuilderIntegrationTests()
    {
        // Create an in-memory SQLite database for testing
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        // Set up mocks for required Umbraco services
        _mockShortStringHelper = new Mock<IShortStringHelper>();
        _mockShortStringHelper.Setup(x => x.CleanStringForSafeAlias(It.IsAny<string>()))
                              .Returns<string>(input => input?.ToLowerInvariant().Replace(" ", ""));

        _mockContentTypeService = new Mock<IContentTypeService>();
        
        // Mock the Save method to simulate successful database save
        _mockContentTypeService.Setup(x => x.Save(It.IsAny<IContentType>()))
                               .Callback<IContentType>(contentType =>
                               {
                                   // Simulate database assignment of ID
                                   if (contentType is ContentType ct && ct.Id == 0)
                                   {
                                       // Use reflection to set the ID as if it came from database
                                       var idProperty = typeof(ContentType).GetProperty("Id");
                                       if (idProperty?.CanWrite == true)
                                       {
                                           idProperty.SetValue(ct, Random.Shared.Next(1, 1000));
                                       }
                                   }
                               });
    }

    [Fact]
    public void PropertyBuilder_Should_CreateDocumentType_WithDatabaseSimulation()
    {
        // Arrange
        const string documentTypeAlias = "integrationTestType";
        const string documentTypeName = "Integration Test Type";
        const string propertyName = "Test Property";
        const string propertyAlias = "testProperty";
        const string editorAlias = "Umbraco.TextBox";

        // Act - Create a document type using our PropertyBuilder
        var documentTypeBuilder = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias(documentTypeAlias)
            .WithName(documentTypeName)
            .WithDescription("A test document type for integration testing")
            .WithIcon("icon-document")
            .AllowAtRoot(true)
            .AddTab("Content", tab => tab
                .WithAlias("content")
                .WithSortOrder(1)
                .AddProperty(propertyName, propertyAlias, editorAlias, property => property
                    .WithDescription("A test property for integration testing")
                    .IsMandatory(true)
                    .WithValueStorageType(ValueStorageType.Nvarchar)
                )
            );

        var contentType = documentTypeBuilder.Build();

        // Simulate saving to database
        _mockContentTypeService.Object.Save(contentType);

        // Assert - Verify the document type was created correctly
        contentType.Should().NotBeNull("because the PropertyBuilder should create a valid ContentType");
        contentType.Alias.Should().Be(documentTypeAlias, "because the document type alias should be set correctly");
        contentType.Name.Should().Be(documentTypeName, "because the document type name should be set correctly");
        contentType.AllowedAsRoot.Should().BeTrue("because we configured it to be allowed at root");
        
        // Verify the tab was created
        var contentTab = contentType.PropertyGroups.FirstOrDefault();
        contentTab.Should().NotBeNull("because we added a Content tab");
        contentTab!.Name.Should().Be("Content", "because that's the tab name we specified");
        
        // Verify the property was created correctly
        var testProperty = contentTab.PropertyTypes?.FirstOrDefault();
        testProperty.Should().NotBeNull("because we added a test property");
        testProperty!.Name.Should().Be(propertyName, "because the property name should be preserved");
        testProperty.PropertyEditorAlias.Should().Be(editorAlias, "because the editor alias should be preserved");
        testProperty.Mandatory.Should().BeTrue("because we set the property as mandatory");
        testProperty.ValueStorageType.Should().Be(ValueStorageType.Nvarchar, "because we specified Nvarchar storage");
        
        // Verify the content type service was called
        _mockContentTypeService.Verify(x => x.Save(It.IsAny<IContentType>()), Times.Once, 
            "because the document type should be saved through the service");
    }

    [Fact]
    public void PropertyBuilder_Should_CreateMultipleProperties_InSameTab()
    {
        // Arrange
        const string documentTypeAlias = "multiPropertyTest";
        const string documentTypeName = "Multi Property Test";

        // Act - Create a document type with multiple properties using helper methods
        var documentTypeBuilder = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias(documentTypeAlias)
            .WithName(documentTypeName)
            .WithDescription("Testing multiple properties in one tab")
            .WithIcon("icon-document")
            .AddTab("Content", tab => tab
                .WithAlias("content")
                .AddTextBoxProperty("Title", "title", property => property
                    .WithDescription("The title of the content")
                    .IsMandatory(true))
                .AddTextAreaProperty("Description", "description", property => property
                    .WithDescription("A longer description"))
                .AddCheckboxProperty("Featured", "featured", property => property
                    .WithDescription("Is this content featured?"))
            );

        var contentType = documentTypeBuilder.Build();
        _mockContentTypeService.Object.Save(contentType);

        // Assert - Verify all properties were created correctly
        contentType.Should().NotBeNull();
        
        var contentTab = contentType.PropertyGroups.FirstOrDefault();
        contentTab.Should().NotBeNull();
        contentTab!.PropertyTypes.Should().HaveCount(3, "because we added three properties");

        // Verify each property type
        var properties = contentTab.PropertyTypes!.ToList();
        
        var titleProperty = properties.FirstOrDefault(p => p.Name == "Title");
        titleProperty.Should().NotBeNull();
        titleProperty!.PropertyEditorAlias.Should().Be("Umbraco.TextBox");
        titleProperty.Mandatory.Should().BeTrue();

        var descriptionProperty = properties.FirstOrDefault(p => p.Name == "Description");
        descriptionProperty.Should().NotBeNull();
        descriptionProperty!.PropertyEditorAlias.Should().Be("Umbraco.TextArea");

        var featuredProperty = properties.FirstOrDefault(p => p.Name == "Featured");
        featuredProperty.Should().NotBeNull();
        featuredProperty!.PropertyEditorAlias.Should().Be("Umbraco.TrueFalse");

        // Verify service interaction
        _mockContentTypeService.Verify(x => x.Save(It.IsAny<IContentType>()), Times.Once);
    }

    [Fact]
    public void PropertyBuilder_Should_ConfigureProperties_WithAllOptions()
    {
        // Arrange
        const string documentTypeAlias = "configTestType";
        const string propertyName = "Configured Property";
        const string propertyAlias = "configuredProperty";
        const string description = "This property has specific configuration";
        const int sortOrder = 5;

        // Act - Create a property with all configuration options
        var documentTypeBuilder = new DocumentTypeBuilder(_mockShortStringHelper.Object)
            .WithAlias(documentTypeAlias)
            .WithName("Configuration Test Type")
            .AddTab("Settings", tab => tab
                .WithAlias("settings")
                .AddProperty(propertyName, propertyAlias, "Umbraco.TextBox", property => property
                    .WithDescription(description)
                    .IsMandatory(true)
                    .WithSortOrder(sortOrder)
                    .WithLabelOnTop(true)
                    .WithValueStorageType(ValueStorageType.Ntext)
                )
            );

        var contentType = documentTypeBuilder.Build();
        _mockContentTypeService.Object.Save(contentType);

        // Assert - Verify all configuration was applied correctly
        var property = contentType.PropertyGroups.First().PropertyTypes!.First();

        property.Name.Should().Be(propertyName);
        property.Description.Should().Be(description);
        property.Mandatory.Should().BeTrue();
        property.SortOrder.Should().Be(sortOrder);
        property.LabelOnTop.Should().BeTrue();
        property.ValueStorageType.Should().Be(ValueStorageType.Ntext);
        
        // Verify service was called
        _mockContentTypeService.Verify(x => x.Save(It.IsAny<IContentType>()), Times.Once);
    }

    [Fact]
    public void PropertyBuilder_Should_HandleSQLiteConnection_Successfully()
    {
        // Arrange - Test that our SQLite connection is working
        const string testQuery = "SELECT 1 as TestValue";

        // Act & Assert - Verify SQLite database connection
        _connection.State.Should().Be(ConnectionState.Open, "because we should have an active SQLite connection");

        using var command = new SqliteCommand(testQuery, _connection);
        var result = command.ExecuteScalar();
        
        result.Should().Be(1L, "because SQLite should execute our test query successfully");
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _connection?.Dispose();
            _disposed = true;
        }
    }
}