using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Extension.Dtos.Commands;
using Umbraco.Extension.Dtos.Queries;
using Umbraco.Extension.Services;

namespace Umbraco.Extension.Controllers.Users;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = Constants.ApiName)]
public class EventTypeController : UmbracoExtensionApiControllerBase
{
    private readonly EventTypeService _eventTypeService;
    private readonly IValidator<EventTypeCommandDto> _validator;

    public EventTypeController(
        EventTypeService eventTypeService,
        IValidator<EventTypeCommandDto> validator)
    {
        _eventTypeService = eventTypeService;
        _validator = validator;
    }
 
    [HttpGet("eventtypes")]
    [ProducesResponseType(typeof(IEnumerable<EventTypeQueryDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var eventTypes = _eventTypeService.GetAll();

        return Ok(eventTypes);
    }

    [HttpPost("eventtypes")]
    [ProducesResponseType(typeof(EventTypeCommandDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(EventTypeCommandDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var createdEventType = await _eventTypeService.CreateAsync(dto);

        return Created(string.Empty, createdEventType);
    }
}
