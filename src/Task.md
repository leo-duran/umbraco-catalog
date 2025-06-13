# AI Agent Instructions: DocTypeBuilder Project Implementation

## Project Overview

**Objective**: Create a comprehensive DocTypeBuilder library for Umbraco CMS with full unit testing to verify functionality and demonstrate builder pattern implementation. 

* There is already a partially implenented DocumentTypeBuilder
* The CatalogPageDocTypeHandler.cs and ProductDocTypeHandler.cs uses the DocumentTypeBuilder
* Make sure to follow a cycle of red/green/refactor for the tests and implement one at a time so you don't create loads of unverified code
* It is ok if things fail, that is how we learn, don't hide failures. I want a focus on what is NOT working

**Context**: This is an educational project to teach programming concepts, testing strategies, and real-world API integration challenges.

## Use Case Description

The user wants to:
1. Verify that an existing DocTypeBuilder works correctly
2. Create comprehensive unit tests using modern .NET and testing frameworks
3. Demonstrate builder pattern implementation with fluent API
4. Test all supported Umbraco CMS document type creation scenarios
5. Identify and document any API compatibility issues

## Technical Requirements

### Framework Specifications
- **.NET Version**: 9.0
- **Umbraco Version**: 15.4.3 (latest at time of implementation)
- **Testing Framework**: xUnit
- **Assertion Library**: FluentAssertions
- **Mocking Framework**: Moq
- **Target Platform**: Cross-platform (linux, windows, macOS)

### Project Structure Expected
```
src/
  Plugins/
    Umbraco.DocTypeBuilder/          # Main library
    Umbraco.DocTypeBuilder.Tests/    # Test project
```

## Step-by-Step Implementation Guide

### Phase 1: Project Analysis and Setup

1. **Analyze Existing Codebase**
   ```bash
   # Explore the project structure
   list_dir src/
   list_dir src/Plugins/
   
   # Examine existing DocTypeBuilder files
   read_file src/Plugins/Umbraco.DocTypeBuilder/DocumentTypeBuilder.cs
   read_file src/Plugins/Umbraco.DocTypeBuilder/PropertyBuilder.cs
   read_file src/Plugins/Umbraco.DocTypeBuilder/TabBuilder.cs
   ```

2. **Create Test Project Structure**
   ```bash
   # Create test project with proper dependencies
   dotnet new xunit -n Umbraco.DocTypeBuilder.Tests -f net9.0
   
   # Add required NuGet packages
   dotnet add package Umbraco.Cms.Core --version 15.4.3
   dotnet add package FluentAssertions --version 6.12.0
   dotnet add package Moq --version 4.20.70
   dotnet add package Microsoft.NET.Test.Sdk --version 17.8.0
   dotnet add package xunit.runner.visualstudio --version 2.5.3
   ```

3. **Configure Project References**
   ```xml
   <!-- Add project reference to main library -->
   <ProjectReference Include="../Umbraco.DocTypeBuilder/Umbraco.DocTypeBuilder.csproj" />
   ```

### Phase 2: Comprehensive Test Creation

#### Test Categories to Implement

1. **PropertyBuilder Tests** (15+ tests)
   - Basic property creation
   - Property configuration (name, alias, description)
   - Data type assignment
   - Validation settings
   - Method chaining verification
   - Default values testing
   - Null value handling

2. **TabBuilder Tests** (20+ tests)
   - All Umbraco property types:
     - TextBox, TextArea, RichText
     - Integer, Checkbox, DatePicker
     - MediaPicker, ContentPicker
     - And all other supported types
   - Tab configuration
   - Multiple properties per tab
   - Property ordering

3. **DocumentTypeBuilder Tests** (18+ tests)
   - Basic document type creation
   - Element type creation
   - Document type inheritance
   - Multiple tabs addition
   - Complex structure creation
   - Template assignment
   - Default values
   - Method chaining

4. **TemplateBuilder Tests** (25+ tests)
   - Basic template creation
   - Template hierarchy (master/child)
   - Template content management
   - Alias and name configuration
   - Method chaining

