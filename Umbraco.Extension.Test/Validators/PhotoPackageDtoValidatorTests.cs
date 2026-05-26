using FluentValidation.TestHelper;
using Umbraco.Extension.Dtos;
using Umbraco.Extension.Validators;

namespace Umbraco.Extension.Test.Validators;

public class PhotoPackageDtoValidatorTests
{
    private PhotoPackageDtoValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new PhotoPackageDtoValidator();
    }

    [Test]
    public void PhotoPackageDto_should_not_have_validation_errors()
    {
        // Arrange
        var dto = new PhotoPackageDto
        {
            EventTypeId = Guid.NewGuid(),
            PhotoPackageName = "PackageName",
            PhotoCount = 1,
            PhotoPrice = 1,
            HourlyPrice = 1,
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
