using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Extension.Dtos.Commands;
using Umbraco.Extension.Dtos.Queries;
using Umbraco.Extension.Services;

namespace Umbraco.Extension.Controllers.Users;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Payments")]
public class PaymentController : UmbracoExtensionApiControllerBase
{
    private readonly PaymentService _paymentService;
    private readonly IValidator<PaymentCommandDto> _validator;

    public PaymentController(
        PaymentService paymentService,
        IValidator<PaymentCommandDto> validator)
    {
        _paymentService = paymentService;
        _validator = validator;
    }

    [HttpGet("payments")]
    [ProducesResponseType(typeof(IEnumerable<PaymentQueryDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var payments = _paymentService.GetAll();

        return Ok(payments);
    }

    [HttpPost("payments")]
    [ProducesResponseType(typeof(PaymentCommandDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(PaymentCommandDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var createdPayment = await _paymentService.CreateAsync(dto);

        return Created(string.Empty, createdPayment);
    }

    [HttpPut("payments/{id:guid}")]
    [ProducesResponseType(typeof(PaymentCommandDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] PaymentCommandDto dto, [FromRoute] Guid id)
    {
        var validationResult = await _validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updatedPayment = await _paymentService.UpdateAsync(dto, id);

        if (updatedPayment is null)
        {
            return NotFound();
        }

        return Ok(updatedPayment);
    }

    [HttpDelete("payments/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _paymentService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
