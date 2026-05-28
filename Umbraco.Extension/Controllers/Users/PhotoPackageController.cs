using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Extension.Dtos.Commands;
using Umbraco.Extension.Dtos.Queries;
using Umbraco.Extension.Services;

namespace Umbraco.Extension.Controllers.Users;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "PhotoPackages")]
public class PhotoPackageController : UmbracoExtensionApiControllerBase
{
    private readonly PhotoPackageService _photoPackageService;
    private readonly IValidator<PhotoPackageCommandDto> _validator;

    public PhotoPackageController(
        PhotoPackageService photoPackageService,
        IValidator<PhotoPackageCommandDto> validator)
    {
        _photoPackageService = photoPackageService;
        _validator = validator;
    }

    [HttpGet("photopackages")]
    [ProducesResponseType(typeof(IEnumerable<PhotoPackageQueryDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var photoPackages = _photoPackageService.GetAll();

        return Ok(photoPackages);
    }

    [HttpPost("photopackages")]
    [ProducesResponseType(typeof(PhotoPackageCommandDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(PhotoPackageCommandDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var createdPhotoPackage = await _photoPackageService.CreateAsync(dto);

        return Created(string.Empty, createdPhotoPackage);
    }

    [HttpPut("photopackages/{id:guid}")]
    [ProducesResponseType(typeof(PhotoPackageCommandDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] PhotoPackageCommandDto dto, [FromRoute] Guid id)
    {
        var validationResult = await _validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updatedPhotoPackage = await _photoPackageService.UpdateAsync(dto, id);

        if (updatedPhotoPackage is null)
        {
            return NotFound();
        }

        return Ok(updatedPhotoPackage);
    }

    [HttpDelete("photopackages/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _photoPackageService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
