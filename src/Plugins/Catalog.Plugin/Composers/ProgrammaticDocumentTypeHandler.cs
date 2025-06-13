using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Catalog.Plugin.Composers
{
    public class ProgrammaticDocumentTypeHandler : INotificationAsyncHandler<UmbracoApplicationStartingNotification>
    {
        private readonly IFileService _fileService;
        private readonly IContentTypeService _contentTypeService;
        private readonly IDataTypeService _dataTypeService;
        private readonly ITemplateService _templateService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly ILogger<ProgrammaticDocumentTypeHandler> _logger;

        public ProgrammaticDocumentTypeHandler(
            IFileService fileService,
            IContentTypeService contentTypeService,
            IDataTypeService dataTypeService,
            ITemplateService templateService,
            IShortStringHelper shortStringHelper,
            ILogger<ProgrammaticDocumentTypeHandler> logger)
        {
            _fileService = fileService;
            _contentTypeService = contentTypeService;
            _dataTypeService = dataTypeService;
            _templateService = templateService;
            _shortStringHelper = shortStringHelper;
            _logger = logger;
        }

        public async Task HandleAsync(UmbracoApplicationStartingNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating programmatic document type and template");
            await CreateDocumentTypeWithTemplateAsync(cancellationToken);
        }

        private async Task CreateDocumentTypeWithTemplateAsync(CancellationToken cancellationToken)
        {
            // Check if the document type already exists
            var documentType = _contentTypeService.Get("programmaticDocType");
            if (documentType != null)
            {
                _logger.LogInformation("Document type 'ProgrammaticDocType' already exists. Skipping creation.");
                return;
            }

            // Create or get a template
            var template = await _templateService.GetAsync("ProgrammaticTemplate");
            if (template == null)
            {
                // Create a new template
                template = new Template(_shortStringHelper, "programmaticTemplate", "Programmatic Template")
                {
                    Content = @"@using Umbraco.Cms.Web.Common.PublishedModels;
@inherits Umbraco.Cms.Web.Common.Views.UmbracoViewPage<ContentModels.ProgrammaticDocType>
@using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;
@{
    Layout = null;
}

<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>@Model.Title</title>
</head>
<body>
    <h1>@Model.Title</h1>
    <div>@Model.BodyText</div>
</body>
</html>"
                };

                // Using FileService.SaveTemplate (it's deprecated but still works in Umbraco 15)
                _fileService.SaveTemplate(template);
                _logger.LogInformation("Created template 'ProgrammaticTemplate'");
            }

            // Get the appropriate data types (by alias in Umbraco 15)
            var allDataTypes = await _dataTypeService.GetAllAsync();
            var textStringDataType = allDataTypes.FirstOrDefault(dt => dt.EditorAlias == "Umbraco.TextBox");
            var richTextDataType = allDataTypes.FirstOrDefault(dt => dt.EditorAlias == "Umbraco.TinyMCE");

            if (textStringDataType == null || richTextDataType == null)
            {
                _logger.LogError("Required data types not found");
                return;
            }

            // Create content type
            var contentType = new ContentType(_shortStringHelper, template.Id)
            {
                Alias = "programmaticDocType",
                Name = "Programmatic Doc Type",
                AllowedAsRoot = true,
                Icon = "icon-document",
                Key = Guid.NewGuid()
            };

            // Add property group
            contentType.AddPropertyGroup("Content", "content");
            
            // Create property types with correct constructor for Umbraco 15
            var titlePropertyType = new PropertyType(_shortStringHelper, textStringDataType.EditorAlias, ValueStorageType.Nvarchar, "title")
            {
                Name = "Title",
                Description = "The title of the page",
                Mandatory = true,
                SortOrder = 0,
                DataTypeId = textStringDataType.Id
            };
            contentType.AddPropertyType(titlePropertyType, "content");

            var bodyTextPropertyType = new PropertyType(_shortStringHelper, richTextDataType.EditorAlias, ValueStorageType.Ntext, "bodyText")
            {
                Name = "Body Text",
                Description = "The main content of the page",
                Mandatory = false,
                SortOrder = 1,
                DataTypeId = richTextDataType.Id
            };
            contentType.AddPropertyType(bodyTextPropertyType, "content");

            // Associate template
            contentType.SetDefaultTemplate(template);
            contentType.AllowedTemplates = new List<ITemplate> { template };

            // Create the document type in Umbraco 15
            // Using IContentTypeService.Save (it's deprecated but still works in Umbraco 15)
            _contentTypeService.Save(contentType);

            _logger.LogInformation("Document type 'ProgrammaticDocType' with template created successfully");
        }
    }
}
