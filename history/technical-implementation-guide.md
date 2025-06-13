# Technical Implementation Guide: DocTypeBuilder Testing Project

## Exact Command Sequence

### Initial Project Setup

```bash
# 1. Navigate to the correct directory
cd /workspace

# 2. Analyze existing project structure
list_dir src/
list_dir src/Plugins/

# 3. Examine existing DocTypeBuilder files
read_file src/Plugins/Umbraco.DocTypeBuilder/DocumentTypeBuilder.cs
read_file src/Plugins/Umbraco.DocTypeBuilder/PropertyBuilder.cs  
read_file src/Plugins/Umbraco.DocTypeBuilder/TabBuilder.cs
```

### Test Project Creation

```bash
# 4. Create test project directory
mkdir -p src/Plugins/Umbraco.DocTypeBuilder.Tests

# 5. Create test project file
edit_file src/Plugins/Umbraco.DocTypeBuilder.Tests/Umbraco.DocTypeBuilder.Tests.csproj
```

**Project File Content:**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="xunit" Version="2.6.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.3">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="Moq" Version="4.20.70" />
    <PackageReference Include="Umbraco.Cms.Core" Version="15.4.3" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="../Umbraco.DocTypeBuilder/Umbraco.DocTypeBuilder.csproj" />
  </ItemGroup>
</Project>
```

### Test File Creation Sequence

```bash
# 6. Create PropertyBuilder tests
edit_file src/Plugins/Umbraco.DocTypeBuilder.Tests/PropertyBuilderTests.cs

# 7. Create TabBuilder tests  
edit_file src/Plugins/Umbraco.DocTypeBuilder.Tests/TabBuilderTests.cs

# 8. Create DocumentTypeBuilder tests
edit_file src/Plugins/Umbraco.DocTypeBuilder.Tests/DocumentTypeBuilderTests.cs

# 9. Create TemplateBuilder (if needed)
edit_file src/Plugins/Umbraco.DocTypeBuilder/TemplateBuilder.cs

# 10. Create TemplateBuilder tests
edit_file src/Plugins/Umbraco.DocTypeBuilder.Tests/TemplateBuilderTests.cs

# 11. Create Integration tests
edit_file src/Plugins/Umbraco.DocTypeBuilder.Tests/IntegrationTests.cs
```

## Test File Templates

### PropertyBuilderTests.cs Template
```csharp
using FluentAssertions;
using Umbraco.Cms.Core.Models;
using Umbraco.DocTypeBuilder;
using Xunit;

namespace Umbraco.DocTypeBuilder.Tests;

public class PropertyBuilderTests
{
    [Fact]
    public void PropertyBuilder_Should_Create_Basic_Property()
    {
        // Arrange & Act
        var property = new PropertyBuilder()
            .SetAlias("testProperty")
            .SetName("Test Property")
            .SetDataType("Umbraco.TextBox")
            .Build();

        // Assert
        property.Should().NotBeNull();
        property.Alias.Should().Be("testProperty");
        property.Name.Should().Be("Test Property");
    }

    // Add 14 more similar tests covering all scenarios
}
```

### TabBuilderTests.cs Template
```csharp
using FluentAssertions;
using Umbraco.Cms.Core.Models;
using Umbraco.DocTypeBuilder;
using Xunit;
using System.Linq;

namespace Umbraco.DocTypeBuilder.Tests;

public class TabBuilderTests
{
    [Fact]
    public void TabBuilder_Should_Add_TextBox_Property()
    {
        // Arrange & Act
        var tab = new TabBuilder()
            .SetName("Content")
            .AddTextBox("title", "Title")
            .Build();

        // Assert
        tab.Should().NotBeNull();
        tab.Name.Should().Be("Content");
        var property = tab.PropertyTypes.FirstOrDefault(p => p.Alias == "title");
        property.Should().NotBeNull();
        property.Alias.Should().Be("title");
    }

    // Add tests for all Umbraco property types:
    // TextArea, RichText, Integer, Checkbox, DatePicker, 
    // MediaPicker, ContentPicker, etc.
}
```

## Build and Test Commands

### Build Verification
```bash
# 12. Clean and build
dotnet clean
dotnet build

# Expected: Build succeeded with 0 errors
```

### Test Execution
```bash
# 13. Run tests with correct command
dotnet test src/Plugins/Umbraco.DocTypeBuilder.Tests/ --verbosity normal

# Expected output format:
# Test summary: total: X, failed: Y, succeeded: Z, skipped: 0, duration: N.Ns
```

## API Compatibility Issues to Address

### Issue 1: SetAlias Method Pattern
```csharp
// Original pattern that may not work:
.WithAlias("alias")

// Updated pattern for Umbraco 15:
.SetAlias("alias")
```

### Issue 2: Constructor Updates
```csharp
// Old constructor pattern:
new ContentType(-1)

// New constructor pattern for Umbraco 15:
new ContentType(Mock.Of<IShortStringHelper>(), -1)
```

### Issue 3: Template Assignment
```csharp
// This may be read-only in Umbraco 15:
contentType.DefaultTemplate = template;

// Alternative approach needed
```

## Expected Test Results Analysis

### Success Indicators
- **Build**: 100% compilation success
- **Test Count**: 60+ tests created
- **Execution**: Tests run without crashes

### Failure Patterns to Document
1. **Alias Assignment Failures**
   ```
   Expected property.Alias to be "testProperty", but found <null>
   ```

2. **Property Group Errors**
   ```
   System.InvalidOperationException : Set an alias before adding the property group.
   ```

3. **Sequence Errors**
   ```
   System.InvalidOperationException : Sequence contains no matching element
   ```

## Documentation Creation Commands

```bash
# 14. Create comprehensive analysis
edit_file history/test-execution-analysis.md

# 15. Document learning outcomes
edit_file history/learning-outcomes.md

# 16. Create recommendations
edit_file history/recommendations.md
```

## Validation Checklist

### Technical Validation
- [ ] All files compile without errors
- [ ] Test project references main project correctly
- [ ] All NuGet packages are compatible versions
- [ ] Tests execute (even if some fail)

### Test Coverage Validation
- [ ] PropertyBuilder: 15+ tests
- [ ] TabBuilder: 20+ tests covering all property types
- [ ] DocumentTypeBuilder: 18+ tests
- [ ] TemplateBuilder: 25+ tests
- [ ] Integration: 6+ real-world scenarios

### Results Analysis Validation
- [ ] Pass/fail statistics documented
- [ ] Root cause analysis completed
- [ ] API compatibility issues identified
- [ ] Educational value documented

## Success Metrics

### Quantitative Metrics
- **Build Success Rate**: 100%
- **Test Creation**: 60+ tests
- **Test Execution**: All tests run to completion
- **Documentation**: 5+ comprehensive documents

### Qualitative Metrics
- **Educational Value**: Demonstrates real-world challenges
- **Code Quality**: Clean, well-structured implementation
- **Problem Identification**: Clear issue documentation
- **Learning Outcomes**: Valuable insights documented

## Common Troubleshooting

### Build Issues
```bash
# If build fails, check:
dotnet restore
dotnet clean
dotnet build --verbosity detailed
```

### Test Execution Issues
```bash
# If tests don't run:
dotnet test --list-tests  # List available tests
dotnet test --verbosity diagnostic  # Detailed output
```

### Package Issues
```bash
# If package conflicts:
dotnet list package --outdated
dotnet add package [PackageName] --version [SpecificVersion]
```

This guide provides the exact sequence of commands and code patterns needed to replicate our DocTypeBuilder testing project implementation.