using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Extension.Dtos;
using Umbraco.Extension.Services;

namespace Umbraco.Extension.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = Constants.ApiName)]
public class EventTypeController : UmbracoExtensionApiControllerBase
{
    private readonly EventTypeService _eventTypeService;
    private readonly IValidator<EventTypeDto> _validator;

    public EventTypeController(
        EventTypeService eventTypeService,
        IValidator<EventTypeDto> validator)
    {
        _eventTypeService = eventTypeService;
        _validator = validator;
    }

    [AllowAnonymous]
    [HttpGet("eventtypes")]
    [ProducesResponseType(typeof(IEnumerable<EventTypeDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var eventTypes = _eventTypeService.GetAll();

        return Ok(eventTypes);
    }

    [AllowAnonymous]
    [HttpPost("eventtypes")]
    [ProducesResponseType(typeof(EventTypeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(EventTypeDto dto)
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
