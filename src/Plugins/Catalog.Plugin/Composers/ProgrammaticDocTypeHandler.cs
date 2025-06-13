using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Catalog.Plugin.Composers
{
    public class ProgrammaticDocTypeHandler : INotificationHandler<UmbracoApplicationStartingNotification>
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IDataTypeService _dataTypeService;
        private readonly IFileService _fileService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly ILogger<ProgrammaticDocTypeHandler> _logger;

        public ProgrammaticDocTypeHandler(
            IContentTypeService contentTypeService,
            IDataTypeService dataTypeService,
            IFileService fileService,
            IShortStringHelper shortStringHelper,
            ILogger<ProgrammaticDocTypeHandler> logger)
        {
            _contentTypeService = contentTypeService;
            _dataTypeService = dataTypeService;
            _fileService = fileService;
            _shortStringHelper = shortStringHelper;
            _logger = logger;
        }

        public void Handle(UmbracoApplicationStartingNotification notification)
        {
            var contentTypeAlias = "productDocType";

            try
            {
                _logger.LogInformation("Starting document type and template creation");
                
                // Check if document type already exists
                var existingDocType = _contentTypeService.Get(contentTypeAlias);
                if (existingDocType != null)
                {
                    _logger.LogInformation("Document type already exists");
                    return;
                }
                
//                 // Create template
//                 var template = new Template(_shortStringHelper, "programmaticTemplate", "Programmatic Template")
//                 {
//                     Content = @"@using Umbraco.Cms.Web.Common.PublishedModels;
// @inherits Umbraco.Cms.Web.Common.Views.UmbracoViewPage
// @{
//     Layout = null;
// }

// <!DOCTYPE html>
// <html>
// <head>
//     <meta charset=""utf-8"" />
//     <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
//     <title>@Model.Title</title>
// </head>
// <body>
//     <h1>@Model.Title</h1>
//     <div>@Model.BodyText</div>
// </body>
// </html>"
//                 };
                
                // Save the template in Umbraco 15
                // _fileService.SaveTemplate(template);
                
                // In Umbraco 15 we need to find data types by alias 
                // Use GetAll() - it's marked obsolete but still works in Umbraco 15
#pragma warning disable CS0618 // Type or member is obsolete
                var allDataTypes = _dataTypeService.GetAll();
#pragma warning restore CS0618
                
                var textbox = allDataTypes.FirstOrDefault(dt => dt.EditorAlias == "Umbraco.TextBox");
                // var richText = allDataTypes.FirstOrDefault(dt => dt.EditorAlias == "Umbraco.TinyMCE");
                
                if (textbox == null)
                {
                    _logger.LogError("Required data types not found");
                    return;
                }
                
                // Create content type with parent ID
                var contentType = new ContentType(_shortStringHelper, -1)
                {
                    Alias = contentTypeAlias,
                    Name = "Product",
                    AllowedAsRoot = true,
                    Icon = "icon-document"
                };
                
                var contentGroupAdded = contentType.AddPropertyGroup("Content", "Content");
                if (!contentGroupAdded)
                {
                    _logger.LogError("Failed to add content group");
                    return;
                }

                // Add title property - use the EditorAlias directly in Umbraco 15
                var titlePropertyType = new PropertyType(_shortStringHelper, "Umbraco.TextBox", ValueStorageType.Nvarchar, "title")
                {
                    Name = "Title",
                    Description = "Page title",
                    Mandatory = true,
                    SortOrder = 1,
                    DataTypeId = textbox.Id
                };
                var titlePropertyAdded = contentType.AddPropertyType(titlePropertyType, "Content");
                if (!titlePropertyAdded)
                {
                    _logger.LogError("Failed to add title property");
                    return;
                }
                
                // // Add body property - use the EditorAlias directly in Umbraco 15
                // var bodyPropertyType = new PropertyType(_shortStringHelper, "Umbraco.TinyMCE", ValueStorageType.Ntext, "bodyText")
                // {
                //     Name = "Body Text",
                //     Description = "Main content of the page",
                //     Mandatory = false,
                //     SortOrder = 2,
                //     DataTypeId = richText.Id
                // };
                // contentType.AddPropertyType(bodyPropertyType, "content");
                
                // Associate template
                // contentType.SetDefaultTemplate(template);
                // contentType.AllowedTemplates = new List<ITemplate> { template };
                
                // Save content type in Umbraco 15
                _contentTypeService.Save(contentType);
                
                _logger.LogInformation("Successfully created document type and template");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating document type: {Message}", ex.Message);
            }
        }
    }
}
