using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Extension.Models;
[TableName(nameof(Photos))]
[PrimaryKey(nameof(Id), AutoIncrement = false)]
[ExplicitColumns]
public class Photos
{
    [Column(nameof(Id))]
    [PrimaryKeyColumn(AutoIncrement = false)]
    public required Guid Id { get; set; }

    [Column(nameof(AlbumId))]
    [ForeignKey(typeof(Albums), Column = nameof(Albums.Id))]
     public required Guid AlbumId { get; set; }

    [ResultColumn]
    [Reference(ReferenceType.OneToOne, ColumnName = nameof(AlbumId), ReferenceMemberName = nameof(Albums.Id))]
    public Albums? Album { get; set; }

    [Column(nameof(Name))]
    [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
    public required string Name { get; set; }

    [Column(nameof(CreatedOn))]
     public required DateTimeOffset CreatedOn { get; set; }

    [Column(nameof(UpdatedOn))]
     public required DateTimeOffset UpdatedOn { get; set; }

    [Column(nameof(Link))]
    [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
    public required string Link { get; set; }

}