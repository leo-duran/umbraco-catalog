using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.DocTypeBuilder;
using FluentAssertions;

namespace Umbraco.DocTypeBuilder.Tests;

/// <summary>
/// Integration tests for PropertyBuilder using real SQLite database.
/// These tests verify that PropertyBuilder works correctly with actual database operations.
/// 
/// This demonstrates real-world usage where document types are created using our builder
/// and can be successfully persisted to and retrieved from a SQLite database.
/// </summary>
public class PropertyBuilderIntegrationTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly TestShortStringHelper _shortStringHelper;
    private bool _disposed = false;

    public PropertyBuilderIntegrationTests()
    {
        // Create an in-memory SQLite database for testing
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        // Initialize database schema (basic tables for testing)
        InitializeTestDatabase();

        // Use a real implementation of IShortStringHelper for testing
        _shortStringHelper = new TestShortStringHelper();
    }

    private void InitializeTestDatabase()
    {
        // Create basic tables that would be needed for content type testing
        var createTablesScript = @"
            CREATE TABLE ContentTypes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Alias TEXT NOT NULL,
                Name TEXT NOT NULL,
                Description TEXT,
                Icon TEXT,
                AllowedAsRoot INTEGER NOT NULL DEFAULT 0,
                IsElement INTEGER NOT NULL DEFAULT 0
            );

            CREATE TABLE PropertyGroups (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ContentTypeId INTEGER NOT NULL,
                Name TEXT NOT NULL,
                Alias TEXT,
                SortOrder INTEGER NOT NULL DEFAULT 0,
                FOREIGN KEY (ContentTypeId) REFERENCES ContentTypes(Id)
            );

            CREATE TABLE PropertyTypes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                PropertyGroupId INTEGER NOT NULL,
                Name TEXT NOT NULL,
                Alias TEXT,
                EditorAlias TEXT NOT NULL,
                Description TEXT,
                Mandatory INTEGER NOT NULL DEFAULT 0,
                SortOrder INTEGER NOT NULL DEFAULT 0,
                ValueStorageType INTEGER NOT NULL DEFAULT 0,
                LabelOnTop INTEGER NOT NULL DEFAULT 0,
                FOREIGN KEY (PropertyGroupId) REFERENCES PropertyGroups(Id)
            );
        ";

        using var command = new SqliteCommand(createTablesScript, _connection);
        command.ExecuteNonQuery();
    }

    [Fact]
    public void PropertyBuilder_Should_CreateDocumentType_WithValidStructure()
    {
        // Arrange
        const string documentTypeAlias = "integrationTestType";
        const string documentTypeName = "Integration Test Type";
        const string propertyName = "Test Property";
        const string propertyAlias = "testProperty";
        const string editorAlias = "Umbraco.TextBox";

        // Act - Create a document type using our PropertyBuilder
        var documentTypeBuilder = new DocumentTypeBuilder(_shortStringHelper)
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

        // Simulate saving to our test database
        var contentTypeId = SaveContentTypeToDatabase(contentType);

        // Assert - Verify the document type was created and can be retrieved
        contentType.Should().NotBeNull("because the PropertyBuilder should create a valid ContentType");
        contentType.Alias.Should().Be("integrationtesttype", "because the TestShortStringHelper converts camelCase to lowercase");
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
        
        // Verify database persistence
        contentTypeId.Should().BeGreaterThan(0, "because the content type should be saved to database with an ID");
        
        // Verify we can retrieve it from database
        var retrievedContentType = GetContentTypeFromDatabase(contentTypeId);
        retrievedContentType.Should().NotBeNull("because we should be able to retrieve the saved content type");
        retrievedContentType!.Alias.Should().Be("integrationtesttype", "because the alias is cleaned by the string helper");
        retrievedContentType.Name.Should().Be(documentTypeName);
    }

    [Fact]
    public void PropertyBuilder_Should_HandleMultiplePropertyTypes_InIntegration()
    {
        // Arrange
        const string documentTypeAlias = "multiPropertyIntegrationTest";

        // Act - Test various property types that our builder supports
        var documentTypeBuilder = new DocumentTypeBuilder(_shortStringHelper)
            .WithAlias(documentTypeAlias)
            .WithName("Multi Property Integration Test")
            .AddTab("Content", tab => tab
                .WithAlias("content")
                .AddTextBoxProperty("Title", "title", property => property.IsMandatory(true))
                .AddTextAreaProperty("Description", "description")
                .AddCheckboxProperty("Featured", "featured")
                .AddNumericProperty("OrderNumber", "orderNumber")
                .AddDatePickerProperty("PublishDate", "publishDate")
            );

        var contentType = documentTypeBuilder.Build();
        var contentTypeId = SaveContentTypeToDatabase(contentType);

        // Assert - Verify all property types are correctly configured
        var contentTab = contentType.PropertyGroups.First();
        var properties = contentTab.PropertyTypes!.ToList();
        
        properties.Should().HaveCount(5, "because we added five different property types");

        // Test each property type
        properties.Single(p => p.Name == "Title").PropertyEditorAlias.Should().Be("Umbraco.TextBox");
        properties.Single(p => p.Name == "Description").PropertyEditorAlias.Should().Be("Umbraco.TextArea");
        properties.Single(p => p.Name == "Featured").PropertyEditorAlias.Should().Be("Umbraco.TrueFalse");
        properties.Single(p => p.Name == "OrderNumber").PropertyEditorAlias.Should().Be("Umbraco.Integer");
        properties.Single(p => p.Name == "PublishDate").PropertyEditorAlias.Should().Be("Umbraco.DateTime");

        // Verify mandatory setting
        properties.Single(p => p.Name == "Title").Mandatory.Should().BeTrue();
        
        // Verify database integration
        contentTypeId.Should().BeGreaterThan(0);
    }

    [Fact]
    public void SQLiteDatabase_Should_BeOperational_ForIntegrationTesting()
    {
        // Arrange & Act - Test basic database operations
        _connection.State.Should().Be(ConnectionState.Open, "because we should have an active SQLite connection");

        // Test insert and select
        var insertSql = "INSERT INTO ContentTypes (Alias, Name, Description, AllowedAsRoot) VALUES (@alias, @name, @desc, @allowRoot)";
        using var insertCommand = new SqliteCommand(insertSql, _connection);
        insertCommand.Parameters.AddWithValue("@alias", "testType");
        insertCommand.Parameters.AddWithValue("@name", "Test Type");
        insertCommand.Parameters.AddWithValue("@desc", "A test content type");
        insertCommand.Parameters.AddWithValue("@allowRoot", 1);
        
        var insertedRows = insertCommand.ExecuteNonQuery();
        insertedRows.Should().Be(1, "because we should insert exactly one row");

        // Test select
        var selectSql = "SELECT Alias, Name FROM ContentTypes WHERE Alias = @alias";
        using var selectCommand = new SqliteCommand(selectSql, _connection);
        selectCommand.Parameters.AddWithValue("@alias", "testType");
        
        using var reader = selectCommand.ExecuteReader();
        reader.Read().Should().BeTrue("because we should find our inserted record");
        reader["Alias"].Should().Be("testType");
        reader["Name"].Should().Be("Test Type");
    }

    private int SaveContentTypeToDatabase(ContentType contentType)
    {
        // Insert content type
        var insertContentTypeSql = @"
            INSERT INTO ContentTypes (Alias, Name, Description, Icon, AllowedAsRoot, IsElement) 
            VALUES (@alias, @name, @description, @icon, @allowedAsRoot, @isElement);
            SELECT last_insert_rowid();";
        
        using var command = new SqliteCommand(insertContentTypeSql, _connection);
        command.Parameters.AddWithValue("@alias", contentType.Alias);
        command.Parameters.AddWithValue("@name", contentType.Name);
        command.Parameters.AddWithValue("@description", contentType.Description ?? "");
        command.Parameters.AddWithValue("@icon", contentType.Icon ?? "");
        command.Parameters.AddWithValue("@allowedAsRoot", contentType.AllowedAsRoot ? 1 : 0);
        command.Parameters.AddWithValue("@isElement", contentType.IsElement ? 1 : 0);
        
        var contentTypeId = Convert.ToInt32(command.ExecuteScalar());

        // Insert property groups and properties
        foreach (var propertyGroup in contentType.PropertyGroups)
        {
            SavePropertyGroupToDatabase(contentTypeId, propertyGroup);
        }

        return contentTypeId;
    }

    private void SavePropertyGroupToDatabase(int contentTypeId, PropertyGroup propertyGroup)
    {
        var insertGroupSql = @"
            INSERT INTO PropertyGroups (ContentTypeId, Name, Alias, SortOrder) 
            VALUES (@contentTypeId, @name, @alias, @sortOrder);
            SELECT last_insert_rowid();";
        
        using var command = new SqliteCommand(insertGroupSql, _connection);
        command.Parameters.AddWithValue("@contentTypeId", contentTypeId);
        command.Parameters.AddWithValue("@name", propertyGroup.Name);
        command.Parameters.AddWithValue("@alias", propertyGroup.Alias ?? "");
        command.Parameters.AddWithValue("@sortOrder", propertyGroup.SortOrder);
        
        var groupId = Convert.ToInt32(command.ExecuteScalar());

        // Insert properties
        if (propertyGroup.PropertyTypes != null)
        {
            foreach (var property in propertyGroup.PropertyTypes)
            {
                SavePropertyToDatabase(groupId, property);
            }
        }
    }

    private void SavePropertyToDatabase(int groupId, IPropertyType property)
    {
        var insertPropertySql = @"
            INSERT INTO PropertyTypes (PropertyGroupId, Name, Alias, EditorAlias, Description, Mandatory, SortOrder, ValueStorageType, LabelOnTop) 
            VALUES (@groupId, @name, @alias, @editorAlias, @description, @mandatory, @sortOrder, @valueStorageType, @labelOnTop)";
        
        using var command = new SqliteCommand(insertPropertySql, _connection);
        command.Parameters.AddWithValue("@groupId", groupId);
        command.Parameters.AddWithValue("@name", property.Name);
        command.Parameters.AddWithValue("@alias", property.Alias ?? "");
        command.Parameters.AddWithValue("@editorAlias", property.PropertyEditorAlias);
        command.Parameters.AddWithValue("@description", property.Description ?? "");
        command.Parameters.AddWithValue("@mandatory", property.Mandatory ? 1 : 0);
        command.Parameters.AddWithValue("@sortOrder", property.SortOrder);
        command.Parameters.AddWithValue("@valueStorageType", (int)property.ValueStorageType);
        command.Parameters.AddWithValue("@labelOnTop", property.LabelOnTop ? 1 : 0);
        
        command.ExecuteNonQuery();
    }

    private ContentTypeInfo? GetContentTypeFromDatabase(int contentTypeId)
    {
        var selectSql = "SELECT Alias, Name, Description, AllowedAsRoot FROM ContentTypes WHERE Id = @id";
        using var command = new SqliteCommand(selectSql, _connection);
        command.Parameters.AddWithValue("@id", contentTypeId);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new ContentTypeInfo
            {
                Alias = reader["Alias"].ToString()!,
                Name = reader["Name"].ToString()!,
                Description = reader["Description"].ToString(),
                AllowedAsRoot = Convert.ToBoolean(reader["AllowedAsRoot"])
            };
        }
        return null;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _connection?.Dispose();
            _disposed = true;
        }
    }

    // Helper classes for integration testing
    private class TestShortStringHelper : IShortStringHelper
    {
        public string CleanString(string text, CleanStringType stringType) => text?.ToLowerInvariant().Replace(" ", "") ?? "";
        public string CleanString(string text, CleanStringType stringType, string? culture) => CleanString(text, stringType);
        public string CleanString(string text, CleanStringType stringType, char separator) => CleanString(text, stringType);
        public string CleanString(string text, CleanStringType stringType, char separator, string? culture) => CleanString(text, stringType);
        public string CleanStringForSafeAlias(string text) => text?.ToLowerInvariant().Replace(" ", "") ?? "";
        public string CleanStringForSafeAlias(string text, string? culture) => CleanStringForSafeAlias(text);
        public string CleanStringForSafeFileName(string text) => CleanStringForSafeAlias(text);
        public string CleanStringForSafeFileName(string text, string? culture) => CleanStringForSafeFileName(text);
        public string CleanStringForUrlSegment(string text) => CleanStringForSafeAlias(text);
        public string CleanStringForUrlSegment(string text, string? culture) => CleanStringForUrlSegment(text);
        public string SplitPascalCasing(string text, char separator) => text;
    }

    private class ContentTypeInfo
    {
        public string Alias { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public bool AllowedAsRoot { get; set; }
    }
}