using System.Linq;
using FluentAssertions;
using Moq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;
using Umbraco.DocTypeBuilder;
using Xunit;

namespace Umbraco.DocTypeBuilder.Tests;

/// <summary>
/// Unit tests for TemplateBuilder class - testing in isolation
/// </summary>
public class TemplateBuilderTests
{
    private readonly Mock<IShortStringHelper> _mockShortStringHelper;

    public TemplateBuilderTests()
    {
        _mockShortStringHelper = new Mock<IShortStringHelper>();
        // Setup common mock behavior
        _mockShortStringHelper
            .Setup(x => x.CleanStringForSafeAlias(It.IsAny<string>()))
            .Returns<string>(input => input?.ToLowerInvariant().Replace(" ", "") ?? string.Empty);
    }

    [Fact]
    public void TemplateBuilder_Should_Create_Basic_Template()
    {
        // Arrange
        var builder = new TemplateBuilder(_mockShortStringHelper.Object);

        // Act
        var template = builder
            .SetName("Home Page")
            .SetAlias("homePage")
            .Build();

        // Assert
        template.Should().NotBeNull();
        template.Name.Should().Be("Home Page");
        template.Alias.Should().Be("homePage");
    }

    [Fact]
    public void TemplateBuilder_Should_Set_Alias()
    {
        // Arrange
        var builder = new TemplateBuilder(_mockShortStringHelper.Object);

        // Act
        var template = builder
            .SetName("Test Template")
            .SetAlias("testTemplate")
            .Build();

        // Assert
        template.Alias.Should().Be("testTemplate");
    }

    [Fact]
    public void TemplateBuilder_Should_Set_Content()
    {
        // Arrange
        var builder = new TemplateBuilder(_mockShortStringHelper.Object);
        const string content = "@model ContentModel\n<h1>@Model.Name</h1>";

        // Act
        var template = builder
            .SetName("Content Template")
            .SetAlias("contentTemplate")
            .SetContent(content)
            .Build();

        // Assert
        template.Content.Should().Be(content);
    }

    [Fact]
    public void TemplateBuilder_Should_Set_MasterTemplate()
    {
        // Arrange
        var builder = new TemplateBuilder(_mockShortStringHelper.Object);

        // Act
        var template = builder
            .SetName("Child Template")
            .SetAlias("childTemplate")
            .SetMasterTemplate("masterTemplate")
            .Build();

        // Assert
        template.MasterTemplateAlias.Should().Be("masterTemplate");
    }

    [Fact]
    public void TemplateBuilder_Should_Chain_All_Configuration_Methods()
    {
        // Arrange
        var builder = new TemplateBuilder(_mockShortStringHelper.Object);
        const string content = "@model ContentModel\n@{\n    Layout = \"master\";\n}\n<h1>@Model.Name</h1>";

        // Act
        var template = builder
            .SetName("Complex Template")
            .SetAlias("complexTemplate")
            .SetContent(content)
            .SetMasterTemplate("masterTemplate")
            .Build();

        // Assert
        template.Should().NotBeNull();
        template.Name.Should().Be("Complex Template");
        template.Alias.Should().Be("complexTemplate");
        template.Content.Should().Be(content);
        template.MasterTemplateAlias.Should().Be("masterTemplate");
    }

    [Fact]
    public void TemplateBuilder_Should_Have_Default_Values()
    {
        // Arrange
        var builder = new TemplateBuilder(_mockShortStringHelper.Object);

        // Act
        var template = builder.Build();

        // Assert
        template.Should().NotBeNull();
        template.Alias.Should().Be("defaultTemplate");
        template.Name.Should().Be(string.Empty);
        template.Content.Should().Be(string.Empty);
        template.MasterTemplateAlias.Should().BeNullOrEmpty();
    }

    [Fact]
    public void TemplateBuilder_Should_Handle_Empty_Content()
    {
        // Arrange
        var builder = new TemplateBuilder(_mockShortStringHelper.Object);

        // Act
        var template = builder
            .SetName("Empty Template")
            .SetAlias("emptyTemplate")
            .SetContent("")
            .Build();

        // Assert
        template.Content.Should().Be("");
    }

    [Fact]
    public void TemplateBuilder_Should_Handle_Null_Values_Gracefully()
    {
        // Arrange
        var builder = new TemplateBuilder(_mockShortStringHelper.Object);

        // Act
        var template = builder
            .SetName("Null Test Template")
            .SetAlias("nullTestTemplate")
            .SetContent(null!)
            .SetMasterTemplate(null!)
            .Build();

        // Assert
        template.Should().NotBeNull();
        template.Content.Should().BeEmpty();
        template.MasterTemplateAlias.Should().BeNullOrEmpty();
    }

    [Fact]
    public void TemplateBuilder_Should_Create_Master_Template()
    {
        // Arrange
        var builder = new TemplateBuilder(_mockShortStringHelper.Object);
        const string masterContent = @"<!DOCTYPE html>
<html>
<head>
    <title>@ViewBag.Title</title>
</head>
<body>
    @RenderBody()
</body>
</html>";

        // Act
        var template = builder
            .SetName("Master Template")
            .SetAlias("masterTemplate")
            .SetContent(masterContent)
            .Build();

        // Assert
        template.Should().NotBeNull();
        template.Name.Should().Be("Master Template");
        template.Alias.Should().Be("masterTemplate");
        template.Content.Should().Be(masterContent);
        template.MasterTemplateAlias.Should().BeNullOrEmpty(); // Master templates don't have a master
    }

    [Fact]
    public void TemplateBuilder_Should_Create_Child_Template_With_Layout()
    {
        // Arrange
        var builder = new TemplateBuilder(_mockShortStringHelper.Object);
        const string childContent = @"@{
    Layout = ""masterTemplate"";
}

<h1>@Model.Name</h1>
<div>@Model.Value(""content"")</div>";

        // Act
        var template = builder
            .SetName("Page Template")
            .SetAlias("pageTemplate")
            .SetContent(childContent)
            .SetMasterTemplate("masterTemplate")
            .Build();

        // Assert
        template.Should().NotBeNull();
        template.Name.Should().Be("Page Template");
        template.Alias.Should().Be("pageTemplate");
        template.Content.Should().Be(childContent);
        template.MasterTemplateAlias.Should().Be("masterTemplate");
    }
}