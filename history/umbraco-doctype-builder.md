# Umbraco Document Type Builder Project

## Overview
In this session, we implemented a builder pattern for creating Umbraco document types. The builder pattern provides a clean, fluent API for defining document types programmatically, making the code more readable and maintainable.

## Key Components Created

### 1. DocumentTypeBuilder Class
The main builder class for creating ContentType objects with a fluent API:

```csharp
var contentTypeBuilder = new DocumentTypeBuilder(_shortStringHelper)
    .WithAlias("product")
    .WithName("Product")
    .WithDescription("A product document type")
    .WithIcon("icon-shopping-basket")
    .AddTab("Content", tab => tab
        // Configure tab here
    );
```

### 2. TabBuilder Class
Builder for creating property groups (tabs) in document types:

```csharp
.AddTab("Content", tab => tab
    .WithAlias("content")
    .WithSortOrder(1)
    .AddTextBoxProperty("Title", "title", property => property
        .WithDescription("Page title")
        .IsMandatory())
    .AddTextAreaProperty("Description", "description")
);
```

### 3. PropertyBuilder Class
Builder for creating property types with various configurations:

```csharp
.AddTextBoxProperty("Title", "title", property => property
    .WithDescription("Page title")
    .IsMandatory()
    .WithValueStorageType(ValueStorageType.Nvarchar)
    .WithSortOrder(1)
);
```

## Implementation Steps

1. **Created a new class library project**:
   ```bash
   dotnet new classlib -n Umbraco.DocTypeBuilder -o src/Plugins/Umbraco.DocTypeBuilder
   ```

2. **Updated project file** to target .NET 9.0 and add Umbraco references:
   ```xml
   <Project Sdk="Microsoft.NET.Sdk">
     <PropertyGroup>
       <TargetFramework>net9.0</TargetFramework>
       <ImplicitUsings>enable</ImplicitUsings>
       <Nullable>enable</Nullable>
     </PropertyGroup>
     <ItemGroup>
       <PackageReference Include="Umbraco.Cms.Core" Version="15.4.3" />
     </ItemGroup>
   </Project>
   ```

3. **Implemented the builder classes**:
   - DocumentTypeBuilder for ContentType objects
   - TabBuilder for PropertyGroup objects
   - PropertyBuilder for PropertyType objects

4. **Added helper methods** for common property types:
   - AddTextBoxProperty
   - AddTextAreaProperty
   - AddRichTextProperty
   - AddMediaPickerProperty
   - AddNumericProperty
   - etc.

5. **Created comprehensive documentation** with XML comments and a README.md file

6. **Updated ProductDocTypeHandler** to use the new builder pattern:
   ```csharp
   var contentTypeBuilder = new DocumentTypeBuilder(_shortStringHelper)
       .WithAlias(contentTypeAlias)
       .WithName("Product")
       .WithDescription("A product document type")
       .WithIcon("icon-shopping-basket")
       .AddTab("Content", tab => tab
           .WithAlias("content")
           .WithSortOrder(1)
           .AddTextBoxProperty("Title", "title", property => property
               .WithDescription("Page title")
               .IsMandatory())
           // More properties...
       );
   ```

7. **Created CatalogPageDocTypeHandler** using the builder pattern:
   - Created a document type for catalog pages
   - Added a template for rendering
   - Configured content and product tabs

8. **Registered handlers** in the CatalogPluginComposer:
   ```csharp
   builder
       .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, ProductDocTypeHandler>()
       .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, CatalogPageDocTypeHandler>();
   ```

## Benefits of the Builder Pattern

1. **Improved readability**: The fluent API makes document type creation more readable and self-documenting.

2. **Better maintainability**: Changes to document types are easier to understand and implement.

3. **Type safety**: The strongly-typed API prevents errors and provides better IDE support.

4. **Reusability**: The builder pattern can be used across multiple document type handlers.

5. **Separation of concerns**: Each builder focuses on a specific aspect of document type creation.

## Class Diagram

```
DocumentTypeBuilder
├── WithAlias()
├── WithName()
├── WithDescription()
├── WithIcon()
├── AddTab()
└── Build()

TabBuilder
├── WithAlias()
├── WithSortOrder()
├── AddProperty()
├── AddTextBoxProperty()
├── AddTextAreaProperty()
└── Build()

PropertyBuilder
├── WithDescription()
├── IsMandatory()
├── WithValueStorageType()
└── Build()
```

## Future Enhancements

1. **Additional helper methods** for more property types and configurations

2. **Extension methods** to further enhance the API

3. **Factory methods** for common document type patterns

4. **Validation** to ensure document types meet Umbraco requirements

5. **Publishing as a NuGet package** for use in other Umbraco projects 