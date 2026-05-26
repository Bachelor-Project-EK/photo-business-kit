using Umbraco.Extension.Dtos;
using Umbraco.Extension.Validators;
using FluentValidation.TestHelper;

namespace Umbraco.Extension.Test.Validators
{
    [TestFixture]
    public class BookingDtoValidatorTests
    {
        private BookingDtoValidator _validator;
        private BookingDto _bookingDto;

        [SetUp]
        public void Setup()
        {
            _validator = new BookingDtoValidator();
            _bookingDto = new BookingDto
            {
                StartDate = DateTime.Now, // Set to a future date
                EndDate = DateTime.Now.AddDays(-1) // Set to a past date
            };
        }

        [Test]
        public async Task Should_Have_Error_When_BookingDate_Is_In_The_Past()
        {
            var result = await _validator.TestValidateAsync(_bookingDto);

            result.ShouldHaveValidationErrors();
        }

        [TearDown]
        public void TearDown()
        {
            _validator = null;
        }
    }
}
