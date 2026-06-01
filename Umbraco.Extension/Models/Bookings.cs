using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using Umbraco.Extension.Enums;

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

    [Column(nameof(Status))]
    [Length(50)]
    public required string Status { get; set; }

    [Column(nameof(Comment))]
    [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
    public string? Comment { get; set; }

    [Column(nameof(PhotoPackageId))]
    [ForeignKey(typeof(PhotoPackages), Column = nameof(PhotoPackages.Id))]
    public required Guid PhotoPackageId { get; set; }

    [ResultColumn]
    [Reference(ReferenceType.OneToOne, ColumnName = nameof(PhotoPackageId), ReferenceMemberName = nameof(PhotoPackages.Id))]
    public PhotoPackages PhotoPackage { get; set; } = null!;
}