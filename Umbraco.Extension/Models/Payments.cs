using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using Umbraco.Extension.Enum;
namespace Umbraco.Extension.Models;

[TableName("Payments")]
[PrimaryKey("Id", AutoIncrement = false)]
[ExplicitColumns]
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

    [Column("TotalPrice")]
    public required decimal TotalPrice { get; set; }

    [Column("Status")]
    [Length(50)]
    public required string PaymentStatus { get; set; }

    [Ignore]
    public PaymentStatus Status
    {
        get => System.Enum.TryParse(PaymentStatus, out PaymentStatus status) &&
            System.Enum.IsDefined(typeof(PaymentStatus), status) ?
            status :
            throw new InvalidOperationException($"Invalid payment status value: {PaymentStatus}");
        set => PaymentStatus = value.ToString();
    }
}