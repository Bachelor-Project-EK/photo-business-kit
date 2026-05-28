using FluentValidation.TestHelper;
using Umbraco.Extension.Dtos.Commands;
using Umbraco.Extension.Enum;
using Umbraco.Extension.Validators;

namespace Umbraco.Extension.Test.Validators;

public class PaymentDtoValidatorTests
{
    private PaymentDtoValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new PaymentDtoValidator();
    }

    [Test]
    public void PaymentDto_should_not_have_validation_errors()
    {
        // Arrange
        var dto = new PaymentCommandDto()
        {
            OrderId = Guid.NewGuid(),
            Paid = true,
            PaymentStatus = PaymentStatus.Confirmed,
            TotalPrice = 100

        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
