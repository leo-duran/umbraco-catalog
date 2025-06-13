using Umbraco.Cms.Tests.Integration.Testing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Core.Models;
using Umbraco.DocTypeBuilder;
using FluentAssertions;

namespace Umbraco.DocTypeBuilder.Tests;

/// <summary>
/// Real integration tests that use actual Umbraco services and database operations.
/// These tests verify PropertyBuilder works correctly with the full Umbraco stack:
/// - Real IContentTypeService
/// - Real IShortStringHelper
/// - Real database saves and retrieves
/// - No mocks - testing actual end-to-end functionality
/// </summary>
[TestFixture]
[UmbracoTest(Database = UmbracoTestOptions.Database.NewSchemaPerTest)]
public class PropertyBuilderRealIntegrationTests : UmbracoIntegrationTest
{
    [Test]
    public void PropertyBuilder_Should_CreateAndSaveDocumentType_WithRealUmbracoServices()
    {
        // Arrange - Get real Umbraco services (no mocks!)
        var contentTypeService = GetRequiredService<IContentTypeService>();
        var shortStringHelper = GetRequiredService<IShortStringHelper>();

        const string documentTypeAlias = "realIntegrationTest";
        const string documentTypeName = "Real Integration Test";
        const string propertyName = "Test Property";
        const string propertyAlias = "testProperty";
        const string editorAlias = "Umbraco.TextBox";

        // Act - Create PropertyType using PropertyBuilder with real services
        var propertyBuilder = new PropertyBuilder(propertyName, propertyAlias, editorAlias, shortStringHelper)
            .WithDescription("A test property for real Umbraco integration testing")
            .IsMandatory(true)
            .WithValueStorageType(ValueStorageType.Nvarchar);

        var property = propertyBuilder.Build();

        // Create ContentType and add our PropertyBuilder-created property
        var contentType = new ContentType(shortStringHelper, -1)
        {
            Alias = documentTypeAlias,
            Name = documentTypeName,
            Description = "A test document type using real Umbraco services",
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

        // Save to real Umbraco database using real service
        contentTypeService.Save(contentType);

        // Assert - Retrieve from real Umbraco database and verify
        var savedContentType = contentTypeService.Get(documentTypeAlias);

        // Verify the document type was saved correctly
        savedContentType.Should().NotBeNull("because the document type should be saved to Umbraco database");
        savedContentType!.Name.Should().Be(documentTypeName, "because the document type name should be preserved");
        savedContentType.AllowedAsRoot.Should().BeTrue("because we configured it to be allowed at root");
        savedContentType.Id.Should().BeGreaterThan(0, "because saved content types should have database IDs");

        // Verify the property group was created in the database
        var savedPropertyGroup = savedContentType.PropertyGroups.FirstOrDefault();
        savedPropertyGroup.Should().NotBeNull("because we added a Content property group");
        savedPropertyGroup!.Name.Should().Be("Content", "because that's the property group name we specified");
        savedPropertyGroup.Id.Should().BeGreaterThan(0, "because saved property groups should have database IDs");

        // Verify the PropertyBuilder-created property was saved correctly
        var savedProperty = savedPropertyGroup.PropertyTypes?.FirstOrDefault();
        savedProperty.Should().NotBeNull("because we added a test property");
        savedProperty!.Name.Should().Be(propertyName, "because the property name should be preserved in database");
        savedProperty.PropertyEditorAlias.Should().Be(editorAlias, "because the editor alias should be preserved in database");
        savedProperty.Mandatory.Should().BeTrue("because we set the property as mandatory");
        savedProperty.ValueStorageType.Should().Be(ValueStorageType.Nvarchar, "because we specified Nvarchar storage");
        savedProperty.Id.Should().BeGreaterThan(0, "because saved properties should have database IDs");
        savedProperty.Description.Should().Be("A test property for real Umbraco integration testing", "because the description should be persisted");

        // Clean up - delete the test content type
        contentTypeService.Delete(savedContentType);
    }

    [Test] 
    public void PropertyBuilder_Should_CreateMultiplePropertiesAndSaveCorrectly_WithRealDatabase()
    {
        // Arrange
        var contentTypeService = GetRequiredService<IContentTypeService>();
        var shortStringHelper = GetRequiredService<IShortStringHelper>();

        const string documentTypeAlias = "multiPropertyRealTest";
        const string documentTypeName = "Multi Property Real Test";

        // Act - Create multiple properties using PropertyBuilder
        var titleProperty = new PropertyBuilder("Title", "title", "Umbraco.TextBox", shortStringHelper)
            .WithDescription("The main title")
            .IsMandatory(true)
            .WithSortOrder(1)
            .Build();

        var descriptionProperty = new PropertyBuilder("Description", "description", "Umbraco.TextArea", shortStringHelper)
            .WithDescription("A brief description")
            .IsMandatory(false)
            .WithSortOrder(2)
            .Build();

        var isPublishedProperty = new PropertyBuilder("Is Published", "isPublished", "Umbraco.TrueFalse", shortStringHelper)
            .WithDescription("Should this content be visible?")
            .IsMandatory(false)
            .WithSortOrder(3)
            .WithValueStorageType(ValueStorageType.Integer)
            .Build();

        // Create ContentType with multiple PropertyBuilder-created properties
        var contentType = new ContentType(shortStringHelper, -1)
        {
            Alias = documentTypeAlias,
            Name = documentTypeName,
            Description = "Testing multiple PropertyBuilder properties with real database",
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
        contentGroup.PropertyTypes!.Add(descriptionProperty);

        var settingsGroup = new PropertyGroup(new PropertyTypeCollection(true))
        {
            Alias = "settings", 
            Name = "Settings",
            SortOrder = 2
        };
        settingsGroup.PropertyTypes!.Add(isPublishedProperty);

        contentType.PropertyGroups.Add(contentGroup);
        contentType.PropertyGroups.Add(settingsGroup);

        // Save to real Umbraco database
        contentTypeService.Save(contentType);

        // Assert - Retrieve from database and verify everything was saved correctly
        var savedContentType = contentTypeService.Get(documentTypeAlias);
        savedContentType.Should().NotBeNull("because the document type should be saved successfully");
        savedContentType!.PropertyGroups.Should().HaveCount(2, "because we added two property groups");

        // Verify Content group and its properties
        var savedContentGroup = savedContentType.PropertyGroups.Single(pg => pg.Name == "Content");
        savedContentGroup.PropertyTypes.Should().HaveCount(2, "because we added two properties to Content group");
        savedContentGroup.Id.Should().BeGreaterThan(0, "because the property group should have a database ID");

        var savedTitleProperty = savedContentGroup.PropertyTypes!.Single(p => p.Name == "Title");
        savedTitleProperty.PropertyEditorAlias.Should().Be("Umbraco.TextBox");
        savedTitleProperty.Mandatory.Should().BeTrue();
        savedTitleProperty.SortOrder.Should().Be(1);
        savedTitleProperty.Id.Should().BeGreaterThan(0, "because saved properties should have database IDs");

        var savedDescriptionProperty = savedContentGroup.PropertyTypes!.Single(p => p.Name == "Description");
        savedDescriptionProperty.PropertyEditorAlias.Should().Be("Umbraco.TextArea");
        savedDescriptionProperty.Mandatory.Should().BeFalse();
        savedDescriptionProperty.SortOrder.Should().Be(2);
        savedDescriptionProperty.Id.Should().BeGreaterThan(0, "because saved properties should have database IDs");

        // Verify Settings group and its properties
        var savedSettingsGroup = savedContentType.PropertyGroups.Single(pg => pg.Name == "Settings");
        savedSettingsGroup.PropertyTypes.Should().HaveCount(1, "because we added one property to Settings group");
        savedSettingsGroup.Id.Should().BeGreaterThan(0, "because the property group should have a database ID");

        var savedIsPublishedProperty = savedSettingsGroup.PropertyTypes!.Single(p => p.Name == "Is Published");
        savedIsPublishedProperty.PropertyEditorAlias.Should().Be("Umbraco.TrueFalse");
        savedIsPublishedProperty.Mandatory.Should().BeFalse();
        savedIsPublishedProperty.SortOrder.Should().Be(3);
        savedIsPublishedProperty.ValueStorageType.Should().Be(ValueStorageType.Integer);
        savedIsPublishedProperty.Id.Should().BeGreaterThan(0, "because saved properties should have database IDs");

        // Clean up
        contentTypeService.Delete(savedContentType);
    }

    [Test]
    public void PropertyBuilder_Should_WorkWithDataTypeDefinitionIds_UsingRealDataTypeService()
    {
        // Arrange
        var contentTypeService = GetRequiredService<IContentTypeService>();
        var dataTypeService = GetRequiredService<IDataTypeService>();
        var shortStringHelper = GetRequiredService<IShortStringHelper>();

        // Get a real data type from Umbraco database
        var textBoxDataType = dataTypeService.GetDataType("Umbraco.TextBox");
        textBoxDataType.Should().NotBeNull("because Umbraco should have built-in TextBox data type");

        const string documentTypeAlias = "dataTypeRealTest";
        const string documentTypeName = "Data Type Real Test";

        // Act - Create property with specific data type ID using real services
        var propertyBuilder = new PropertyBuilder("Test Property", "testProperty", "Umbraco.TextBox", shortStringHelper)
            .WithDescription("Property using real data type ID")
            .WithDataTypeDefinitionId(textBoxDataType!.Id)
            .IsMandatory(false);

        var property = propertyBuilder.Build();

        // Create ContentType and save with real services
        var contentType = new ContentType(shortStringHelper, -1)
        {
            Alias = documentTypeAlias,
            Name = documentTypeName,
            Description = "Testing PropertyBuilder with real data type IDs"
        };

        var propertyGroup = new PropertyGroup(new PropertyTypeCollection(true))
        {
            Alias = "content",
            Name = "Content"
        };
        propertyGroup.PropertyTypes!.Add(property);
        contentType.PropertyGroups.Add(propertyGroup);

        // Save to real database
        contentTypeService.Save(contentType);

        // Assert - Retrieve from database and verify data type ID is correct
        var savedContentType = contentTypeService.Get(documentTypeAlias);
        var savedProperty = savedContentType!.PropertyGroups.First().PropertyTypes!.First();
        
        savedProperty.DataTypeId.Should().Be(textBoxDataType.Id, "because we specified the exact data type ID from the real data type service");
        savedProperty.Name.Should().Be("Test Property");
        savedProperty.PropertyEditorAlias.Should().Be("Umbraco.TextBox");
        savedProperty.Id.Should().BeGreaterThan(0, "because the property should be saved to database");

        // Clean up
        contentTypeService.Delete(savedContentType);
    }

    [Test]
    public void PropertyBuilder_Should_CreateContentFromDocumentType_EndToEndWorkflow()
    {
        // Arrange
        var contentTypeService = GetRequiredService<IContentTypeService>();
        var contentService = GetRequiredService<IContentService>();
        var shortStringHelper = GetRequiredService<IShortStringHelper>();

        const string documentTypeAlias = "endToEndRealTest";
        const string documentTypeName = "End-to-End Real Test";

        // Act - Create document type with PropertyBuilder properties
        var titleProperty = new PropertyBuilder("Title", "title", "Umbraco.TextBox", shortStringHelper)
            .WithDescription("The page title")
            .IsMandatory(true)
            .Build();

        var bodyProperty = new PropertyBuilder("Body", "body", "Umbraco.TextArea", shortStringHelper)
            .WithDescription("The page content")
            .IsMandatory(false)
            .Build();

        var contentType = new ContentType(shortStringHelper, -1)
        {
            Alias = documentTypeAlias,
            Name = documentTypeName,
            Description = "End-to-end testing with real content creation",
            AllowedAsRoot = true
        };

        var propertyGroup = new PropertyGroup(new PropertyTypeCollection(true))
        {
            Alias = "content",
            Name = "Content"
        };
        propertyGroup.PropertyTypes!.Add(titleProperty);
        propertyGroup.PropertyTypes!.Add(bodyProperty);
        contentType.PropertyGroups.Add(propertyGroup);

        // Save document type to real database
        contentTypeService.Save(contentType);

        // Create actual content using the document type we just created
        var content = contentService.Create("Test Content", -1, documentTypeAlias);
        content.SetValue("title", "Integration Test Content Title");
        content.SetValue("body", "This content was created using our PropertyBuilder-generated document type!");
        
        // Save and publish content to real database
        contentService.SaveAndPublish(content);

        // Assert - Verify the complete end-to-end workflow
        var savedContentType = contentTypeService.Get(documentTypeAlias);
        savedContentType.Should().NotBeNull("because the document type should be saved");
        savedContentType!.PropertyGroups.Should().HaveCount(1, "because we added one property group");
        savedContentType.PropertyGroups.First().PropertyTypes.Should().HaveCount(2, "because we added two properties");

        var savedContent = contentService.GetById(content.Id);
        savedContent.Should().NotBeNull("because the content should be saved");
        savedContent!.GetValue("title").Should().Be("Integration Test Content Title", "because we set this value on the content");
        savedContent.GetValue("body").Should().Be("This content was created using our PropertyBuilder-generated document type!", "because we set this value on the content");
        savedContent.Published.Should().BeTrue("because we published the content");

        // Verify the content is actually in the database by getting it fresh
        var freshContent = contentService.GetById(content.Id);
        freshContent!.GetValue("title").Should().Be("Integration Test Content Title", "because the value should be persisted to database");

        // Clean up
        contentService.Delete(savedContent);
        contentTypeService.Delete(savedContentType);
    }
}