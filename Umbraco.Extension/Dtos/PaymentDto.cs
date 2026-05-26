using Umbraco.Extension.Enum;

namespace Umbraco.Extension.Dtos;

public class PaymentDto
{
    public Guid OrderId { get; set; }
    public bool Paid { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public decimal TotalPrice { get; set; }
}