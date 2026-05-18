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
    public Guid Id { get; set; }

    [Column("nodeId")]
    public int NodeId { get; set; }

    [Column("CreatedOn")]
    public DateTimeOffset CreatedOn { get; set; }

    [Column("UpdatedOn")]
    public DateTimeOffset UpdatedOn { get; set; }

    [Column("StartDate")]
    public DateTimeOffset StartDate { get; set; }

    [Column("EndDate")]
    public DateTimeOffset EndDate { get; set; }

    [Column("Status")]
    public BookingStatus Status { get; set; }

}