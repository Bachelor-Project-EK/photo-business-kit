using NPoco;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using Umbraco.Extension.Enum;


namespace Umbraco.Extension.Models;

[TableName(nameof(Bookings))]
[PrimaryKey(nameof(Id), AutoIncrement = false)]
[ExplicitColumns]
public class Bookings
{
    [Column(nameof(Id))]
    [PrimaryKeyColumn(AutoIncrement = false)]
    public required Guid Id { get; set; }

    [Column(nameof(NodeId))]
    public required int NodeId { get; set; }

    [Column(nameof(CreatedOn))]
    public required DateTimeOffset CreatedOn { get; set; }

    [Column(nameof(UpdatedOn))]
    public required DateTimeOffset UpdatedOn { get; set; }

    [Column(nameof(StartDate))]
    public required DateTimeOffset StartDate { get; set; }

    [Column(nameof(EndDate))]
    public required DateTimeOffset EndDate { get; set; }

    [Column(nameof(StatusValue))]
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