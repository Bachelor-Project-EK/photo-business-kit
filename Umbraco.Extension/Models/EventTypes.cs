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
    public Guid Id { get; set; }

    [Column("Name")] [Length(255)]
     public string Name { get; set; } = string.Empty;

}