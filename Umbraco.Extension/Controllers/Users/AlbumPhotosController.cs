using Asp.Versioning;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Extension.Dtos;
using Umbraco.Extension.Dtos.Filters;
using Umbraco.Extension.Models;
using Umbraco.Extension.Repositories;
using Umbraco.Extension.Services;
using Umbraco.Extension.Services.Interface;

namespace Umbraco.Extension.Controllers.Users;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "PhotosAddedByUsers")]
public class AlbumPhotosController(IPhotoService photoService, AlbumService albumService)
    : UmbracoExtensionApiControllerBase
{
    // This function is for testing purposes only - using swagger
    [HttpGet("photos/{photoId:guid}/file")]
    [Produces("image/jpeg")]
    public async Task<IActionResult> GetPhotoFile(
        IAzureBlobPhotoStorageService blobStorageService,
        Guid photoId,
        CancellationToken cancellationToken)
    {
        var photo = await photoService.GetByIdAsync(photoId, cancellationToken);
        if (photo is null)
            return NotFound();

        var blobStream = await blobStorageService.GetAsync(photo.BlobPath, cancellationToken);

        return File(blobStream, "application/octet-stream", photo.OriginalFileName);
    }

    [HttpGet("albums/{albumId:guid}/photos")]
    public async Task<ActionResult<IReadOnlyList<PhotoDto>>> GetPhotosByAlbumId(
        IAzureBlobPhotoStorageService blobStorageService,
        Guid albumId,
        [FromQuery] Pagination filter,
        CancellationToken cancellationToken)
    {
        var photos = await photoService.GetPhotosByAlbumIdAsync(
            albumId,
            filter,
            cancellationToken);

        foreach (var photo in photos)
        {
            photo.Link = Url.Action(
                action: nameof(GetPhotoFile),
                controller: "AlbumPhotos",
                values: new { photoId = photo.Id },
                protocol: Request.Scheme
            )!;
        }

        return Ok(photos);
    }

    [HttpGet("bookings/{bookingId:guid}/album")]
    public async Task<ActionResult<IReadOnlyList<PhotoDto>>> GetAlbumByBookingId(
        Guid bookingId,
        CancellationToken cancellationToken)
    {
        var album = await albumService.GetAlbumByBookingIdAsync(
            bookingId);
        
        if(album is null)
            return NotFound();

        return Ok(album);
    }

    [HttpPost("albums/{albumId:guid}/photos")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<IReadOnlyList<PhotoDto>>> UploadPhotos(
        Guid albumId,
        [FromForm] PhotoDto dto,
        CancellationToken cancellationToken)
    {
        dto.AlbumId = albumId;
        var result = await photoService.UploadToAlbumAsync(
            dto,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{bookingId:guid}")]
    public async Task<ActionResult<AlbumDto>> CreateAlbum(
        Guid bookingId,
        [FromBody] AlbumDto dto,
        CancellationToken cancellationToken)
    {
        var album = new Albums
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            Name = dto.Name,
            CreatedOn = DateTimeOffset.UtcNow,
            UpdatedOn = DateTimeOffset.UtcNow
        };

        var result = await albumService.CreateAsync(
            album,
            cancellationToken);

        return Ok(result);
    }
}