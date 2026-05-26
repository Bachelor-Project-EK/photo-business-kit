using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extension.Dtos;
using Umbraco.Extension.Services;
using Umbraco.Extension.Validators;

namespace Umbraco.Extension.Composers;

public class EventTypeComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<EventTypeService>();
        builder.Services.AddScoped<IValidator<EventTypeDto>, EventTypeDtoValidator>();
    }
}