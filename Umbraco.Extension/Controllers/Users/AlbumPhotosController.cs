using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Extension.Dtos;
using Umbraco.Extension.Services;

namespace Umbraco.Extension.Controllers.Users
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ApiName)]
    public class AlbumPhotosController(IPhotoService photoService) : UmbracoExtensionApiControllerBase
    {
        private readonly IPhotoService _photoService = photoService;


        [HttpPost("/photos")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<IReadOnlyList<PhotoDto>>> UploadPhotos(
            [FromBody] PhotoDto dto,
            CancellationToken cancellationToken)
        {

            var result = await photoService.UploadToAlbumAsync(
                dto,
                cancellationToken);

            return Ok();
        }
    }




}
