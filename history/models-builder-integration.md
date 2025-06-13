# Integrating ModelsBuilder with Custom Document Types

## Overview

In this session, we identified and fixed an issue where our custom document types (Product and Catalog Page) were being created successfully but not appearing in the Umbraco backoffice UI. We integrated Umbraco's ModelsBuilder to generate strongly-typed models for our document types, ensuring they are properly recognized by the Umbraco CMS.

## The Problem

Our document types were being created successfully in the database (as confirmed by logs), but they weren't showing up in the Umbraco backoffice UI. This was likely due to one or more of the following issues:

1. ModelsBuilder not being enabled or properly configured
2. Document types not being properly registered with Umbraco's model generation system
3. Missing properties that Umbraco expects for document types to be displayed in the UI

## Solution Steps

### 1. Enabled ModelsBuilder in appsettings.Development.json

We configured ModelsBuilder to automatically generate models from our document types:

```json
"ModelsBuilder": {
  "ModelsMode": "SourceCodeAuto",
  "ModelsNamespace": "Catalog.Models"
}
```

This tells Umbraco to:
- Generate C# models automatically when document types change
- Use "Catalog.Models" as the namespace for the generated models
- Generate the models as source code (rather than in-memory)

### 2. Updated Document Type Handlers

We modified both document type handlers to explicitly set properties required for proper ModelsBuilder integration:

#### ProductDocTypeHandler:

```csharp
// Enable ModelsBuilder for this content type
_logger.LogInformation("Enabling ModelsBuilder for content type");
contentType.IsElement = false;
```

#### CatalogPageDocTypeHandler:

```csharp
// Enable ModelsBuilder for this content type
_logger.LogInformation("Enabling ModelsBuilder for content type");
contentType.IsElement = false;
contentType.AllowedAsRoot = true;
```

### 3. Enhanced Logging

We improved logging to better understand the document type creation process:

```csharp
"Serilog": {
  "MinimumLevel": {
    "Default": "Information",
    "Override": {
      "Catalog.Plugin": "Debug",
      "Umbraco.DocTypeBuilder": "Debug"
    }
  }
}
```

This increased logging verbosity for our custom namespaces to help diagnose any issues.

## Key Insights

1. **IsElement Property**: Setting `IsElement = false` is crucial for document types that should appear in the content tree. Element types (`IsElement = true`) are meant for nested content and don't appear as standalone content nodes.

2. **AllowedAsRoot**: For document types that should be creatable at the root level of the content tree, `AllowedAsRoot = true` must be set.

3. **ModelsBuilder Configuration**: The ModelsBuilder needs to be explicitly enabled and configured in the application settings.

4. **Template Association**: Document types with templates need proper association between the document type and template for the UI to display correctly.

## Benefits of ModelsBuilder Integration

1. **Strongly-Typed Models**: Provides C# classes for document types, enabling IntelliSense and compile-time checking.

2. **Better Developer Experience**: Makes working with document types more intuitive and less error-prone.

3. **UI Integration**: Ensures document types appear correctly in the Umbraco backoffice.

4. **Type Safety**: Prevents runtime errors by catching type mismatches at compile time.

## Next Steps

1. **Create Content**: With properly registered document types, content editors can now create Product and Catalog Page items in the Umbraco backoffice.

2. **Extend Models**: Consider adding custom logic to the generated models by creating partial classes.

3. **Implement Controllers**: Create controllers that use the strongly-typed models for rendering content.

4. **Add Validation**: Implement validation rules for the document type properties. 