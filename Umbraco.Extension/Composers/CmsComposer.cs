using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
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
    }
}
