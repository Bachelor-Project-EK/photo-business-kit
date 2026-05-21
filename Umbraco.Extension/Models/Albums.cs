using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Extension.Models;
[TableName(nameof(Albums))]
[PrimaryKey(nameof(Id), AutoIncrement = false)]
[ExplicitColumns]
public class Albums
{
    [Column(nameof(Id))]
    [PrimaryKeyColumn(AutoIncrement = false)]
    public Guid Id { get; set; }

    [Column(nameof(BookingsId))]
    [ForeignKey(typeof(Bookings), Column = nameof(Bookings.Id))]
    public Guid BookingsId { get; set; }

    [ResultColumn]
    [Reference(ReferenceType.OneToOne, ColumnName = nameof(BookingsId), ReferenceMemberName = nameof(Bookings.Id))]
    public Bookings? Booking { get; set; }

    [Column(nameof(Name))]
    [Length(50)]
    public string Name { get; set; } = string.Empty;

    [Column(nameof(CreatedOn))]
     public DateTimeOffset CreatedOn { get; set; }

    [Column(nameof(UpdatedOn))] 
    public DateTimeOffset UpdatedOn { get; set; }

    [ResultColumn]
    [Reference(ReferenceType.Many, ColumnName = nameof(Id), ReferenceMemberName = nameof(Id))]
    public ICollection<Photos?> ListOfPhoto { get; set; } = new List<Photos?>();
}