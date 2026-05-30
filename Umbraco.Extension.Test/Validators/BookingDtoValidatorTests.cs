using Umbraco.Extension.Dtos;
using Umbraco.Extension.Validators;
using FluentValidation.TestHelper;

namespace Umbraco.Extension.Test.Validators
{
    [TestFixture]
    public class BookingDtoValidatorTests
    {
        private BookingDtoValidator _validator;
        private static BookingDto _bookingDto;

        [SetUp]
        public void Setup()
        {
            _validator = new BookingDtoValidator();
            _bookingDto = new BookingDto
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                PhotoPackageId = Guid.NewGuid()
            };
        }

        private static IEnumerable<TestCaseData> InvalidBookingDates()
        {
            yield return new TestCaseData(DateTime.Now, DateTime.Now.AddMinutes(-1)).SetName("BookingDate_Is_In_The_Past_Minutes");
            yield return new TestCaseData(DateTime.Now, DateTime.Now.AddHours(-1)).SetName("BookingDate_Is_In_The_Past_Hours");
            yield return new TestCaseData(DateTime.Now, DateTime.Now.AddDays(-1)).SetName("BookingDate_Is_In_The_Past_Days");
            yield return new TestCaseData(DateTime.Now, DateTime.Now.AddMonths(-1)).SetName("BookingDate_Is_In_The_Past_Months");
            yield return new TestCaseData(DateTime.Now, DateTime.Now.AddYears(-1)).SetName("BookingDate_Is_In_The_Past_Years");
        }

        [TestCaseSource(nameof(InvalidBookingDates))]
        public void EndDate_Should_Have_Validation_Errors(DateTime startDate, DateTime endDate)
        {
            _bookingDto.StartDate = startDate;
            _bookingDto.EndDate = endDate;
            var result = _validator.TestValidate(_bookingDto);
            result.ShouldHaveValidationErrorFor(x => x.EndDate);
        }

        private static IEnumerable<TestCaseData> ValidBookingDates()
        {
            yield return new TestCaseData(DateTime.Now, DateTime.Now).SetName("BookingDate_Is_Now");
            yield return new TestCaseData(DateTime.Now, DateTime.Now.AddMinutes(1)).SetName("BookingDate_Is_In_The_Future_Minutes");
            yield return new TestCaseData(DateTime.Now, DateTime.Now.AddHours(1)).SetName("BookingDate_Is_In_The_Future_Hours");
            yield return new TestCaseData(DateTime.Now, DateTime.Now.AddDays(1)).SetName("BookingDate_Is_In_The_Future_Days");
            yield return new TestCaseData(DateTime.Now, DateTime.Now.AddMonths(1)).SetName("BookingDate_Is_In_The_Future_Months");
            yield return new TestCaseData(DateTime.Now, DateTime.Now.AddYears(1)).SetName("BookingDate_Is_In_The_Future_Years");
        }

        [TestCaseSource(nameof(ValidBookingDates))]
        public void EndDate_Should_Not_Have_Validation_Errors(DateTime startDate, DateTime endDate)
        {
            _bookingDto.StartDate = startDate;
            _bookingDto.EndDate = endDate;
            var result = _validator.TestValidate(_bookingDto);
            result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        }

        [TestCase("Wedding 2026")]
        [TestCase("Anna-Marie")]
        [TestCase("Family_Photos")]
        [TestCase("John & Sara")]
        [TestCase("Album (Final)")]
        [TestCase("Børnefødselsdag")]
        public void Comment_Should_Not_Have_Validation_Errors(string comment)
        {
            _bookingDto.Comment = comment;

            var result = _validator.TestValidate(_bookingDto);

            result.ShouldNotHaveValidationErrorFor(x => x.Comment);
        }

        [TestCase("Wedding!")]
        [TestCase("Album@2026")]
        [TestCase("Photo#1")]
        [TestCase("Name/Folder")]
        [TestCase("Album:Final")]
        public void Comment_Should_Have_Validation_Errors(string comment)
        {
            _bookingDto.Comment = comment;

            var result = _validator.TestValidate(_bookingDto);

            result.ShouldHaveValidationErrorFor(x => x.Comment);
        }

        [TearDown]
        public void TearDown()
        {
            _validator = null;
        }
    }
}
