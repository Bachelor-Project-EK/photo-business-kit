using Umbraco.Extension.Dtos;
using Umbraco.Extension.Validators;

namespace Umbraco.Extension.Test
{
    internal class AlbumDtoValidatorTests
    {
        private AlbumDtoValidator _validator = null!;

        [SetUp]
        public void Setup()
        {
            _validator = new AlbumDtoValidator();
        }

        [Test]
        public void Validate_ValidAlbumDto_ShouldPass()
        {
            var dto = CreateValidAlbumDto();

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.True);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Validate_EmptyName_ShouldFail(string? name)
        {
            var dto = CreateValidAlbumDto();
            dto.Name = name!;

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.False);
        }
        [TestCase(" Wedding")]
        [TestCase("Wedding ")]
        [TestCase(" Wedding ")]
        public void Validate_NameWithStartOrEndWhitespace_ShouldFail(string name)
        {
            var dto = CreateValidAlbumDto();
            dto.Name = name;

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void Validate_NameLongerThan50Characters_ShouldFail()
        {
            var dto = CreateValidAlbumDto();
            dto.Name = new string('A', 51);

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.False);
        }

        [TestCase("Wedding 2026")]
        [TestCase("Anna-Marie")]
        [TestCase("Family_Photos")]
        [TestCase("John & Sara")]
        [TestCase("Album (Final)")]
        [TestCase("Børnefødselsdag")]
        public void Validate_AllowedNameCharacters_ShouldPass(string name)
        {
            var dto = CreateValidAlbumDto();
            dto.Name = name;

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.True);
        }

        [TestCase("Wedding!")]
        [TestCase("Album@2026")]
        [TestCase("Photo#1")]
        [TestCase("Name/Folder")]
        [TestCase("Album:Final")]
        public void Validate_InvalidNameCharacters_ShouldFail(string name)
        {
            var dto = CreateValidAlbumDto();
            dto.Name = name;

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void Validate_EmptyCreatedOn_ShouldFail()
        {
            var dto = CreateValidAlbumDto();
            dto.CreatedOn = default;

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void Validate_EmptyUpdatedOn_ShouldFail()
        {
            var dto = CreateValidAlbumDto();
            dto.UpdatedOn = default;

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void Validate_UpdatedOnBeforeCreatedOn_ShouldFail()
        {
            var dto = CreateValidAlbumDto();
            dto.CreatedOn = DateTimeOffset.Now;
            dto.UpdatedOn = dto.CreatedOn.AddMinutes(-1);

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void Validate_UpdatedOnSameAsCreatedOn_ShouldPass()
        {
            var now = DateTimeOffset.Now;

            var dto = CreateValidAlbumDto();
            dto.CreatedOn = now;
            dto.UpdatedOn = now;

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Validate_UpdatedOnAfterCreatedOn_ShouldPass()
        {
            var dto = CreateValidAlbumDto();
            dto.CreatedOn = DateTimeOffset.Now;
            dto.UpdatedOn = dto.CreatedOn.AddMinutes(5);

            var result = _validator.Validate(dto);

            Assert.That(result.IsValid, Is.True);
        }


        private static AlbumDto CreateValidAlbumDto()
        {
            var now = DateTimeOffset.Now;
            return new AlbumDto
            {
                BookingId = Guid.NewGuid(),
                Name = "Wedding Album",
                CreatedOn = now,
                UpdatedOn = now
            };
        }
    }

}
