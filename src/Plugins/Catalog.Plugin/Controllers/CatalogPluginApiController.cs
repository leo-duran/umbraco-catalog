using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace Catalog.Plugin.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "Catalog.Plugin")]
    public class CatalogPluginApiController : CatalogPluginApiControllerBase
    {
        private readonly IContentTypeService _contentTypeService;

        public CatalogPluginApiController(IContentTypeService contentTypeService)
        {
            _contentTypeService = contentTypeService;
        }

        [HttpGet("ping")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        public string Ping() => "Pong";

        [HttpGet("content-types")]
        [ProducesResponseType<IEnumerable<ContentTypeDto>>(StatusCodes.Status200OK)]
        public IActionResult GetContentTypes()
        {
            var contentTypes = _contentTypeService.GetAll();
            var result = contentTypes.Select(ct => new ContentTypeDto
            {
                Id = ct.Id,
                Key = ct.Key,
                Alias = ct.Alias,
                Name = ct.Name,
                Description = ct.Description,
                Icon = ct.Icon,
                IsElement = ct.IsElement,
                AllowedAsRoot = ct.AllowedAsRoot,
                PropertyGroups = ct.PropertyGroups.Select(pg => new PropertyGroupDto
                {
                    Id = pg.Id,
                    Key = pg.Key,
                    Name = pg.Name,
                    Alias = pg.Alias,
                    SortOrder = pg.SortOrder,
                    PropertyTypes = pg.PropertyTypes.Select(pt => new PropertyTypeDto
                    {
                        Id = pt.Id,
                        Key = pt.Key,
                        Alias = pt.Alias,
                        Name = pt.Name,
                        Description = pt.Description,
                        Mandatory = pt.Mandatory,
                        DataTypeId = pt.DataTypeId,
                        DataTypeKey = pt.DataTypeKey,
                        SortOrder = pt.SortOrder
                    }).ToList()
                }).ToList()
            });

            return Ok(result);
        }
    }

    public class ContentTypeDto
    {
        public int Id { get; set; }
        public Guid Key { get; set; }
        public string Alias { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public bool IsElement { get; set; }
        public bool AllowedAsRoot { get; set; }
        public List<PropertyGroupDto> PropertyGroups { get; set; }
    }

    public class PropertyGroupDto
    {
        public int Id { get; set; }
        public Guid Key { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public int SortOrder { get; set; }
        public List<PropertyTypeDto> PropertyTypes { get; set; }
    }

    public class PropertyTypeDto
    {
        public int Id { get; set; }
        public Guid Key { get; set; }
        public string Alias { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Mandatory { get; set; }
        public int DataTypeId { get; set; }
        public Guid DataTypeKey { get; set; }
        public int SortOrder { get; set; }
    }
}
