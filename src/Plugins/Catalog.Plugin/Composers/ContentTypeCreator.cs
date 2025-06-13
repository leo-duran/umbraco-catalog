using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Catalog.Plugin.Composers
{
    public class ContentTypeCreator : INotificationAsyncHandler<UmbracoApplicationStartingNotification>
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IDataTypeService _dataTypeService;
        private readonly ITemplateService _templateService;
        private readonly IFileService _fileService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly ICoreScopeProvider _scopeProvider;

        public ContentTypeCreator(
            IContentTypeService contentTypeService,
            IDataTypeService dataTypeService,
            ITemplateService templateService,
            IShortStringHelper shortStringHelper,
            ICoreScopeProvider scopeProvider,
            IFileService fileService)
        {
            _contentTypeService = contentTypeService;
            _dataTypeService = dataTypeService;
            _templateService = templateService;
            _shortStringHelper = shortStringHelper;
            _scopeProvider = scopeProvider;
            _fileService = fileService;
        }

        public async Task HandleAsync(UmbracoApplicationStartingNotification notification, CancellationToken cancellationToken)
        {
            using (var scope = _scopeProvider.CreateCoreScope())
            {
                // Create a base composition type
                var metaComposition = new ContentType(_shortStringHelper, -1)
                {
                    Alias = "metaComposition",
                    Name = "Meta Composition",
                    Description = "Common meta properties",
                    Icon = "icon-document"
                };

                // Add meta properties in a "Meta" tab
                var metaTab = new PropertyGroup(new PropertyTypeCollection(true))
                {
                    Name = "Meta",
                    Alias = "meta",
                    SortOrder = 1,
                    Type = PropertyGroupType.Tab
                };
                metaComposition.PropertyGroups.Add(metaTab);

                metaComposition.AddPropertyType(new PropertyType(_shortStringHelper, "Umbraco.TextBox", ValueStorageType.Nvarchar, "metaDescription")
                {
                    Alias = "metaDescription",
                    Name = "Meta Description",
                    Description = "SEO meta description",
                    SortOrder = 1
                }, "meta");

                // Save the composition
                _contentTypeService.Save(metaComposition);

                // Create a main content type that uses the composition
                var pageType = new ContentType(_shortStringHelper, -1)
                {
                    Alias = "samplePage",
                    Name = "Sample Page",
                    Description = "Standard page type",
                    Icon = "icon-document",
                    AllowedAsRoot = true
                };

                // Add a "Content" tab
                var contentTab = new PropertyGroup(new PropertyTypeCollection(true))
                {
                    Name = "Content",
                    Alias = "content",
                    SortOrder = 1,
                    Type = PropertyGroupType.Tab
                };
                pageType.PropertyGroups.Add(contentTab);

                // Add a "Header" group within the Content tab
                var headerGroup = new PropertyGroup(new PropertyTypeCollection(true))
                {
                    Name = "Header",
                    Alias = "header",
                    SortOrder = 1,
                    Type = PropertyGroupType.Group
                };
                pageType.PropertyGroups.Add(headerGroup);

                // Add title property to the Header group
                pageType.AddPropertyType(new PropertyType(_shortStringHelper, "Umbraco.TextBox", ValueStorageType.Nvarchar, "title")
                {
                    Alias = "title",
                    Name = "Title",
                    Description = "Page title",
                    Mandatory = true,
                    SortOrder = 1
                }, "header");

                // Add hero image property to the Header group
                pageType.AddPropertyType(new PropertyType(_shortStringHelper, "Umbraco.ImageCropper", ValueStorageType.Nvarchar, "heroImage")
                {
                    Alias = "heroImage",
                    Name = "Hero Image",
                    Description = "Main header image",
                    SortOrder = 2
                }, "header");

                // Add a "Main Content" group within the Content tab
                var mainContentGroup = new PropertyGroup(new PropertyTypeCollection(true))
                {
                    Name = "Main Content",
                    Alias = "mainContent",
                    SortOrder = 2,
                    Type = PropertyGroupType.Group
                };
                pageType.PropertyGroups.Add(mainContentGroup);

                // Add body text property to the Main Content group
                pageType.AddPropertyType(new PropertyType(_shortStringHelper, "Umbraco.RichtextEditor", ValueStorageType.Nvarchar, "bodyText")
                {
                    Alias = "bodyText",
                    Name = "Body Text",
                    Description = "Main content",
                    SortOrder = 1
                }, "mainContent");

                // Add the composition (this will add the Meta tab from the composition)
                pageType.AddContentType(metaComposition);

                // Set up templates - use consistent naming
                const string templateAlias = "pageTemplate";
                const string templateName = "Page Template";

                // Try to get existing template first
                var template = await _templateService.GetAsync(templateAlias);
                if (template == null)
                {
                    // Create a new template with consistent naming
                    template = new Template(_shortStringHelper, templateAlias, templateName)
                    {
                        Content = GetDefaultTemplateContent()
                    };

                    // Save the template
                    _fileService.SaveTemplate(template);
                }

                pageType.AllowedTemplates = [template];
                pageType.SetDefaultTemplate(template);

                // Save the content type
                _contentTypeService.Save(pageType);

                scope.Complete();
            }

            return;
        }

        private string GetDefaultTemplateContent()
        {
            return @"@using Umbraco.Cms.Web.Common.PublishedModels;
@inherits Umbraco.Cms.Web.Common.Views.UmbracoViewPage

@{
    Layout = null;
}

<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>@Model.Title</title>
    @if (!string.IsNullOrEmpty(Model.MetaDescription))
    {
        <meta name=""description"" content=""@Model.MetaDescription"">
    }
</head>
<body>
    <header>
        <h1>@Model.Title</h1>
        @if (Model.HeroImage != null)
        {
            <img src=""@Model.HeroImage.Url()"" alt=""@Model.Title"">
        }
    </header>
    
    <main>
        @if (!string.IsNullOrEmpty(Model.BodyText))
        {
            @Html.Raw(Model.BodyText)
        }
    </main>
    
    <footer>
        &copy; @DateTime.Now.Year - Your Website
    </footer>
</body>
</html>";
        }
    }
}