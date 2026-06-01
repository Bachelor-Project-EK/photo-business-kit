using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Extension.Dtos;
using Umbraco.Extension.Dtos.Filters;
using Umbraco.Extension.Models;
using Umbraco.Extension.Services;

namespace Umbraco.Extension.Controllers.Users;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Bookings")]
[Route("[controller]")]
public class BookingsController : UmbracoExtensionApiControllerBase
{
    private readonly IValidator<BookingDto> _validator;
    private readonly BookingService _bookingService;

    public BookingsController(
        IValidator<BookingDto> validator,
        BookingService bookingService)
    {
        _validator = validator;
        _bookingService = bookingService;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Bookings), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        [FromRoute] Guid id, 
        CancellationToken cancellationToken)
    {
        var booking = await _bookingService.GetAsync(id, cancellationToken);
        if (booking is null)
        {
            return NotFound();
        }
        return Ok(booking);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Bookings>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Pagination bookingFilter,
        CancellationToken cancellationToken)
    {
        return Ok(await _bookingService.GetAllAsync(bookingFilter, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] BookingDto dto,
        [FromQuery] string email, 
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        return Created(string.Empty, await _bookingService.CreateAsync(dto, email, cancellationToken));
    }

    [HttpPut("{id}/propose")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Propose(
        [FromRoute] Guid id, 
        [FromBody] BookingDto dto, 
        CancellationToken cancellationToken)
    {
        var result = await _bookingService.ProposeUpdateAsync(id, dto, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPut("{id}/approve")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(
        [FromRoute] Guid id, 
        CancellationToken cancellationToken)
    {
        var result = await _bookingService.ApproveAsync(id, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPut("{id}/reject")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(
        [FromRoute] Guid id, 
        CancellationToken cancellationToken)
    {
        var result = await _bookingService.RejectAsync(id, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id, 
        CancellationToken cancellationToken)
    {
        var result = await _bookingService.DeleteAsync(id, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}