using FluentValidation;
using Umbraco.Extension.Dtos.Commands;

namespace Umbraco.Extension.Validators;

public class EventTypeDtoValidator : AbstractValidator<EventTypeCommandDto>
{
    public EventTypeDtoValidator()
    {
        RuleFor(x => x.EventTypeName)
            .NotEmpty()
            .Length(1, 100);
    }
}