using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extension.Dtos.Queries;
using Umbraco.Extension.Services;

namespace Umbraco.Extension.Controllers.Members;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "EventTypes")]
public class EventTypesController : UmbracoExtensionSurfaceApiControllerBase
{
    private readonly EventTypeService _eventTypeService;

    public EventTypesController(
        IUmbracoContextAccessor umbracoContextAccessor,
        IUmbracoDatabaseFactory databaseFactory,
        ServiceContext services,
        AppCaches appCaches,
        IProfilingLogger profilingLogger,
        IPublishedUrlProvider publishedUrlProvider,
        EventTypeService eventTypeService)
        : base(
            umbracoContextAccessor,
            databaseFactory,
            services,
            appCaches,
            profilingLogger,
            publishedUrlProvider)
    {
        _eventTypeService = eventTypeService;
    }

    [HttpGet("eventtypes")]
    [ProducesResponseType(typeof(IEnumerable<EventTypeQueryDto>), StatusCodes.Status200OK)]
    public IActionResult MembersGetAll()
    {
        var eventTypes = _eventTypeService.MembersGetAll();

        return Ok(eventTypes);
    }
}
