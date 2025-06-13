# Failing Tests Log - Umbraco DocTypeBuilder

**Date**: 2024-12-19  
**Command**: `dotnet test src/Plugins/Umbraco.DocTypeBuilder.Tests/ --verbosity normal`  
**Total Tests**: 63  
**Passing**: 30 ✅  
**Failing**: 33 ❌  

---

## 🚨 **FAILING TESTS DETAILED LOG**

### **1. PropertyBuilder Tests (8 failures)**

#### ❌ `PropertyBuilder_Should_Create_Basic_Property`
```
Expected property.Alias to be "testProperty", but found <null>.
Stack Trace:
   at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
   at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
   at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
   at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
   at FluentAssertions.Primitives.StringValidator.ValidateAgainstNulls()
   at FluentAssertions.Primitives.StringValidator.Validate()
   at FluentAssertions.Primitives.StringAssertions`1.Be(String expected, String because, Object[] becauseArgs)
   at Umbraco.DocTypeBuilder.Tests.PropertyBuilderTests.PropertyBuilder_Should_Create_Basic_Property() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/PropertyBuilderTests.cs:line 41
```

#### ❌ `PropertyBuilder_Should_Chain_All_Configuration_Methods`
```
Expected property.Alias to be "complexProperty", but found <null>.
Stack Trace:
   at Umbraco.DocTypeBuilder.Tests.PropertyBuilderTests.PropertyBuilder_Should_Chain_All_Configuration_Methods() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/PropertyBuilderTests.cs:line 186
```

#### ❌ `PropertyBuilder_Should_Have_Default_Values`
```
Expected property.Alias to be "defaultProperty", but found <null>.
Stack Trace:
   at Umbraco.DocTypeBuilder.Tests.PropertyBuilderTests.PropertyBuilder_Should_Have_Default_Values() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/PropertyBuilderTests.cs:line 209
```

#### ❌ `PropertyBuilder_Should_Handle_Null_Values_Gracefully`
```
Expected property.Description to be empty, but found <null>.
Stack Trace:
   at FluentAssertions.Primitives.StringAssertions`1.BeEmpty(String because, Object[] becauseArgs)
   at Umbraco.DocTypeBuilder.Tests.PropertyBuilderTests.PropertyBuilder_Should_Handle_Null_Values_Gracefully() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/PropertyBuilderTests.cs:line 261
```

---

### **2. TemplateBuilder Tests (6 failures)**

#### ❌ `TemplateBuilder_Should_Create_Basic_Template`
```
Expected template.Alias to be "homePage", but found <null>.
Stack Trace:
   at Umbraco.DocTypeBuilder.Tests.TemplateBuilderTests.TemplateBuilder_Should_Create_Basic_Template() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/TemplateBuilderTests.cs:line 42
```

#### ❌ `TemplateBuilder_Should_Set_Alias`
```
Expected template.Alias to be "testTemplate", but found <null>.
Stack Trace:
   at Umbraco.DocTypeBuilder.Tests.TemplateBuilderTests.TemplateBuilder_Should_Set_Alias() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/TemplateBuilderTests.cs:line 58
```

#### ❌ `TemplateBuilder_Should_Chain_All_Configuration_Methods`
```
Expected template.Alias to be "complexTemplate", but found <null>.
Stack Trace:
   at Umbraco.DocTypeBuilder.Tests.TemplateBuilderTests.TemplateBuilder_Should_Chain_All_Configuration_Methods() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/TemplateBuilderTests.cs:line 114
```

#### ❌ `TemplateBuilder_Should_Have_Default_Values`
```
Expected template.Alias to be "defaultTemplate", but found <null>.
Stack Trace:
   at Umbraco.DocTypeBuilder.Tests.TemplateBuilderTests.TemplateBuilder_Should_Have_Default_Values() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/TemplateBuilderTests.cs:line 130
```

#### ❌ `TemplateBuilder_Should_Create_Master_Template`
```
Expected template.Alias to be "masterTemplate", but found <null>.
Stack Trace:
   at Umbraco.DocTypeBuilder.Tests.TemplateBuilderTests.TemplateBuilder_Should_Create_Master_Template() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/TemplateBuilderTests.cs:line 198
```

