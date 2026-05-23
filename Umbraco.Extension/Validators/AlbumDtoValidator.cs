using FluentValidation;
using Umbraco.Extension.Dtos;

namespace Umbraco.Extension.Validators;

public class AlbumDtoValidator : AbstractValidator<AlbumDto>
{
    public AlbumDtoValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("BookingId is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MinimumLength(2)
            .MaximumLength(50)
            .WithMessage("Name must be between 2 and 50 characters.");

        RuleFor(x => x.CreatedOn)
            .NotEmpty()
            .WithMessage("CreatedOn is required.");

        RuleFor(x => x.UpdatedOn)
            .NotEmpty()
            .WithMessage("UpdatedOn is required.");
    }
}