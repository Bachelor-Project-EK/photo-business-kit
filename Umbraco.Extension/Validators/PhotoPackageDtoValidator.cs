using FluentValidation;
using Umbraco.Extension.Dtos;

namespace Umbraco.Extension.Validators;

public class PhotoPackageDtoValidator : AbstractValidator<PhotoPackageDto>
{
    public PhotoPackageDtoValidator()
    {
        RuleFor(x => x.EventTypeId)
            .NotEmpty();
        
        RuleFor(x => x.PhotoPackageName)
            .Length(1, 255);
        
        RuleFor(x => x.PhotoCount)
            .NotNull()
            .GreaterThanOrEqualTo(0);
        
        RuleFor(x => x.PhotoPrice)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(999999)
            .PrecisionScale(8, 2, false);


        RuleFor(x => x.HourlyPrice)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(999999)
            .PrecisionScale(8, 2, false);
    }
}