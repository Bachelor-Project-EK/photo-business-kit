using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using Umbraco.Extension.Enum;
namespace Umbraco.Extension.Models;

[TableName(nameof(Payments))]
[PrimaryKey(nameof(Id), AutoIncrement = false)]
[ExplicitColumns]
public class Payments
{
    [Column(nameof(Id))]
    [PrimaryKeyColumn(AutoIncrement = false)]
    public required Guid Id { get; set; }

    [Column(nameof(OrderId))]
    public required Guid OrderId { get; set; }

    [Column(nameof(CreatedOn))]
    public required DateTimeOffset CreatedOn { get; set; }

    [Column(nameof(UpdatedOn))]
    public required DateTimeOffset UpdatedOn { get; set; }

    [Column(nameof(Paid))]
    public required bool Paid { get; set; }

    //[Column(nameof(TotalPrice))]
    //[SpecialDbType("money")]
    [Ignore]
    public required decimal TotalPrice { get; set; }

    [Column(nameof(PaymentStatus))]
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