using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Extension.Models;

[TableName(nameof(Orders))]
[PrimaryKey(nameof(Id), AutoIncrement = false)]
[ExplicitColumns]
public class Orders
{
    [Column(nameof(Id))]
    [PrimaryKeyColumn(AutoIncrement = false)]
    public required Guid Id { get; set; }

    [Column(nameof(BookingId))]
    [ForeignKey(typeof(Bookings), Column = nameof(Bookings.Id))]
    public required Guid BookingId { get; set; }

    [ResultColumn]
    [Reference(ReferenceType.OneToOne, ColumnName = nameof(BookingId), ReferenceMemberName = nameof(Bookings.Id))]
    public Bookings? Booking { get; set; }

    [Column(nameof(PhotoPackageId))]
    [ForeignKey(typeof(PhotoPackages), Column = nameof(PhotoPackages.Id))]
    public required Guid PhotoPackageId { get; set; }

    [ResultColumn]
    [Reference(ReferenceType.OneToOne, ColumnName = nameof(PhotoPackageId), ReferenceMemberName = nameof(PhotoPackages.Id))]
    public PhotoPackages? PhotoPackage { get; set; }

    [Column(nameof(CreatedOn))]
    public required DateTimeOffset CreatedOn { get; set; }

    [Column(nameof(UpdatedOn))]
    public required DateTimeOffset UpdatedOn { get; set; }

    [Column(nameof(Comment))]
    [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
    public string? Comment { get; set; }
}