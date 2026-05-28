using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extension.Dtos;
using Umbraco.Extension.Dtos.Commands;
using Umbraco.Extension.Services;
using Umbraco.Extension.Validators;

namespace Umbraco.Extension.Composers;

public class CmsComposer : IComposer
{
    //Composer
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<EventTypeService>();
        builder.Services.AddScoped<PaymentService>();
        builder.Services.AddScoped<PhotoPackageService>();
        builder.Services.AddScoped<IValidator, EventTypeDtoValidator>();
        builder.Services.AddScoped<IValidator<PaymentCommandDto>, PaymentDtoValidator>();
        builder.Services.AddScoped<IValidator<PhotoPackageCommandDto>, PhotoPackageDtoValidator>();

        builder.Services.AddScoped<IPhotoService, PhotoService>();
        builder.Services.AddScoped<IPhotoStorageService, AzureBlobPhotoStorageService>();
        builder.Services.AddScoped<IValidator<PhotoDto>, PhotoDtoValidator>();

        builder.AddAzureBlobMediaFileSystem();
        builder.AddAzureBlobImageSharpCache();

    }
}
