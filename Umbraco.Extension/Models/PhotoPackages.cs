using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Extension.Models;

[TableName("PhotoPackages")]
[PrimaryKey("Id", AutoIncrement = false)]
[ExplicitColumns]
public class PhotoPackages
{
    [Column("Id")]
    [PrimaryKeyColumn(AutoIncrement = false)]
    public required Guid Id { get; set; }

    [Column("EventTypeId")]
     public required Guid EventTypeId { get; set; }

    [Column("Name")]
    [Length(100)] 
    public required string Name { get; set; }

    [Column("PhotoPrice")]
    public decimal? PhotoPrice { get; set; }

    [Column("HourlyPrice")]
    public decimal? HourlyPrice { get; set; }

    [Column("PhotoCount")] 
    public required int PhotoCount { get; set; }
}