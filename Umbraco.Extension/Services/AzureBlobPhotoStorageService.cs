using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Umbraco.Extension.Services.Interface;

namespace Umbraco.Extension.Services
{
    public class AzureBlobPhotoStorageService(BlobContainerClient containerClient) : IAzureBlobPhotoStorageService
    {
        public async Task<Stream> GetAsync(
            string blobPath,
            CancellationToken cancellationToken = default)
        {
            var blobClient = containerClient.GetBlobClient(blobPath);

            var response = await blobClient.DownloadStreamingAsync(
                cancellationToken: cancellationToken);

            return response.Value.Content;
        }

        public async Task<string> UploadAsync(Guid albumId, Guid photoId, IFormFile file, CancellationToken cancellationToken = default)
        {
            await containerClient.CreateIfNotExistsAsync(
                cancellationToken: cancellationToken);

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
