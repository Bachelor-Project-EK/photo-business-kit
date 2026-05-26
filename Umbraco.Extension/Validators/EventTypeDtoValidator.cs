using FluentValidation;
using Umbraco.Extension.Dtos;

namespace Umbraco.Extension.Validators;

public class EventTypeDtoValidator : AbstractValidator<EventTypeDto>
{
    public EventTypeDtoValidator()
    {
        RuleFor(x => x.EventTypeName)
            .NotEmpty()
            .Length(1, 100);
    }
}