using FluentValidation;
using Microsoft.AspNetCore.Http;
using Umbraco.Extension.Dtos;

namespace Umbraco.Extension.Validators;

public class PhotoDtoValidator : AbstractValidator<PhotoDto>
{
    private const long MaxFileSizeBytes = 50 * 1024 * 1024; // 50 MB
    private const long MaxTotalSizeBytes = 250 * 1024 * 1024; // 250 MB per upload
    private const int MaxNumberOfFiles = 10;


    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg"
        };


    public PhotoDtoValidator()
    {
        RuleFor(photo => photo.AlbumId)
            .NotEmpty()
            .WithMessage("AlbumId is required.");

        RuleFor(dto => dto.Files)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("Files is required.")
            .NotEmpty()
            .WithMessage("At least one photo is required.")
            .Must(files => files!.Count <= MaxNumberOfFiles)
            .WithMessage("You can upload maximum 10 photos at a time.")
            .Must(files => files!.Sum(file => file.Length) <= MaxTotalSizeBytes)
            .WithMessage("The total upload must not exceed 250 MB.");

        RuleForEach(dto => dto.Files)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("Each photo file is required.")
            .NotEmpty()
            .WithMessage("Each photo file is required.")
            //.Must(file => file is not null && file.Length > 0)
            .Must(file => file.FileName.Length is > 0 and <= 255)
            .WithMessage("A photo must not be empty.")
            .Must(file => file.Length <= MaxFileSizeBytes)
            .WithMessage("Each photo must not exceed 50 MB.")
            .Must(HaveAllowedExtension)
            .WithMessage("Only .jpg and .jpeg files are allowed.")
            .Must(HaveAllowedContentType)
            .WithMessage("The file content type must be image/jpeg.")
            .Must(HaveValidJpegSignature)
            .WithMessage("The uploaded file is not a valid JPEG file.");

    }

    private static bool HaveAllowedExtension(IFormFile? file)
    {
        return file is not null && AllowedExtensions.Contains(Path.GetExtension(file.FileName));
    }

    private static bool HaveAllowedContentType(IFormFile? file)
    {
        return file is not null &&
               string.Equals(
                   file.ContentType,
                   "image/jpeg",
                   StringComparison.OrdinalIgnoreCase);
    }

    private static bool HaveValidJpegSignature(IFormFile? file)
    {

        if (file is null || file.Length < 3) return false;

        // link to jpeg signature: https://www.file-recovery.com/jpg-signature-format.htm 
        byte[] expectedHeader = [0xFF, 0xD8, 0xFF];
        var actualHeader = new byte[expectedHeader.Length];


        using var stream = file.OpenReadStream();

        var bytesRead = stream.Read(
            actualHeader,
            0,
            actualHeader.Length);

        return bytesRead == expectedHeader.Length &&
               actualHeader.SequenceEqual(expectedHeader);


    }
}