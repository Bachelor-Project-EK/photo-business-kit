using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace Umbraco.Extension.Services
{
    public class AzureBlobPhotoStorageService(BlobContainerClient containerClient) : IPhotoStorageService
    {
        public async Task<string> UploadAsync(Guid albumId, Guid photoId, IFormFile file, CancellationToken cancellationToken = default)
        {
            var extension = Path
                .GetExtension(file.FileName)
                .ToLowerInvariant();

            var blobName =
                $"albums/{albumId:N}/{photoId:N}{extension}";

            var blobClient = containerClient.GetBlobClient(blobName);

            await using var stream = file.OpenReadStream();

            // Her we send the file stream directly to Azure Blob Storage
            await blobClient.UploadAsync(
                stream,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = file.ContentType
                    }
                },
                cancellationToken);

            return blobName;

        }

        public async Task DeleteIfExistsAsync(
            string blobName,
            CancellationToken cancellationToken = default)
        {
            await containerClient.DeleteBlobIfExistsAsync(
                blobName,
                DeleteSnapshotsOption.IncludeSnapshots,
                cancellationToken: cancellationToken);
        }
    }
}
