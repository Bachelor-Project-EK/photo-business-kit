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
using Umbraco.Extension.Services.Interface;
using Umbraco.Extension.Validators;

namespace Umbraco.Extension.Composers;

public class CmsComposer : IComposer
{
    //Composer
    public void Compose(IUmbracoBuilder builder)
    {
        // Register repositories
        builder.Services.AddScoped<BookingRepository>();
        builder.Services.AddScoped<AlbumRepository>();
        builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();


        // Register services
        builder.Services.AddScoped<BookingService>();
        builder.Services.AddScoped<EventTypeService>();
        builder.Services.AddScoped<PaymentService>();
        builder.Services.AddScoped<PhotoPackageService>();
        builder.Services.AddScoped<AlbumService>();


        // Register validators
        builder.Services.AddScoped<IValidator<BookingDto>, BookingDtoValidator>();
        builder.Services.AddScoped<IValidator<EventTypeCommandDto>, EventTypeDtoValidator>();
        builder.Services.AddScoped<IValidator<PaymentCommandDto>, PaymentDtoValidator>();
        builder.Services.AddScoped<IValidator<PhotoPackageCommandDto>, PhotoPackageDtoValidator>();

        builder.Services.AddScoped<IPhotoService, PhotoService>();
        builder.Services.AddScoped<IValidator<PhotoDto>, PhotoDtoValidator>();

        builder.Services.AddSingleton(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var logger = sp.GetRequiredService<ILoggerFactory>()
                .CreateLogger("BlobStorageConfig");

            var connectionString = configuration.GetConnectionString("umbracoBlobStorage") ?? configuration.GetSection(
                "Umbraco:Storage:AzureBlob:Media:ConnectionString").Value;

            logger.LogInformation(
                "umbracoBlobStorage connection string exists: {Exists}, length: {Length}",
                !string.IsNullOrWhiteSpace(connectionString),
                connectionString?.Length ?? 0
            );

            if (!string.IsNullOrWhiteSpace(connectionString))
                return new BlobContainerClient(connectionString, "umbraco-media");

            logger.LogError("umbracoBlobStorage connection string is missing or empty.");
            throw new InvalidOperationException("umbracoBlobStorage connection string is missing.");

        });

        builder.Services.AddScoped<IAzureBlobPhotoStorageService, AzureBlobPhotoStorageService>();

        builder.AddAzureBlobMediaFileSystem();
        builder.AddAzureBlobImageSharpCache();
    }
}
