using FluentValidation;
using Umbraco.Extension.Dtos;

namespace Umbraco.Extension.Validators;

public class PaymentDtoValidator : AbstractValidator<PaymentDto>
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