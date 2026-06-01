using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Extension.Dtos;
using Umbraco.Extension.Models;

namespace Umbraco.Extension.Services
{
    public interface IPhotoService
    {
        Task<IReadOnlyList<Photos>> UploadToAlbumAsync(
            PhotoDto dto,
            CancellationToken cancellationToken = default);
    }
}
