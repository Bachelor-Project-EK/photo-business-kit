using FluentValidation.TestHelper;
using Umbraco.Extension.Dtos.Commands;
using Umbraco.Extension.Validators;

namespace Umbraco.Extension.Test.Validators;

public class EventTypeDtoValidatorTests
{
    private EventTypeDtoValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new EventTypeDtoValidator();
    }

    [Test]
    public void EventTypeDto_should_have_validation_errors()
    {
        // Arrange
        var dto = new EventTypeCommandDto()
        {
            EventTypeName = ""
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EventTypeName);
    }

    [Test]
    public void EventTypeDto_should_not_have_validation_errors()
    {
        // Arrange
        var dto = new EventTypeCommandDto
        {
            EventTypeName = "Wedding"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