#### ❌ `TemplateBuilder_Should_Create_Child_Template_With_Layout`
```
Expected template.Alias to be "pageTemplate", but found <null>.
Stack Trace:
   at Umbraco.DocTypeBuilder.Tests.TemplateBuilderTests.TemplateBuilder_Should_Create_Child_Template_With_Layout() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/TemplateBuilderTests.cs:line 226
```

---

### **3. TabBuilder Tests (9 failures)**

#### ❌ `TabBuilder_Should_Add_TextBox_Property`
```
Expected property.Alias to be "title", but found <null>.
Stack Trace:
   at Umbraco.DocTypeBuilder.Tests.TabBuilderTests.TabBuilder_Should_Add_TextBox_Property() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/TabBuilderTests.cs:line 74
```

#### ❌ `TabBuilder_Should_Add_TextArea_Property`
```
Expected property.Alias to be "description", but found <null>.
Stack Trace:
   at Umbraco.DocTypeBuilder.Tests.TabBuilderTests.TabBuilder_Should_Add_TextArea_Property() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/TabBuilderTests.cs:line 94
```

#### ❌ `TabBuilder_Should_Add_RichText_Property`
```
Expected property.Alias to be "bodyText", but found <null>.
Stack Trace:
   at Umbraco.DocTypeBuilder.Tests.TabBuilderTests.TabBuilder_Should_Add_RichText_Property() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/TabBuilderTests.cs:line 114
```

#### ❌ `TabBuilder_Should_Add_MediaPicker_Property`
```
Expected property.Alias to be "heroImage", but found <null>.
Stack Trace:
   at Umbraco.DocTypeBuilder.Tests.TabBuilderTests.TabBuilder_Should_Add_MediaPicker_Property() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/TabBuilderTests.cs:line 134
```

#### ❌ `TabBuilder_Should_Add_ContentPicker_Property`
```
Expected property.Alias to be "relatedPage", but found <null>.
Stack Trace:
   at Umbraco.DocTypeBuilder.Tests.TabBuilderTests.TabBuilder_Should_Add_ContentPicker_Property() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/TabBuilderTests.cs:line 154
```

#### ❌ `TabBuilder_Should_Add_Integer_Property`
```
Expected property.Alias to be "sortOrder", but found <null>.
Stack Trace:
   at Umbraco.DocTypeBuilder.Tests.TabBuilderTests.TabBuilder_Should_Add_Integer_Property() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/TabBuilderTests.cs:line 174
```

#### ❌ `TabBuilder_Should_Add_Checkbox_Property`
```
Expected property.Alias to be "isActive", but found <null>.
Stack Trace:
   at Umbraco.DocTypeBuilder.Tests.TabBuilderTests.TabBuilder_Should_Add_Checkbox_Property() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/TabBuilderTests.cs:line 195
```

#### ❌ `TabBuilder_Should_Add_DatePicker_Property`
```
Expected property.Alias to be "publishDate", but found <null>.
Stack Trace:
   at Umbraco.DocTypeBuilder.Tests.TabBuilderTests.TabBuilder_Should_Add_DatePicker_Property() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/TabBuilderTests.cs:line 216
```

#### ❌ `TabBuilder_Should_Add_Multiple_Properties`
```
Expected titleProperty not to be <null>.
Stack Trace:
   at FluentAssertions.Primitives.ReferenceTypeAssertions`2.NotBeNull(String because, Object[] becauseArgs)
   at Umbraco.DocTypeBuilder.Tests.TabBuilderTests.TabBuilder_Should_Add_Multiple_Properties() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/TabBuilderTests.cs:line 240
```

#### ❌ `TabBuilder_Should_Handle_All_Property_Types_With_Configuration`
```
System.InvalidOperationException : Sequence contains no matching element
Stack Trace:
   at System.Linq.ThrowHelper.ThrowNoMatchException()
   at System.Linq.Enumerable.First[TSource](IEnumerable`1 source, Func`2 predicate)
   at Umbraco.DocTypeBuilder.Tests.TabBuilderTests.TabBuilder_Should_Handle_All_Property_Types_With_Configuration() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/TabBuilderTests.cs:line 296
```

---

### **4. DocumentTypeBuilder Tests (10 failures)**

#### ❌ `DocumentTypeBuilder_Should_Create_Basic_ContentType`
```
Expected contentType.Alias to be "testAlias", but found <null>.
Stack Trace:
   at Umbraco.DocTypeBuilder.Tests.DocumentTypeBuilderTests.DocumentTypeBuilder_Should_Create_Basic_ContentType() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/DocumentTypeBuilderTests.cs:line 41
