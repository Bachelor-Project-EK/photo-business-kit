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
    public Guid Id { get; set; }

    [Column("EventTypeId")] public Guid EventTypeId { get; set; }

    [Column("Name")] [Length(255)] public string Name { get; set; } = string.Empty;

    [Column("PhotoPrice")]
    public decimal? PhotoPrice { get; set; }

    [Column("HourlyPrice")]
    public decimal? HourlyPrice { get; set; }

    [Column("PhotoCount")] public int PhotoCount { get; set; }
}