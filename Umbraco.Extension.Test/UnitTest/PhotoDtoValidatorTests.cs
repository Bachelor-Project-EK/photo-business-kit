using Microsoft.AspNetCore.Http;
using Umbraco.Extension.Dtos;
using Umbraco.Extension.Models;
using Umbraco.Extension.Validators;

namespace Umbraco.Extension.Test.UnitTest
{
    public class PhotoDtoValidatorTests
    {
        private const long OneMb = 1024 * 1024;
        private PhotoDtoValidator _validator = null!;

        [SetUp]
        public void Setup()
        {
            _validator = new PhotoDtoValidator();
        }
        
        [Test]
        public void Validate_FileNameLongerThanCharacters256_ShouldFail()
        {
            // The maximum file name length (including extension) is 255 characters. 
            // This test uses 252 'a' characters + 4 characters for ".jpg" = 256 total, which exceeds the limit.
            var longFileName = new string('a', 252) + ".jpg";
            var dto = new PhotoDto
            {
                AlbumId = Guid.NewGuid(),
                Files =
                [
                    CreateJpegFile(longFileName, 20 * OneMb),
                ]
            };

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void Validate_MultipleValidJpegFiles_ShouldPass()
        {
            var dto = new PhotoDto
            {
                AlbumId = Guid.NewGuid(),
                Files =
                [
                    CreateJpegFile("wedding-001.jpg", 20 * OneMb),
                    CreateJpegFile("wedding-002.jpeg", 35 * OneMb),
                    CreateJpegFile("wedding-003.JPG", 40 * OneMb)
                ]
            };

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.True);
        }

        [TestCase("customer-photo.jpg")]
        [TestCase("customer-photo.jpeg")]
        [TestCase("customer-photo.JPG")]
        [TestCase("customer-photo.JPEG")]
        public void Validate_ValidJpegExtension_ShouldPass(string fileName)
        {
            var dto = new PhotoDto
            {
                AlbumId = Guid.NewGuid(),
                Files =
                [
                    CreateJpegFile(fileName, 10 * OneMb)
                ]
            };

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Validate_OneFileExceeds50Mb_ShouldFail()
        {
            var dto = new PhotoDto
            {
                AlbumId = Guid.NewGuid(),
                Files =
                [
                    CreateJpegFile("large-photo.jpg", 51 * OneMb)
                ]
            };

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.False);
            Assert.That(
                result.Errors.Any(error =>
                    error.ErrorMessage.Contains("50 MB")),
                Is.True);
        }

