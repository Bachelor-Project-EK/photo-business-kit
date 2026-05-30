using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extension.Dtos.Commands;
using Umbraco.Extension.Dtos.Queries;
using Umbraco.Extension.Models;

namespace Umbraco.Extension.Services;

public class PaymentService
{
    private readonly IUmbracoDatabaseFactory _databaseFactory;

    public PaymentService(IUmbracoDatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }

    public async Task<PaymentCommandDto> CreateAsync(PaymentCommandDto dto)
    {
        var now = DateTimeOffset.UtcNow;
        var payment = new Payments()
        {
            Id = Guid.NewGuid(),
            OrderId = dto.OrderId,
            CreatedOn = now,
            UpdatedOn = now,
            Paid = dto.Paid,
            PaymentStatus = dto.PaymentStatus.ToString(),
            TotalPrice = dto.TotalPrice
        };

        using var database = _databaseFactory.CreateDatabase();

        await database.InsertAsync(payment);

        return new PaymentCommandDto
        {
            OrderId = payment.OrderId,
            Paid = payment.Paid,
            PaymentStatus = payment.Status,
            TotalPrice = payment.TotalPrice
        };
    }

    public IEnumerable<PaymentQueryDto> GetAll()
    {
        using var database = _databaseFactory.CreateDatabase();

        return database.Fetch<Payments>()
            .Select(payment => new PaymentQueryDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Paid = payment.Paid,
                PaymentStatus = payment.Status,
                TotalPrice = payment.TotalPrice
            })
            .ToList();
    }

    public async Task<PaymentCommandDto?> UpdateAsync(PaymentCommandDto dto, Guid id)
    {
        using var database = _databaseFactory.CreateDatabase();

        var payment = await database.SingleOrDefaultByIdAsync<Payments>(id);

        if (payment is null)
        {
            return null;
        }

        payment.OrderId = dto.OrderId;
        payment.UpdatedOn = DateTimeOffset.UtcNow;
        payment.Paid = dto.Paid;
        payment.Status = dto.PaymentStatus;
        payment.TotalPrice = dto.TotalPrice;

        await database.UpdateAsync(payment);

        return new PaymentCommandDto
        {
            OrderId = payment.OrderId,
            Paid = payment.Paid,
            PaymentStatus = payment.Status,
            TotalPrice = payment.TotalPrice
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var database = _databaseFactory.CreateDatabase();

        var payment = await database.SingleOrDefaultByIdAsync<Payments>(id);

        if (payment is null)
        {
            return false;
        }

        await database.DeleteAsync(payment);

        return true;
    }
}
