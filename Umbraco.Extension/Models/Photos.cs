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
    public required Guid Id { get; set; }

    [Column("AlbumId")]
     public required Guid AlbumId { get; set; }

    [Column("Name")]
    [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
    public required string Name { get; set; }

    [Column("CreatedOn")]
     public required DateTimeOffset CreatedOn { get; set; }

    [Column("UpdatedOn")]
     public required DateTimeOffset UpdatedOn { get; set; }

    [Column("Link")]
    [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
    public required string Link { get; set; }
}