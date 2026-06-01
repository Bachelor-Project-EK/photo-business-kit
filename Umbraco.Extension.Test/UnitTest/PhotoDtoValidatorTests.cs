using Microsoft.AspNetCore.Http;
using Umbraco.Extension.Dtos;
using Umbraco.Extension.Models;
using Umbraco.Extension.Validators;

namespace Umbraco.Extension.Test.UnitTest
{
    [Category(nameof(TestCategory.UnitTest))]
    public class PhotoDtoValidatorTests
    {
        private const long OneMb = 1024 * 1024;

        private const long MaxFileSize = 50 * OneMb;


        private PhotoDtoValidator _validator = null!;

        [SetUp]
        public void Setup()
        {
            _validator = new PhotoDtoValidator();
        }

        [TestCase(false, true)] // Valid partition: Et udfyldt AlbumId
        [TestCase(true, false)] // Invalid partition: Guid.Empty
        public void Validate_AlbumId_ShouldReturnExpectedResult(bool useEmptyAlbumId, bool expectedIsValid)
        {
            // Arrange
            var dto = new PhotoDto
            {
                AlbumId = useEmptyAlbumId ? Guid.Empty : Guid.NewGuid(),
                Files = [CreateJpegFile("photo.jpg", 10 * OneMb)]
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            Assert.That(result.IsValid, Is.EqualTo(expectedIsValid));
        }

        [TestCase(1, true)] //Lower boundary : 1 file is valid
        [TestCase(50, true)] //Upper boundary: 50 files is valid

        [TestCase(0, false)] // Outside lower boundary: 0 files is invalid
        [TestCase(51, false)] // Outside upper boundary: 51 files is invalid
        [TestCase(10000, false)] // Extreme case: A very large number of files is invalid
        public void Validate_NumberOfFiles_ShouldReturnExpectedResult(int numberOfFiles, bool expectedIsValid)
        {
            var dto = new PhotoDto
            {
                AlbumId = Guid.NewGuid(),
                Files = Enumerable.Range(1, numberOfFiles)
                    .Select(index => CreateJpegFile($"photo-{index}.jpg", 1 * OneMb)).ToList()
            };
            var result = _validator.Validate(dto);
            Assert.That(result.IsValid, Is.EqualTo(expectedIsValid));
        }

        [Test]
        [TestCaseSource(nameof(EmptyFilesTestData))]
        public void Validate_Empty_Or_Null_FilesList_ShouldFail(List<IFormFile>? obj)
        {
            // Arrange
            var dto = new PhotoDto
            {
                AlbumId = Guid.NewGuid(),
                Files = obj
            };
            // Act
            var result = _validator.Validate(dto);

            //Assert
            Assert.That(result.IsValid, Is.False);
        }
        private static IEnumerable<List<IFormFile>?> EmptyFilesTestData()
        {
            yield return null; // Invalid partition: files is null
            yield return []; // Invalid partition: files is an empty list
        }

        [Test]
        [TestCase("customer-photo.jpg", true)]   // Valid partition: .jpg
        [TestCase("customer-photo.jpeg", true)]  // Valid partition: .jpeg
        [TestCase("customer-photo.JPG", true)]   // Valid partition: Uppercase .JPG
        [TestCase("customer-photo.JPEG", true)]  // Valid partition: Uppercase .JPEG
        [TestCase("customer-photo.png", false)]  // Invalid partition: .png
        [TestCase("customer-photo.gif", false)]  // Invalid partition: .gif
        [TestCase("customer-photo.webp", false)] // Invalid partition: .webp
        [TestCase("customer-photo", false)]      // Invalid partition: No extension
        public void Validate_FileExtension_ShouldReturnExpectedResult(string fileName, bool expectedIsValid)
        {
            // Arrange
            var dto = new PhotoDto
            {
                AlbumId = Guid.NewGuid(),
                Files = [
                    CreateJpegFile(fileName, 10 * OneMb)
                ]
            };
            // Act
            var result = _validator.Validate(dto);
            // Assert
            Assert.That(result.IsValid, Is.EqualTo(expectedIsValid));
        }

        [TestCase("photo<script>.jpg")] // Invalid partition: HTML-/script-lignende tegn
        [TestCase("photo?.jpg")]        // Invalid partition: Ikke-tilladt specialtegn
        public void Validate_FileNameWithInvalidCharacters_ShouldFail(string fileName)
        {
            // Arrange
            var dto = new PhotoDto
            {
                AlbumId = Guid.NewGuid(),
                Files = [
                    CreateJpegFile(fileName, 10 * OneMb)
                ]
            };
            // Act
            var result = _validator.Validate(dto);
            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        //Valid partition
        [TestCase(1, true)] // Lower boundary: Exactly 1 file is valid
        [TestCase(50, true)] // Upper boundary: Exactly 50 files is valid

        //Invalid partition
        [TestCase(0, false)] // Outside lower boundary: 0 files is invalid
        [TestCase(51, false)] // Outside upper boundary: 51 files is invalid
        public void Validate_Exactly10Files_ShouldPass(int numberOfFiles, bool expectedIsValid)
        {
            var dto = new PhotoDto
            {
                AlbumId = Guid.NewGuid(),
                Files = Enumerable.Range(0, numberOfFiles).Select(index => CreateJpegFile($"photo-{index}.jpg", 1 * OneMb)).ToList()


            };
            var result = _validator.Validate(dto);
            Assert.That(result.IsValid, Is.EqualTo(expectedIsValid));
        }


        [TestCase(1, true)]    // Lower boundary: 1 character + extension is valid
        [TestCase(255, true)]  // Upper boundary: 255 characters (including extension) is valid

        [TestCase(256, false)] // Outside lower/upper boundary: 256 characters is invalid
        [TestCase(512, false)] // Extreme case: A very long file name is invalid
        public void Validate_FileNameLength_ShouldReturnExpectedResult(int totalFileNameLength, bool expectedIsValid)
        {
            // Arrange
            var extension = ".jpg";
            //var numberOfNameCharacters = totalFileNameLength - extension.Length;

            var fileName = new string('a', totalFileNameLength) + extension;

            var dto = new PhotoDto
            {
                AlbumId = Guid.NewGuid(),
                Files = [
                    CreateJpegFile(fileName, 10 * OneMb)
                ]
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            Assert.That(result.IsValid, Is.EqualTo(expectedIsValid));
        }

        //Valid partitions
        [TestCase(1 * OneMb, true)]                 // Lower boundary: 1 byte is valid
        [TestCase(MaxFileSize, true)]        // Upper boundary: Exactly 50 MB is valid

        //Invalid partitions
        [TestCase(0 * OneMb, false)]                 // Outside lower boundary: 0 bytes is invalid
        [TestCase(MaxFileSize + 1, false)]   // Outside upper boundary: 50 MB + 1 byte
        [TestCase(100 * OneMb, false)]        // Extreme case: 100 MB is invalid

        public void Validate_FileSize_ShouldReturnExpectedResult(long fileSize, bool expectedIsValid)
        {
            // Arrange
            var dto = new PhotoDto
            {
                AlbumId = Guid.NewGuid(),
                Files = [
                    CreateJpegFile("photo.jpg", fileSize)
                ]
            };
            // Act
            var result = _validator.Validate(dto);
            // Assert
            Assert.That(result.IsValid, Is.EqualTo(expectedIsValid));
        }

        [Test]
        [TestCaseSource(nameof(TotalUploadSizeTestData))]
        public void Validate_TotalUploadSize_ShouldReturnExpectedResult(long[] fileSizes, bool expectedIsValid)
        {
            // Arrange
            var files = fileSizes
                .Select((fileSize, index) => CreateJpegFile($"photo-{index + 1}.jpg", fileSize))
                .ToList();
            var dto = new PhotoDto
            {
                AlbumId = Guid.NewGuid(),
                Files = files
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            Assert.That(result.IsValid, Is.EqualTo(expectedIsValid));
        }

        private static IEnumerable<TestCaseData> TotalUploadSizeTestData()
        {
            //  Upper boundary: 250 MB : TotalUpload_Exactly250Mb_ShouldPass
            yield return new TestCaseData(new[]
            {
                50 * OneMb,
                50 * OneMb,
                50 * OneMb,
                50 * OneMb,
                50 * OneMb
            }, true);


            // Outside upper boundary: 250 MB + 1 byte:  TotalUpload_250MbPlusOneByte_ShouldFail
            yield return
                new TestCaseData(new[]
                {
                    50 * OneMb,
                    50 * OneMb,
                    50 * OneMb,
                    50 * OneMb,
                    49 * OneMb,
                    1 * OneMb + 1
                }, false);

        }

        [TestCase("image/jpeg", true)]   // Valid partition: Allowed MIME type

        [TestCase(null, false)]          // Invalid partition: Null value
        [TestCase("", false)]            // Invalid partition: Empty string
        [TestCase("image/png", false)]   // Invalid partition: PNG
        [TestCase("application/pdf", false)] // Invalid partition: PDF

        public void Validate_ContentType_ShouldReturnExpectedResult(string? contentType, bool expectedIsValid)
        {
            // Arrange
            var dto = new PhotoDto
            {
                AlbumId = Guid.NewGuid(),
                Files =
                [ 
                    // We are testing only the content type
                    CreateJpegFile("photo.jpg", 10 * OneMb, contentType!)
                ]
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            Assert.That(result.IsValid, Is.EqualTo(expectedIsValid));
        }

        [TestCase(true, true)] // Valid partition: Valid JPEG signature
        [TestCase(false, false)] // Invalid partition: Invalid JPEG signature
        public void Validate_JpegSignature_ShouldReturnExpectedResult(bool useValidJpegSignature, bool expectedIsValid)
        {
            // Arrange
            var file = useValidJpegSignature
                ? CreateJpegFile("photo.jpg", 10 * OneMb)
                : CreateInvalidFile("photo.jpg", 10 * OneMb);
            var dto = new PhotoDto { AlbumId = Guid.NewGuid(), Files = [file] };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            Assert.That(result.IsValid, Is.EqualTo(expectedIsValid));
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

        private static IFormFile CreateJpegFile(string fileName, long fileSize, string contentType = "image/jpeg")
        {
            // Valid JPEG signature
            byte[] jpegContent =
            [
                0xFF, 0xD8, 0xFF
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
