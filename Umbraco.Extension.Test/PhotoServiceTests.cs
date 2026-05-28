using Azure.Storage.Blobs;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extension.Dtos;
using Umbraco.Extension.Services;
using Umbraco.Extension.Validators;

namespace Umbraco.Extension.Test
{
    public class PhotoServiceTests
    {
        private const string ConnectionString = "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;AccountName=devstoreaccount1;BlobEndpoint=http://127.0.0.1:27000/devstoreaccount1;";
        private const string ContainerName = "photos-test";

        private BlobContainerClient _containerClient = null!;
        private AzureBlobPhotoStorageService _service = null!;

        [SetUp]
        public async Task SetUp()
        {
            _containerClient = new BlobContainerClient(
                ConnectionString,
                ContainerName);

            await _containerClient.DeleteIfExistsAsync();
            await _containerClient.CreateIfNotExistsAsync();

            _service = new AzureBlobPhotoStorageService(_containerClient);

        }

        [Test]
        public async Task UploadAsync_ValidJpeg_ShouldUploadBlobAndReturnBlobPath()
        {
            // Arrange
            var albumId = Guid.NewGuid();
            var photoId = Guid.NewGuid();

            var file = CreateJpegFile("wedding-001.jpg");

            // Act
            var blobPath = await _service.UploadAsync(
                albumId,
                photoId,
                file);

            // Assert
            var blobClient = _containerClient.GetBlobClient(blobPath);

            var exists = await blobClient.ExistsAsync();

            Assert.Multiple(() =>
            {
                Assert.That(blobPath, Is.EqualTo(
                    $"albums/{albumId:N}/{photoId:N}.jpg"));

                Assert.That(exists.Value, Is.True);
            });
        }

        [Test]
        public async Task DeleteIfExistsAsync_ExistingBlob_ShouldDeleteBlob()
        {
            // Arrange
            var albumId = Guid.NewGuid();
            var photoId = Guid.NewGuid();

            var file = CreateJpegFile("wedding-001.jpg");

            var blobPath = await _service.UploadAsync(
                albumId,
                photoId,
                file);

            var blobClient = _containerClient.GetBlobClient(blobPath);

            Assert.That((await blobClient.ExistsAsync()).Value, Is.True);

            // Act
            await _service.DeleteIfExistsAsync(blobPath);

            // Assert
            Assert.That((await blobClient.ExistsAsync()).Value, Is.False);
        }

        private static IFormFile CreateJpegFile(string fileName)
        {
            byte[] jpegBytes =
            [
                0xFF, 0xD8, 0xFF, 0xE0

            ];

            var stream = new MemoryStream(jpegBytes);

            return new FormFile(
                baseStream: stream,
                baseStreamOffset: 0,
                length: stream.Length,
                name: "files",
                fileName: fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }
    }
}
