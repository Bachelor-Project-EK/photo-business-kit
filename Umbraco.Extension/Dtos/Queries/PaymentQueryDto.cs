using Umbraco.Extension.Enum;

namespace Umbraco.Extension.Dtos.Queries;

public class PaymentQueryDto
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public bool Paid { get; init; }
    public PaymentStatus PaymentStatus { get; init; }
    public decimal TotalPrice { get; init; }
}