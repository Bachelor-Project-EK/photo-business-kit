using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Umbraco.Extension.Services
{
    public interface IPhotoStorageService
    {
        Task<string> UploadAsync(
            Guid albumId,
            Guid photoId,
            IFormFile file,
            CancellationToken cancellationToken = default);

        Task DeleteIfExistsAsync(
            string blobName,
            CancellationToken cancellationToken = default);
    }
}
