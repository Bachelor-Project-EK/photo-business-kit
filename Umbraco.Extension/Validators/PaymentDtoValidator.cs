using FluentValidation;
using Umbraco.Extension.Dtos.Commands;

namespace Umbraco.Extension.Validators;

public class PaymentDtoValidator : AbstractValidator<PaymentCommandDto>
{
    public PaymentDtoValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();

        RuleFor(x => x.Paid)
            .NotNull();

        RuleFor(x => x.PaymentStatus)
            .IsInEnum();

        RuleFor(x => x.TotalPrice)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(99999)
            .PrecisionScale(8, 2, false);
    }
}