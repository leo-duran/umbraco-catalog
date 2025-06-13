# Adding Template Support to DocumentTypeBuilder

## Overview
In this update, we enhanced the `DocumentTypeBuilder` class to support template creation and association with document types. This improvement streamlines the process of creating document types with templates in Umbraco.

## Implementation Details

### 1. Updated DocumentTypeBuilder Class

The `DocumentTypeBuilder` class was extended with the following features:

- Added a new constructor that accepts `IFileService` and `ITemplateService` for template operations
- Added private fields to store template information
- Implemented both synchronous and asynchronous methods for template creation
- Added template association in the `Build()` method
- Added getter methods for template access

```csharp
// New constructor with template support
public DocumentTypeBuilder(
    IShortStringHelper shortStringHelper, 
    IFileService fileService, 
    ITemplateService templateService)
{
    _shortStringHelper = shortStringHelper;
    _contentType = new ContentType(shortStringHelper, -1);
    _fileService = fileService;
    _templateService = templateService;
}

// Template creation methods
public DocumentTypeBuilder WithTemplate(string templateAlias, string templateName, string templateContent)
{
    // Implementation details...
}

public async Task<DocumentTypeBuilder> WithTemplateAsync(string templateAlias, string templateName, string templateContent)
{
    // Implementation details...
}
```

### 2. Updated CatalogPageDocTypeHandler

The `CatalogPageDocTypeHandler` was refactored to use the new template support:

- Removed manual template creation code
- Used the new constructor that accepts template services
- Used the `WithTemplate` method to create and associate a template
- Simplified the document type creation process

```csharp
// Create the document type with template support
var contentTypeBuilder = new DocumentTypeBuilder(_shortStringHelper, _fileService, _templateService)
    .WithAlias(contentTypeAlias)
    .WithName("Catalog Page")
    .WithDescription("A page that displays a catalog of products")
    .WithIcon("icon-shopping-basket-alt")
    .AllowAtRoot(true)
    .WithTemplate(templateAlias, templateName, GetDefaultTemplateContent())
    // Add properties...
```

## Benefits

1. **Simplified API**: The template creation and association process is now encapsulated within the builder pattern.
2. **Reduced Boilerplate**: No need for manual template creation and association code.
3. **Consistency**: Ensures templates are properly created and associated with document types.
4. **Fluent Interface**: Maintains the fluent API style for better readability.

## Next Steps

1. Update other document type handlers to use the new template support
2. Consider adding more template-related features:
   - Template inheritance
   - Template sections
   - Template composition
3. Add unit tests for the template creation functionality 