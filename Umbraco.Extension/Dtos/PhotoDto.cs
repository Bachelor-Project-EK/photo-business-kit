using Microsoft.AspNetCore.Http;

namespace Umbraco.Extension.Dtos;

public class PhotoDto
{

    public required Guid AlbumId { get; set; }

    public List<IFormFile>? Files { get; init; }


}