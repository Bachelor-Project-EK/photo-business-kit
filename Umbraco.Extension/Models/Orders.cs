using NPoco;
using Umbraco.Cms.Core.Models.Entities;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Extension.Models;

[TableName("Orders")]
[PrimaryKey("Id", AutoIncrement = false)]
[ExplicitColumns]
public class Orders : EntityBase
{
    [Column("Id")]
    [PrimaryKeyColumn(AutoIncrement = false)]
    public Guid Id { get; set; }

    [Column("BookingId")] 
    public Guid BookingId { get; set; }

    [Column("PhotoPackageId")]
     public Guid PhotoPackageId { get; set; }

    [Column("CreatedOn")] 
    public DateTimeOffset CreatedOn { get; set; }

    [Column("UpdatedOn")]
     public DateTimeOffset UpdatedOn { get; set; }

    [Column("Comment")]
    [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
    public string? Comment { get; set; }
}