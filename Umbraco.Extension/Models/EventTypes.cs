using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Extension.Models;

[TableName("EventTypes")]
[PrimaryKey("Id", AutoIncrement = false)]
[ExplicitColumns]
public class EventTypes
{
    [Column("Id")]
    [PrimaryKeyColumn(AutoIncrement = false)]
    public required Guid Id { get; set; }

    [Column("Name")]
    [Length(100)]
    public required string Name { get; set; } 

}