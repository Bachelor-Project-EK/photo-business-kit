using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Extension.Dtos;

namespace Umbraco.Extension.Validators
{
    public class BookingDtoValidator : AbstractValidator<BookingDto>
    {
        public BookingDtoValidator()
        {
            RuleFor(x => x.StartDate)
                .NotNull()
                .WithMessage("Start date is required.")
                .LessThan(x => x.EndDate)
                .WithMessage("Start date must be before end date.");
            RuleFor(x => x.EndDate)
                .NotNull()
                .WithMessage("End date is required.")
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be after start date.");
            RuleFor(x => x.Comment)
                .MaximumLength(500)
                .WithMessage("Comment cannot exceed 500 characters.");
        }
    }
}
