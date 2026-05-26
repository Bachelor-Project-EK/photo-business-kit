using Umbraco.Extension.Dtos.Queries;
using Umbraco.Extension.Dtos.Commands;
using Umbraco.Extension.Models;
using Umbraco.Cms.Infrastructure.Persistence;

namespace Umbraco.Extension.Services;

public class EventTypeService
{
    private readonly IUmbracoDatabaseFactory _databaseFactory;

    public EventTypeService(IUmbracoDatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }

    public async Task<EventTypeCommandDto> CreateAsync(EventTypeCommandDto dto)
    {
        var eventType = new EventTypes()
        {
            Id = Guid.NewGuid(),
            Name = dto.EventTypeName
        };

        using var database = _databaseFactory.CreateDatabase();

        await database.InsertAsync(eventType);

        return new EventTypeCommandDto()
        {
            EventTypeName = eventType.Name
        };
    }

    public IEnumerable<EventTypeQueryDto> GetAll()
    {
        using var database = _databaseFactory.CreateDatabase();

        return database.Fetch<EventTypes>()
            .Select(eventType => new EventTypeQueryDto
            {
                Id = eventType.Id,
                EventTypeName = eventType.Name
            })
            .ToList();
    }
}
