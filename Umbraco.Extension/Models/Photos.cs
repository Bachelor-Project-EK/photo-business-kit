using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Extension.Models;
[TableName("Photos")]
[PrimaryKey("Id", AutoIncrement = false)]
[ExplicitColumns]
public class Photos
{
    [Column("Id")]
    [PrimaryKeyColumn(AutoIncrement = false)]
    public Guid Id { get; set; }

    [Column("AlbumId")] public Guid AlbumId { get; set; }

    [Column("Name")]
    [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
    public string Name { get; set; } = string.Empty;

    [Column("CreatedOn")] public DateTimeOffset CreatedOn { get; set; }

    [Column("UpdatedOn")] public DateTimeOffset UpdatedOn { get; set; }

    [Column("Link")]
    [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
    public string Link { get; set; } = string.Empty;
}