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
     public required Guid EventTypeId { get; set; }

    [Column(nameof(Name))]
    [Length(100)] 
    public required string Name { get; set; }

    [Column(nameof(PhotoPrice))]
    public decimal? PhotoPrice { get; set; }

    [Column(nameof(HourlyPrice))]
    public decimal? HourlyPrice { get; set; }

    [Column(nameof(PhotoCount))] 
    public required int PhotoCount { get; set; }
}