```

#### ❌ `DocumentTypeBuilder_Should_Chain_All_Configuration_Methods`
```
Expected contentType.Alias to be "complexPage", but found <null>.
Stack Trace:
   at Umbraco.DocTypeBuilder.Tests.DocumentTypeBuilderTests.DocumentTypeBuilder_Should_Chain_All_Configuration_Methods() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/DocumentTypeBuilderTests.cs:line 239
```

#### ❌ `DocumentTypeBuilder_Should_Have_Default_Values`
```
Expected contentType.Alias to be "defaultAlias", but found <null>.
Stack Trace:
   at Umbraco.DocTypeBuilder.Tests.DocumentTypeBuilderTests.DocumentTypeBuilder_Should_Have_Default_Values() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/DocumentTypeBuilderTests.cs:line 258
```

#### ❌ `DocumentTypeBuilder_Should_Add_Tab_With_Properties`
```
System.InvalidOperationException : Set an alias before adding the property group.
Stack Trace:
   at Umbraco.Cms.Core.Models.PropertyGroupCollection.Add(PropertyGroup item)
   at Umbraco.DocTypeBuilder.DocumentTypeBuilder.Build() in /workspace/src/Plugins/Umbraco.DocTypeBuilder/DocumentTypeBuilder.cs:line 152
   at Umbraco.DocTypeBuilder.Tests.DocumentTypeBuilderTests.DocumentTypeBuilder_Should_Add_Tab_With_Properties() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/DocumentTypeBuilderTests.cs:line 170
```

#### ❌ `DocumentTypeBuilder_Should_Add_Multiple_Tabs`
```
System.InvalidOperationException : Set an alias before adding the property group.
Stack Trace:
   at Umbraco.Cms.Core.Models.PropertyGroupCollection.Add(PropertyGroup item)
   at Umbraco.DocTypeBuilder.DocumentTypeBuilder.Build() in /workspace/src/Plugins/Umbraco.DocTypeBuilder/DocumentTypeBuilder.cs:line 152
   at Umbraco.DocTypeBuilder.Tests.DocumentTypeBuilderTests.DocumentTypeBuilder_Should_Add_Multiple_Tabs() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/DocumentTypeBuilderTests.cs:line 200
```

#### ❌ `DocumentTypeBuilder_Should_Create_Document_Type_With_Complex_Structure`
```
System.InvalidOperationException : Set an alias before adding the property group.
Stack Trace:
   at Umbraco.Cms.Core.Models.PropertyGroupCollection.Add(PropertyGroup item)
   at Umbraco.DocTypeBuilder.DocumentTypeBuilder.Build() in /workspace/src/Plugins/Umbraco.DocTypeBuilder/DocumentTypeBuilder.cs:line 152
   at Umbraco.DocTypeBuilder.Tests.DocumentTypeBuilderTests.DocumentTypeBuilder_Should_Create_Document_Type_With_Complex_Structure() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/DocumentTypeBuilderTests.cs:line 273
```

#### ❌ `DocumentTypeBuilder_Should_Handle_Empty_Tab_Configuration`
```
System.InvalidOperationException : Set an alias before adding the property group.
Stack Trace:
   at Umbraco.Cms.Core.Models.PropertyGroupCollection.Add(PropertyGroup item)
   at Umbraco.DocTypeBuilder.DocumentTypeBuilder.Build() in /workspace/src/Plugins/Umbraco.DocTypeBuilder/DocumentTypeBuilder.cs:line 152
   at Umbraco.DocTypeBuilder.Tests.DocumentTypeBuilderTests.DocumentTypeBuilder_Should_Handle_Empty_Tab_Configuration() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/DocumentTypeBuilderTests.cs:line 315
```

---

### **5. Integration Tests (6 failures)**

#### ❌ `Integration_Should_Create_Complete_Blog_Post_Document_Type`
```
System.InvalidOperationException : Set an alias before adding the property group.
Stack Trace:
   at Umbraco.Cms.Core.Models.PropertyGroupCollection.Add(PropertyGroup item)
   at Umbraco.DocTypeBuilder.DocumentTypeBuilder.Build() in /workspace/src/Plugins/Umbraco.DocTypeBuilder/DocumentTypeBuilder.cs:line 152
   at Umbraco.DocTypeBuilder.Tests.IntegrationTests.Integration_Should_Create_Complete_Blog_Post_Document_Type() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/IntegrationTests.cs:line 35
