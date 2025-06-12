using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Plugin.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "Catalog.Plugin")]
    public class CatalogPluginApiController : CatalogPluginApiControllerBase
    {

        [HttpGet("ping")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        public string Ping() => "Pong";
    }
}
