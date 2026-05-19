using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Extension.Models;
[TableName("Albums")]
[PrimaryKey("Id", AutoIncrement = false)]
[ExplicitColumns]
public class Albums
{
    [Column("Id")]
    [PrimaryKeyColumn(AutoIncrement = false)]
    public Guid Id { get; set; }

    [Column("OrderId")]
     public Guid OrderId { get; set; }

    [Column("Name")]
    [Length(50)]
    public string Name { get; set; } = string.Empty;

    [Column("CreatedOn")]
     public DateTimeOffset CreatedOn { get; set; }

    [Column("UpdatedOn")] 
    public DateTimeOffset UpdatedOn { get; set; }
}