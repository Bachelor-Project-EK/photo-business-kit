using Azure.Storage.Blobs;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extension.Dtos;
using Umbraco.Extension.Dtos.Commands;
using Umbraco.Extension.Repositories;
using Umbraco.Extension.Services;
using Umbraco.Extension.Validators;

namespace Umbraco.Extension.Composers;

public class CmsComposer : IComposer
{
    //Composer
    public void Compose(IUmbracoBuilder builder)
    {
        // Register repositories
        builder.Services.AddScoped<BookingRepository>();

        // Register services
        builder.Services.AddScoped<BookingService>();
        builder.Services.AddScoped<EventTypeService>();
        builder.Services.AddScoped<PaymentService>();
        builder.Services.AddScoped<PhotoPackageService>();

        // Register validators
        builder.Services.AddScoped<IValidator<BookingDto>, BookingDtoValidator>();
        builder.Services.AddScoped<IValidator<EventTypeCommandDto>, EventTypeDtoValidator>();
        builder.Services.AddScoped<IValidator<PaymentCommandDto>, PaymentDtoValidator>();
        builder.Services.AddScoped<IValidator<PhotoPackageCommandDto>, PhotoPackageDtoValidator>();

        builder.Services.AddScoped<IPhotoService, PhotoService>();
        //builder.Services.AddScoped<IPhotoStorageService, AzureBlobPhotoStorageService>();
        builder.Services.AddScoped<IValidator<PhotoDto>, PhotoDtoValidator>();

        builder.Services.AddSingleton(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var logger = sp.GetRequiredService<ILoggerFactory>()
                .CreateLogger("BlobStorageConfig");

            var connectionString = configuration.GetConnectionString("umbracoBlobStorage");

            logger.LogInformation(
                "umbracoBlobStorage connection string exists: {Exists}, length: {Length}",
                !string.IsNullOrWhiteSpace(connectionString),
                connectionString?.Length ?? 0
            );

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                logger.LogError("umbracoBlobStorage connection string is missing or empty.");
                throw new InvalidOperationException("umbracoBlobStorage connection string is missing.");
            }

            return new BlobContainerClient(connectionString, "umbraco-media");
        });

        builder.Services.AddScoped<IPhotoStorageService, AzureBlobPhotoStorageService>();

        builder.AddAzureBlobMediaFileSystem();
        builder.AddAzureBlobImageSharpCache();
    }
}
