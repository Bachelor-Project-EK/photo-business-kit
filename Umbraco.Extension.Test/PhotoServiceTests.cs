using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Testcontainers.Azurite;
using Umbraco.Extension.Services;

namespace Umbraco.Extension.Test;

public class PhotoServiceTests
{

    private BlobContainerClient _containerClient = null!;
    private AzureBlobPhotoStorageService _service = null!;

    private AzuriteContainer _azurite;

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {

        _azurite = new AzuriteBuilder()
            .WithImage("mcr.microsoft.com/azure-storage/azurite:latest")
            .WithName("azurite-testcontainer")
            .Build();

        await _azurite.StartAsync();
    }

    [OneTimeTearDown]
    public async Task GlobalTeardown()
    {
        await _azurite.DisposeAsync();
    }

    [SetUp]
    public async Task SetUp()
    {

        _containerClient = new BlobContainerClient(
            _azurite.GetConnectionString(),
            _azurite.Name.Substring(1, _azurite.Name.Length - 1),
            options: new BlobClientOptions(Azure.Storage.Blobs.BlobClientOptions.ServiceVersion.V2025_11_05)
            );



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
            stream,
            0,
            stream.Length,
            "files",
            fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };
    }
}