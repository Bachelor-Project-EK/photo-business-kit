using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Http;
using Testcontainers.Azurite;
using Umbraco.Extension.Dtos;
using Umbraco.Extension.Validators;



namespace Umbraco.Extension.Test
{
    [TestFixture]
    public class PhotoBlobStorageIntegrationTests
    {

        private AzuriteContainer _azurite;

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            _azurite = new AzuriteBuilder()
                .WithImage("mcr.microsoft.com/azure-storage/azurite")
                .WithName("azurite-testcontainer")
                .Build();

            await _azurite.StartAsync();
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            await _azurite.DisposeAsync();

        }

        [TestCase("sample-photo.jpg")]
        [TestCase("virus.jpg")]
        public async Task ValidateAndUploadRealJpeg_ToAzurite_ShouldWork(string fileName)
        {
            // Arrange
            var blobServiceClient = new BlobServiceClient(_azurite.GetConnectionString());

            var containerClient =
                blobServiceClient.GetBlobContainerClient(_azurite.Name.Substring(1, _azurite.Name.Length - 1));

            await containerClient.CreateIfNotExistsAsync();

            var albumId = Guid.NewGuid();
            var photoId = Guid.NewGuid();

            var blobName = $"albums/{albumId:N}/photos/{photoId:N}.jpg";
            var imagePath = Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "TestFiles",
                fileName);

            Assert.That(File.Exists(imagePath), Is.True);

            var imageBytes = await File.ReadAllBytesAsync(imagePath);

            await using var stream = new MemoryStream(imageBytes);

            IFormFile file = new FormFile(
                baseStream: stream,
                baseStreamOffset: 0,
                length: imageBytes.Length,
                name: "Files",
                fileName: fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            var dto = new PhotoDto
            {
                AlbumId = Guid.NewGuid(),
                Files = [file]
            };
            // validate
            var validator = new PhotoDtoValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (fileName == "sample-photo.jpg")
            {
                Assert.That(validationResult.IsValid, Is.True);

                var blobClient = containerClient.GetBlobClient(blobName);

                // Act
                await using var uploadStream = file.OpenReadStream();

                await blobClient.UploadAsync(
                    uploadStream,
                    new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders
                        {
                            ContentType = "image/jpeg"
                        }
                    });

                // Assert 1: Blob exists
                var exists = await blobClient.ExistsAsync();

                Assert.That(exists.Value, Is.True);

                // Assert 2: Blob metadata/content type is correct
                var properties = await blobClient.GetPropertiesAsync();

                Assert.That(properties.Value.ContentType, Is.EqualTo("image/jpeg"));
                Assert.That(properties.Value.ContentLength, Is.EqualTo(imageBytes.Length));

                // Assert 3: Download blob and compare with original image
                var downloaded = await blobClient.DownloadContentAsync();
                var downloadedBytes = downloaded.Value.Content.ToArray();

                Assert.That(downloadedBytes, Is.EqualTo(imageBytes));

                // Cleanup
                await blobClient.DeleteIfExistsAsync();
            }
            else
            {
                Assert.That(validationResult.IsValid, Is.False);
            }

        }
    }

}
