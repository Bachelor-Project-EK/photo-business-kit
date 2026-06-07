using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.Filters;
using Umbraco.Extension.Dtos.Filters;
using Umbraco.Extension.Models;
using Umbraco.Extension.Services;

namespace Umbraco.Extension.Controllers.Members;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Test")]
[MapToApi(Constants.SurfaceApiName)]
[UmbracoMemberAuthorize]
public class BookingsController : ControllerBase
{
    private readonly BookingService _bookingService;

    public BookingsController(BookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet("GetAll")]
    [ProducesResponseType(typeof(IEnumerable<Bookings>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBookings([FromQuery] Pagination? pagination, CancellationToken cancellationToken)
    {
        pagination ??= new Pagination();
        var eventTypes = await _bookingService.GetAllAsync(pagination, cancellationToken);

        return Ok(eventTypes);
    }
}
