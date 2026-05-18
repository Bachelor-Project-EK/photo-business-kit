using NPoco;
using Umbraco.Cms.Core.Models.Entities;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Extension.Models;

[TableName("Orders")]
[PrimaryKey("Id", AutoIncrement = false)]
[ExplicitColumns]
public class Orders
{
    [Column("Id")]
    [PrimaryKeyColumn(AutoIncrement = false)]
    public required Guid Id { get; set; }

    [Column("BookingId")] 
    public required Guid BookingId { get; set; }

    [Column("PhotoPackageId")]
     public required Guid PhotoPackageId { get; set; }

    [Column("CreatedOn")] 
    public required DateTimeOffset CreatedOn { get; set; }

    [Column("UpdatedOn")]
     public required DateTimeOffset UpdatedOn { get; set; }

    [Column("Comment")]
    [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
    public string? Comment { get; set; }
}