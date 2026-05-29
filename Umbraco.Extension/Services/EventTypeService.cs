using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extension.Dtos.Queries;
using Umbraco.Extension.Dtos.Commands;
using Umbraco.Extension.Models;

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

    public IEnumerable<EventTypeQueryDto> MembersGetAll()
    {
        using var database = _databaseFactory.CreateDatabase();

        return database.Fetch<EventTypes>()
            .Select(eventType => new EventTypeQueryDto
            {
                EventTypeName = eventType.Name
            })
            .ToList();
    }

    public async Task<EventTypeCommandDto?> UpdateAsync(EventTypeCommandDto dto, Guid id)
    {
        using var database = _databaseFactory.CreateDatabase();

        var eventType = await database.SingleOrDefaultByIdAsync<EventTypes>(id);

        if (eventType is null)
        {
            return null;
        }

        eventType.Name = dto.EventTypeName;

        await database.UpdateAsync(eventType);

        return new EventTypeCommandDto
        {
            EventTypeName = eventType.Name
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var database = _databaseFactory.CreateDatabase();

        var eventType = await database.SingleOrDefaultByIdAsync<EventTypes>(id);

        if (eventType is null)
        {
            return false;
        }

        await database.DeleteAsync(eventType);

        return true;
    }
}
