using FluentValidation;
using Microsoft.AspNetCore.Http;
using Moq;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extension.Dtos;
using Umbraco.Extension.Models;
using Umbraco.Extension.Services;

namespace Umbraco.Extension.Test.UnitTest
{
    public class PhotoServiceTest
    {
        private Mock<IUmbracoDatabaseFactory> _databaseFactory = null!;
        private Mock<IUmbracoDatabase> _readDb = null!;
        private Mock<IUmbracoDatabase> _writeDb = null!;
        private Mock<IPhotoStorageService> _photoStorageService = null!;
        private Mock<IValidator<PhotoDto>> _validator = null!;

        private PhotoService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _databaseFactory = new Mock<IUmbracoDatabaseFactory>();

            _readDb = new Mock<IUmbracoDatabase>();
            _writeDb = new Mock<IUmbracoDatabase>();

            _photoStorageService = new Mock<IPhotoStorageService>();
            _validator = new Mock<IValidator<PhotoDto>>();

            _databaseFactory
                .SetupSequence(x => x.CreateDatabase())
                .Returns(_readDb.Object)
                .Returns(_writeDb.Object);

            _service = new PhotoService(
                _databaseFactory.Object,
                _photoStorageService.Object,
                _validator.Object);
        }

        [Test]
        public async Task UploadToAlbumAsync_WhenValidDto_ShouldUploadAndInsertPhotos()
        {
            // Arrange
            var albumId = Guid.NewGuid();

            var dto = new PhotoDto
            {
                AlbumId = albumId,
                Files =
                [
                    CreateJpegFile("photo-1.jpg"),
                    CreateJpegFile("photo-2.jpg")
                ]
            };

            _readDb
                .Setup(x => x.SingleOrDefaultByIdAsync<Albums>(
                    albumId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Albums()
                {
                    Id = albumId,
                    Name = "Test_album"
                });



            _photoStorageService
                .Setup(x => x.UploadAsync(
                    albumId,
                    It.IsAny<Guid>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid _, Guid photoId, IFormFile file, CancellationToken _) =>
                    $"albums/{albumId}/{photoId}-{file.FileName}");

            // Act
            var result = await _service.UploadToAlbumAsync(dto);

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));

            _photoStorageService.Verify(x => x.UploadAsync(
                    albumId,
                    It.IsAny<Guid>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));

            // Verify that the photos was inserted into the database with the correct values
            _writeDb.Verify(x => x.InsertAsync(
                    It.IsAny<Photos>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));

            // Verify that the transaction was completed
            _writeDb.Verify(x => x.CompleteTransactionAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private static IFormFile CreateJpegFile(string fileName)
        {
            var jpegHeader = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };
            var stream = new MemoryStream(jpegHeader);

            return new FormFile(stream, 0, stream.Length, "files", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }
    }
}