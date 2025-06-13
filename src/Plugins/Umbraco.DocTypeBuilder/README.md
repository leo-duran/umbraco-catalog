# Umbraco Document Type Builder

A fluent API for building Umbraco document types programmatically using the builder pattern.

## Features

- Create document types with a clean, fluent API
- Configure tabs (property groups) and properties
- Fully typed API with IntelliSense support
- Helper methods for common property types
- Comprehensive XML documentation

## Installation

Add the package to your Umbraco project:

```bash
dotnet add package Umbraco.DocTypeBuilder
```

## Usage

### Basic Example

```csharp
// Create a document type
var contentTypeBuilder = new DocumentTypeBuilder(_shortStringHelper)
    .WithAlias("product")
    .WithName("Product")
    .WithDescription("A product document type")
    .WithIcon("icon-shopping-basket")
    .AllowAtRoot(false)
    .AddTab("Content", tab => tab
        .WithAlias("content")
        .WithSortOrder(1)
        .AddTextBoxProperty("Title", "title", property => property
            .WithDescription("The product title")
            .IsMandatory()
        )
        .AddRichTextProperty("Description", "description")
        .AddMediaPickerProperty("Product Image", "productImage")
    )
    .AddTab("Pricing", tab => tab
        .WithAlias("pricing")
        .WithSortOrder(2)
        .AddNumericProperty("Price", "price", property => property
            .IsMandatory()
        )
        .AddCheckboxProperty("On Sale", "onSale")
    );

// Build and save the content type
var contentType = contentTypeBuilder.Build();
_contentTypeService.Save(contentType);
```

### Creating Common Property Types

The TabBuilder includes helper methods for common property types:

```csharp
// Add a text box property
.AddTextBoxProperty("Title", "title")

// Add a text area property
.AddTextAreaProperty("Summary", "summary")

// Add a rich text editor property
.AddRichTextProperty("Description", "description")

// Add a media picker property
.AddMediaPickerProperty("Image", "image")

// Add a content picker property
.AddContentPickerProperty("Related Product", "relatedProduct")

// Add a numeric property
.AddNumericProperty("Price", "price")

// Add a checkbox property
.AddCheckboxProperty("Featured", "featured")

// Add a date picker property
.AddDatePickerProperty("Release Date", "releaseDate")
```

### Advanced Configuration

You can configure properties with additional options:

```csharp
.AddTextBoxProperty("Title", "title", property => property
    .WithDescription("The product title")
    .IsMandatory()
    .WithSortOrder(1)
    .WithValidationRegex("^[a-zA-Z0-9 ]*$")
    .WithLabelOnTop()
)
```

## License

MIT 