namespace Umbraco.Extension.Dtos.Queries;

public class EventTypeQueryDto
{
    public Guid Id { get; init; }
    public required string EventTypeName { get; init; }
}
