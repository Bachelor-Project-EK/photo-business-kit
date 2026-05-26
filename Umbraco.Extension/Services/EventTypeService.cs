using Umbraco.Extension.Dtos;
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

    public async Task<EventTypeDto> CreateAsync(EventTypeDto dto)
    {
        var eventType = new EventTypes
        {
            Id = Guid.NewGuid(),
            Name = dto.EventTypeName
        };

        using var database = _databaseFactory.CreateDatabase();

        await database.InsertAsync(eventType);

        return new EventTypeDto
        {
            EventTypeName = eventType.Name
        };
    }

    public IEnumerable<EventTypeDto> GetAll()
    {
        using var database = _databaseFactory.CreateDatabase();

        return database.Fetch<EventTypes>()
            .Select(eventType => new EventTypeDto
            {
                EventTypeName = eventType.Name
            })
            .ToList();
    }
}
