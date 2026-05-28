using Umbraco.Extension.Dtos.Queries;

namespace Umbraco.Extension.Dtos;

public class AlbumDto
{
    public Guid BookingId { get; set; }
    public required string Name { get; set; }
    public  DateTimeOffset CreatedOn { get; set; }
    public  DateTimeOffset UpdatedOn { get; set; }
    public ICollection<PhotoPackageQueryDto>? Photos { get; set; }
}