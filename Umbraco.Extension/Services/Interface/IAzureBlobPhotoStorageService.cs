using Microsoft.AspNetCore.Http;

namespace Umbraco.Extension.Services.Interface
{
    public interface IAzureBlobPhotoStorageService
    {
        Task<Stream> GetAsync(
            string blobPath,
            CancellationToken cancellationToken = default);

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
