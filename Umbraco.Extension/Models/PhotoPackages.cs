using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Extension.Models;

[TableName(nameof(PhotoPackages))]
[PrimaryKey(nameof(Id), AutoIncrement = false)]
[ExplicitColumns]
public class PhotoPackages
{
    [Column(nameof(Id))]
    [PrimaryKeyColumn(AutoIncrement = false)]
    public required Guid Id { get; set; }

    [Column(nameof(EventTypeId))]
    [ForeignKey(typeof(EventTypes), Column = nameof(EventTypes.Id))]
     public required Guid EventTypeId { get; set; }

    [ResultColumn]
    [Reference(ReferenceType.OneToOne, ColumnName = nameof(EventTypeId), ReferenceMemberName = nameof(EventTypes.Id))]
    public EventTypes? EventType { get; set; }

    [Column(nameof(Name))]
    [Length(100)] 
    public required string Name { get; set; }

    [Column(nameof(PhotoCount))] 
    public required int PhotoCount { get; set; }
}