        [Test]
        public void Validate_TotalUploadExceeds250Mb_ShouldFail()
        {
            var dto = new PhotoDto
            {
                AlbumId = Guid.NewGuid(),
                Files =
                [
                    CreateJpegFile("photo-001.jpg", 45 * OneMb),
                    CreateJpegFile("photo-002.jpg", 45 * OneMb),
                    CreateJpegFile("photo-003.jpg", 45 * OneMb),
                    CreateJpegFile("photo-004.jpg", 45 * OneMb),
                    CreateJpegFile("photo-005.jpg", 45 * OneMb),
                    CreateJpegFile("photo-006.jpg", 45 * OneMb)
                ]
            };

            // 6 × 45 MB = 270 MB total.
            // Each individual photo is valid, but total upload is invalid.

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.False);
            Assert.That(
                result.Errors.Any(error =>
                    error.ErrorMessage.Contains("250 MB")),
                Is.True);
        }
        [Test]
        public void Validate_MoreThan10Files_ShouldFail()
        {
            var dto = new PhotoDto
            {
                AlbumId = Guid.NewGuid(),
                Files = Enumerable
                    .Range(1, 11)
                    .Select(index => CreateJpegFile($"photo-{index}.jpg", 1 * OneMb))
                    .ToList()
            };

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.False);
            Assert.That(
                result.Errors.Any(error =>
                    error.ErrorMessage.Contains("maximum 10")),
                Is.True);
        }

        [Test]
        public void Validate_InvalidFileExtension_ShouldFail()
        {
            var dto = new PhotoDto
            {
                AlbumId = Guid.NewGuid(),
                Files =
                [
                    CreateJpegFile("photo.png", 10 * OneMb)
                ]
            };

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void Validate_InvalidJpegSignature_ShouldFail()
        {
            var dto = new PhotoDto
            {
                AlbumId = Guid.NewGuid(),
                Files =
                [
                    CreateInvalidFile("fake-photo.jpg", 10 * OneMb)
                ]
            };

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.False);
        }


        [Test]
        [TestCaseSource(nameof(EmptyFilesTestData))]
        public void Validate_Empty_Or_Null_FilesList_ShouldFail(List<IFormFile>? obj)
        {
            var dto = new PhotoDto
            {
                AlbumId = Guid.NewGuid(),
                Files = obj
            };

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.False);
        }
        private static IEnumerable<List<IFormFile>?> EmptyFilesTestData()
        {
            yield return null;
            yield return [];
        }

        [Test]
        public void Validate_EmptyAlbumId_ShouldFail()
        {
            var dto = new PhotoDto
            {
                AlbumId = Guid.Empty,
                Files =
                [
                    CreateJpegFile("photo.jpg", 10 * OneMb)
                ]
            };

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void CreatePhotos_FromValidPhotoDto_ShouldMapCorrectDatabaseValues()
        {
            // Arrange
            var photoId = Guid.NewGuid();
            var albumId = Guid.NewGuid();

            var dto = new PhotoDto
            {
                AlbumId = albumId,
                Files = [CreateJpegFile("customer-photo.jpg", 10 * OneMb)]
            };

            // Act
            var photo = new Photos
            {
                Id = photoId,
                AlbumId = dto.AlbumId,
                OriginalFileName = Path.GetFileName(dto.Files[0].FileName),
                BlobPath = $"albums/{dto.AlbumId}/{photoId:N}.jpg",
                ContentType = dto.Files[0].ContentType,
                FileSizeInBytes = dto.Files[0].Length,
                CreatedOn = DateTimeOffset.UtcNow,
                UpdatedOn = DateTimeOffset.UtcNow
            };

            // Assert
            Assert.That(photo.Id, Is.EqualTo(photoId));
            Assert.That(photo.AlbumId, Is.EqualTo(albumId));
            Assert.That(photo.OriginalFileName, Is.EqualTo("customer-photo.jpg"));
            Assert.That(photo.BlobPath, Is.EqualTo($"albums/{albumId}/{photoId:N}.jpg"));
            Assert.That(photo.ContentType, Is.EqualTo("image/jpeg"));
            Assert.That(photo.FileSizeInBytes, Is.EqualTo(dto.Files[0].Length));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("application/pdf")]
        public void Validate_ContentType_With_Application_pdf_ShouldFail(string? contentType)
        {
            var dto = new PhotoDto
            {
                AlbumId = Guid.NewGuid(),
                Files =
                [
                    CreateJpegFile("photo.png", 10 * OneMb, contentType)
                ]
            };

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.False);
        }


        private static IFormFile CreateJpegFile(string fileName, long fileSize, string contentType = "image/jpeg")
        {
            // Valid JPEG signature
            byte[] jpegContent =
            [
                0xFF, 0xD8, 0xFF, 0xE0
            ];
            var stream = new MemoryStream(jpegContent);

            return new FormFile(
                baseStream: stream,
                baseStreamOffset: 0,
                length: fileSize,
                name: "Files",
                fileName: fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }

        private static IFormFile CreateInvalidFile(string fileName, long fileSize)
        {
            byte[] invalidContent =
            [
                0x00, 0x01, 0x02, 0x03
            ];

            var stream = new MemoryStream(invalidContent);

            return new FormFile(
                baseStream: stream,
                baseStreamOffset: 0,
                length: fileSize,
                name: "Files",
                fileName: fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }

    }
}
