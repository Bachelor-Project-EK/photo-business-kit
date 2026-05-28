using Umbraco.Extension.Enum;

namespace Umbraco.Extension.Dtos.Commands;

public class PaymentCommandDto
{
    public Guid OrderId { get; set; }
    public bool Paid { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public decimal TotalPrice { get; set; }
}