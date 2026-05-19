using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using Umbraco.Extension.Enum;

namespace Umbraco.Extension.Models;

[TableName("Bookings")]
[PrimaryKey("Id", AutoIncrement = false)]
[ExplicitColumns]
public class Bookings
{
    [Column("Id")]
    [PrimaryKeyColumn(AutoIncrement = false)]
    public required Guid Id { get; set; }

    [Column("nodeId")]
    public required int NodeId { get; set; }

    [Column("CreatedOn")]
    public required DateTimeOffset CreatedOn { get; set; }

    [Column("UpdatedOn")]
    public required DateTimeOffset UpdatedOn { get; set; }

    [Column("StartDate")]
    public required DateTimeOffset StartDate { get; set; }

    [Column("EndDate")]
    public required DateTimeOffset EndDate { get; set; }

    [Column("Status")]
    [Length(50)]
    public required string StatusValue { get; set; }

    [Ignore]
    public BookingStatus Status
    {
        get => System.Enum.TryParse(StatusValue, out BookingStatus status) &&
            System.Enum.IsDefined(typeof(BookingStatus), status) ?
            status :
            throw new InvalidOperationException($"Invalid booking status value: {StatusValue}");
        set => StatusValue = value.ToString();
    }
}