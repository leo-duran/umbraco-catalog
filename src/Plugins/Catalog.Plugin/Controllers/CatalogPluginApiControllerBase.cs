using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;

namespace Catalog.Plugin.Controllers
{
    [ApiController]
    [BackOfficeRoute("catalogplugin/api/v{version:apiVersion}")]
    [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
    [MapToApi(Constants.ApiName)]
    public class CatalogPluginApiControllerBase : ControllerBase
    {
    }
}
