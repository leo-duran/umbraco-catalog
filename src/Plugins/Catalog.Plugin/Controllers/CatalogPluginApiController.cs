using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Catalog.Plugin.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "Catalog.Plugin")]
    public class CatalogPluginApiController : CatalogPluginApiControllerBase
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IDataTypeService _dataTypeService;
        private readonly ILogger<CatalogPluginApiController> _logger;

        public CatalogPluginApiController(
            IContentTypeService contentTypeService,
            IDataTypeService dataTypeService,
            ILogger<CatalogPluginApiController> logger)
        {
            _contentTypeService = contentTypeService;
            _dataTypeService = dataTypeService;
            _logger = logger;
        }

        [HttpGet("ping")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        public string Ping() => "Pong";

        [HttpGet("content-types")]
        [ProducesResponseType<IEnumerable<ContentTypeDto>>(StatusCodes.Status200OK)]
        public IActionResult GetContentTypes()
        {
            _logger.LogInformation("Getting all content types");
            var contentTypes = _contentTypeService.GetAll();
            var result = contentTypes.Select(ct => new ContentTypeDto
            {
                Id = ct.Id,
                Key = ct.Key,
                Alias = ct.Alias ?? string.Empty,
                Name = ct.Name ?? string.Empty,
                Description = ct.Description ?? string.Empty,
                Icon = ct.Icon ?? string.Empty,
                IsElement = ct.IsElement,
                AllowedAsRoot = ct.AllowedAsRoot,
                PropertyGroups = ct.PropertyGroups?.Select(pg => new PropertyGroupDto
                {
                    Id = pg.Id,
                    Key = pg.Key,
                    Name = pg.Name ?? string.Empty,
                    Alias = pg.Alias ?? string.Empty,
                    SortOrder = pg.SortOrder,
                    PropertyTypes = pg.PropertyTypes?.Select(pt => new PropertyTypeDto
                    {
                        Id = pt.Id,
                        Key = pt.Key,
                        Alias = pt.Alias ?? string.Empty,
                        Name = pt.Name ?? string.Empty,
                        Description = pt.Description ?? string.Empty,
                        Mandatory = pt.Mandatory,
                        DataTypeId = pt.DataTypeId,
                        DataTypeKey = pt.DataTypeKey,
                        SortOrder = pt.SortOrder
                    }).ToList() ?? new List<PropertyTypeDto>()
                }).ToList() ?? new List<PropertyGroupDto>()
            });

            return Ok(result);
        }

        [HttpGet("content-types/{alias}")]
        [ProducesResponseType<ContentTypeDetailDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetContentTypeByAlias(string alias)
        {
            _logger.LogInformation($"Getting content type by alias: {alias}");
            var contentType = _contentTypeService.Get(alias);
            if (contentType == null)
            {
                _logger.LogWarning($"Content type with alias '{alias}' not found");
                return NotFound($"Content type with alias '{alias}' not found");
            }

            var propertyGroups = contentType.PropertyGroups?.Select(pg => new PropertyGroupDetailDto
            {
                Id = pg.Id,
                Key = pg.Key,
                Name = pg.Name ?? string.Empty,
                Alias = pg.Alias ?? string.Empty,
                SortOrder = pg.SortOrder,
                PropertyTypes = pg.PropertyTypes?.Select(pt =>
                {
                    var dataType = _dataTypeService.GetDataType(pt.DataTypeId);
                    return new PropertyTypeDetailDto
                    {
                        Id = pt.Id,
                        Key = pt.Key,
                        Alias = pt.Alias ?? string.Empty,
                        Name = pt.Name ?? string.Empty,
                        Description = pt.Description ?? string.Empty,
                        Mandatory = pt.Mandatory,
                        ValidationRegExp = pt.ValidationRegExp ?? string.Empty,
                        DataType = dataType != null ? new DataTypeDto
                        {
                            Id = dataType.Id,
                            Key = dataType.Key,
                            Name = dataType.Name ?? string.Empty,
                            EditorAlias = dataType.EditorAlias ?? string.Empty,
                            DatabaseType = pt.ValueStorageType.ToString()
                        } : null!,
                        SortOrder = pt.SortOrder
                    };
                }).ToList() ?? new List<PropertyTypeDetailDto>()
            }).ToList() ?? new List<PropertyGroupDetailDto>();

            var compositions = contentType.ContentTypeComposition?.Select(ct => new ContentTypeReferenceDto
            {
                Id = ct.Id,
                Key = ct.Key,
                Alias = ct.Alias ?? string.Empty,
                Name = ct.Name ?? string.Empty,
                Icon = ct.Icon ?? string.Empty
            }).ToList() ?? new List<ContentTypeReferenceDto>();

            var allowedContentTypes = contentType.AllowedContentTypes?.Select(act => new AllowedContentTypeDto
            {
                Id = act.Alias != null ? _contentTypeService.Get(act.Alias)?.Id ?? 0 : 0,
                Alias = act.Alias ?? string.Empty,
                SortOrder = act.SortOrder
            }).ToList() ?? new List<AllowedContentTypeDto>();

            var result = new ContentTypeDetailDto
            {
                Id = contentType.Id,
                Key = contentType.Key,
                Alias = contentType.Alias ?? string.Empty,
                Name = contentType.Name ?? string.Empty,
                Description = contentType.Description ?? string.Empty,
                Icon = contentType.Icon ?? string.Empty,
                IsElement = contentType.IsElement,
                AllowedAsRoot = contentType.AllowedAsRoot,
                PropertyGroups = propertyGroups.Cast<PropertyGroupDto>().ToList(),
                Compositions = compositions,
                AllowedContentTypes = allowedContentTypes,
                CreateDate = contentType.CreateDate,
                UpdateDate = contentType.UpdateDate,
                DefaultTemplate = contentType.DefaultTemplate?.Name
            };

            _logger.LogInformation($"Successfully retrieved content type: {contentType.Name}");
            return Ok(result);
        }
    }

    public class ContentTypeDto
    {
        public int Id { get; set; }
        public Guid Key { get; set; }
        public string Alias { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public bool IsElement { get; set; }
        public bool AllowedAsRoot { get; set; }
        public List<PropertyGroupDto> PropertyGroups { get; set; } = new();
    }

    public class PropertyGroupDto
    {
        public int Id { get; set; }
        public Guid Key { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public List<PropertyTypeDto> PropertyTypes { get; set; } = new();
    }

    public class PropertyTypeDto
    {
        public int Id { get; set; }
        public Guid Key { get; set; }
        public string Alias { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Mandatory { get; set; }
        public int DataTypeId { get; set; }
        public Guid DataTypeKey { get; set; }
        public int SortOrder { get; set; }
    }

    public class ContentTypeDetailDto : ContentTypeDto
    {
        public List<ContentTypeReferenceDto> Compositions { get; set; } = new();
        public List<AllowedContentTypeDto> AllowedContentTypes { get; set; } = new();
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string? DefaultTemplate { get; set; }
    }

    public class ContentTypeReferenceDto
    {
        public int Id { get; set; }
        public Guid Key { get; set; }
        public string Alias { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

    public class PropertyGroupDetailDto : PropertyGroupDto
    {
        public new List<PropertyTypeDetailDto> PropertyTypes { get; set; } = new();
    }

    public class PropertyTypeDetailDto : PropertyTypeDto
    {
        public string ValidationRegExp { get; set; } = string.Empty;
        public DataTypeDto? DataType { get; set; }
    }

    public class DataTypeDto
    {
        public int Id { get; set; }
        public Guid Key { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EditorAlias { get; set; } = string.Empty;
        public string DatabaseType { get; set; } = string.Empty;
    }

    public class AllowedContentTypeDto
    {
        public int Id { get; set; }
        public string Alias { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