```

#### ❌ `Integration_Should_Create_E_Commerce_Product_Document_Type`
```
System.InvalidOperationException : Set an alias before adding the property group.
Stack Trace:
   at Umbraco.Cms.Core.Models.PropertyGroupCollection.Add(PropertyGroup item)
   at Umbraco.DocTypeBuilder.DocumentTypeBuilder.Build() in /workspace/src/Plugins/Umbraco.DocTypeBuilder/DocumentTypeBuilder.cs:line 152
   at Umbraco.DocTypeBuilder.Tests.IntegrationTests.Integration_Should_Create_E_Commerce_Product_Document_Type() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/IntegrationTests.cs:line 94
```

#### ❌ `Integration_Should_Create_Landing_Page_Document_Type`
```
System.InvalidOperationException : Set an alias before adding the property group.
Stack Trace:
   at Umbraco.Cms.Core.Models.PropertyGroupCollection.Add(PropertyGroup item)
   at Umbraco.DocTypeBuilder.DocumentTypeBuilder.Build() in /workspace/src/Plugins/Umbraco.DocTypeBuilder/DocumentTypeBuilder.cs:line 152
   at Umbraco.DocTypeBuilder.Tests.IntegrationTests.Integration_Should_Create_Landing_Page_Document_Type() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/IntegrationTests.cs:line 152
```

#### ❌ `Integration_Should_Create_Block_Grid_Element_Types`
```
System.InvalidOperationException : Set an alias before adding the property group.
Stack Trace:
   at Umbraco.Cms.Core.Models.PropertyGroupCollection.Add(PropertyGroup item)
   at Umbraco.DocTypeBuilder.DocumentTypeBuilder.Build() in /workspace/src/Plugins/Umbraco.DocTypeBuilder/DocumentTypeBuilder.cs:line 152
   at Umbraco.DocTypeBuilder.Tests.IntegrationTests.Integration_Should_Create_Block_Grid_Element_Types() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/IntegrationTests.cs:line 207
```

#### ❌ `Integration_Should_Create_Complex_Multi_Tab_Document_Type`
```
System.InvalidOperationException : Set an alias before adding the property group.
Stack Trace:
   at Umbraco.Cms.Core.Models.PropertyGroupCollection.Add(PropertyGroup item)
   at Umbraco.DocTypeBuilder.DocumentTypeBuilder.Build() in /workspace/src/Plugins/Umbraco.DocTypeBuilder/DocumentTypeBuilder.cs:line 152
   at Umbraco.DocTypeBuilder.Tests.IntegrationTests.Integration_Should_Create_Complex_Multi_Tab_Document_Type() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/IntegrationTests.cs:line 263
```

#### ❌ `Integration_Should_Demonstrate_Builder_Pattern_Flexibility`
```
System.InvalidOperationException : Set an alias before adding the property group.
Stack Trace:
   at Umbraco.Cms.Core.Models.PropertyGroupCollection.Add(PropertyGroup item)
   at Umbraco.DocTypeBuilder.DocumentTypeBuilder.Build() in /workspace/src/Plugins/Umbraco.DocTypeBuilder/DocumentTypeBuilder.cs:line 152
   at Umbraco.DocTypeBuilder.Tests.IntegrationTests.Integration_Should_Demonstrate_Builder_Pattern_Flexibility() in /workspace/src/Plugins/Umbraco.DocTypeBuilder.Tests/IntegrationTests.cs:line 341
```

---

## 📊 **FAILURE PATTERN ANALYSIS**

### **Primary Issues:**

1. **Alias Properties Null (23 failures)**
   - All PropertyType, Template, and ContentType objects have null Alias properties
   - Indicates Umbraco 15.4.3 constructor/setter behavior has changed

2. **PropertyGroup Dependency (10 failures)**
   - PropertyGroups require parent ContentType to have valid alias
   - Error: "Set an alias before adding the property group"

### **Root Cause:**
Umbraco 15.4.3 has changed how object aliases are set, making our current approach incompatible with the new API patterns.

### **Build Status:**
✅ **All compilation errors resolved**  
❌ **Runtime behavior incompatible with Umbraco 15.4.3**