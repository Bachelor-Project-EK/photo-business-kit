using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using Umbraco.Extension.Enum;
namespace Umbraco.Extension.Models;

public class Payments
{
    [Column("Id")]
    [PrimaryKeyColumn(AutoIncrement = false)]
    public Guid Id { get; set; }

    [Column("OrderId")] 
    public Guid OrderId { get; set; }

    [Column("CreatedOn")]
     public DateTimeOffset CreatedOn { get; set; }

    [Column("UpdatedOn")]
     public DateTimeOffset UpdatedOn { get; set; }

    [Column("Paid")] 
    public bool Paid { get; set; }

    [Column("Status")] 
    public PaymentStatus Status { get; set; }

    [Column("TotalPrice")]
     public decimal TotalPrice { get; set; }
}