5. **Integration Tests** (6+ tests)
   - Real-world scenarios:
     - Blog post document type
     - E-commerce product type
     - Landing page type
     - Block grid elements
   - Complex multi-tab structures
   - Builder pattern flexibility

#### Test Implementation Pattern

```csharp
[Fact]
public void TestName_Should_ExpectedBehavior()
{
    // Arrange
    var builder = new BuilderClass();
    
    // Act
    var result = builder
        .ConfigurationMethod1("value1")
        .ConfigurationMethod2("value2")
        .Build();
    
    // Assert
    result.Should().NotBeNull();
    result.Property1.Should().Be("value1");
    result.Property2.Should().Be("value2");
}
```

### Phase 3: Build and Test Execution

1. **Build Verification**
   ```bash
   # Ensure clean build
   dotnet clean
   dotnet build
   
   # Verify no compilation errors
   # All projects should build successfully
   ```

2. **Test Execution**
   ```bash
   # Run tests with detailed output
   dotnet test src/Plugins/Umbraco.DocTypeBuilder.Tests/ --verbosity normal
   
   # Analyze results:
   # - Total test count
   # - Pass/fail ratio
   # - Specific failure patterns
   ```

3. **Results Analysis**
   - Document pass/fail statistics
   - Identify failure categories
   - Analyze root causes
   - Document API compatibility issues

### Phase 4: Issue Investigation and Documentation

#### Common Issues to Investigate

1. **Alias Assignment Problems**
   - Check if `SetAlias()` methods actually set properties
   - Verify Umbraco internal behavior
   - Document API behavior changes

2. **Property Group Addition Errors**
   - "Set an alias before adding the property group" errors
   - Umbraco framework requirements
   - Order of operations issues

3. **API Compatibility Issues**
   - Framework version differences
   - Breaking changes in Umbraco 15.x
   - Internal API behavior modifications

#### Documentation Requirements

Create comprehensive documentation covering:
- Test execution results
- Identified issues and root causes
- API compatibility analysis
- Learning outcomes
- Recommendations for production use

### Phase 5: Educational Analysis

#### Key Learning Points to Document

1. **Build vs Runtime Success**
   - Compilation success ≠ Runtime functionality
   - Importance of comprehensive testing
   - Real-world API integration challenges

2. **Framework Version Management**
   - Version compatibility issues
   - Breaking changes in major updates
   - Importance of version pinning

3. **Testing Strategy Value**
   - Unit tests reveal hidden issues
   - Integration tests verify real behavior
   - Test metrics provide clear success indicators

## Expected Outcomes

### Success Metrics
- **Build Success**: 100% compilation success
- **Test Coverage**: 60+ comprehensive tests
- **Functionality Verification**: Clear pass/fail metrics
- **Issue Identification**: Specific failure points documented
- **Educational Value**: Real-world development challenges demonstrated

### Deliverables
1. Complete test suite (60+ tests)
2. Build system configuration
3. Comprehensive results analysis
4. Educational documentation
5. API compatibility report
6. Recommendations for future development

## Critical Success Factors

1. **Comprehensive Testing**: Cover all major functionality
2. **Proper Framework Versions**: Use specified .NET 9 and Umbraco 15.4.3
3. **Detailed Analysis**: Don't just report pass/fail, analyze why
4. **Educational Focus**: Document learning outcomes
5. **Real-World Relevance**: Address practical development challenges

## Common Pitfalls to Avoid

1. **Incomplete Test Coverage**: Ensure all builders and scenarios are tested
2. **Wrong Test Commands**: Use correct `dotnet test` syntax with proper paths
3. **Ignoring Failures**: Investigate and document all test failures
4. **Version Mismatches**: Ensure consistent framework versions
5. **Missing Documentation**: Document both successes and failures

## Final Validation

The project is successful when:
- ✅ All code compiles without errors
- ✅ Comprehensive test suite is created (60+ tests)
- ✅ Test execution provides clear metrics
- ✅ Issues are identified and documented
- ✅ Educational value is demonstrated
- ✅ Real-world challenges are addressed

This approach demonstrates professional software development practices while providing valuable learning experiences about API integration, testing strategies, and framework compatibility challenges.
