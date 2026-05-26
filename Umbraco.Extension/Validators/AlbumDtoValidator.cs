using FluentValidation;
using Umbraco.Extension.Dtos;

namespace Umbraco.Extension.Validators;

public class AlbumDtoValidator : AbstractValidator<AlbumDto>
{
    
    public AlbumDtoValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("BookingId is required and cannot be Guid.Empty.");

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Name is required.")
            .Must(name => name == name.Trim())
            .WithMessage("Name cannot start or end with whitespace.")
            .Length(1, 50)
            .WithMessage("Name must be between 1 and 50 characters.")
            .Matches(@"^[\p{L}\p{N} _'&().,\-]+$")
            .WithMessage(
                "Name may only contain letters, numbers, spaces and the characters - _ ' & ( ) . ,");
      
        RuleFor(x => x.CreatedOn)
            .NotEmpty()
            .WithMessage("CreatedOn is required.");


        RuleFor(x => x.UpdatedOn)
            .NotEmpty()
            .WithMessage("UpdatedOn is required.")
            .GreaterThanOrEqualTo(x => x.CreatedOn)
            .WithMessage("UpdatedOn must be the same as or later than CreatedOn.");
    }
}