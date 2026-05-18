using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using Umbraco.Extension.Enum;
namespace Umbraco.Extension.Models;

public class Payments
{
    [Column("Id")]
    [PrimaryKeyColumn(AutoIncrement = false)]
    public required Guid Id { get; set; }

    [Column("OrderId")] 
    public required Guid OrderId { get; set; }

    [Column("CreatedOn")]
     public required DateTimeOffset CreatedOn { get; set; }

    [Column("UpdatedOn")]
     public required DateTimeOffset UpdatedOn { get; set; }

    [Column("Paid")] 
    public required bool Paid { get; set; }

    [Column("Status")] 
    public required PaymentStatus Status { get; set; }

    [Column("TotalPrice")]
     public required decimal TotalPrice { get; set; }
}