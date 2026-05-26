namespace Umbraco.Extension.Dtos.Queries;

public class PhotoPackageQueryDto
{
    public Guid Id { get; init; }
    public Guid EventTypeId { get; init; }
    public required string PhotoPackageName { get; init; }
    public int PhotoCount { get; init; }
    public decimal? PhotoPrice { get; init; }
    public decimal? HourlyPrice { get; init; }
}
