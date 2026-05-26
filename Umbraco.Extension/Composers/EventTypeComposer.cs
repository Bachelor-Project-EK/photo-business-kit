using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extension.Dtos.Commands;
using Umbraco.Extension.Services;
using Umbraco.Extension.Validators;

namespace Umbraco.Extension.Composers;

public class EventTypeComposer : IComposer
{
    //Composer
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<EventTypeService>();
        builder.Services.AddScoped<IValidator<EventTypeCommandDto>, EventTypeDtoValidator>();
    }
}