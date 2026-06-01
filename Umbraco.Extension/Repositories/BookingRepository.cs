using NPoco.Linq;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extension.Models;

namespace Umbraco.Extension.Repositories;

public class BookingRepository
{
    private readonly IUmbracoDatabaseFactory _databaseFactory;

    public BookingRepository(IUmbracoDatabaseFactory factory)
    {
        _databaseFactory = factory;
    }

    public async Task<Bookings?> GetAsync(
        Guid guid,
        CancellationToken cancellationToken)
    {
        using var database = _databaseFactory.CreateDatabase();

        var booking = await database.QueryAsync<Bookings>()
            .Include(x => x.PhotoPackage)
            .FirstOrDefault(x => x.Id == guid);
        
        return booking;
    }

    public async Task<IAsyncQueryProviderWithIncludes<Bookings>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        using var database = _databaseFactory.CreateDatabase();

        var query = database.QueryAsync<Bookings>()
            .Include(x => x.PhotoPackage);

        return query;
    }

    public async Task<object?> CreateAsync(
        Bookings booking,
        CancellationToken cancellationToken)
    {
        using var database = _databaseFactory.CreateDatabase();

        var result = await database.InsertAsync(booking, cancellationToken);

        return result;
    }

    public async Task<int> UpdateAsync(
        Bookings booking,
        CancellationToken cancellationToken)
    {
        using var database = _databaseFactory.CreateDatabase();
        return await database.UpdateAsync(booking, cancellationToken);
    }

     public async Task<int> DeleteAsync(
        Bookings booking,
        CancellationToken cancellationToken)
    {
        using var database = _databaseFactory.CreateDatabase();
        return await database.DeleteAsync(booking, cancellationToken);
    }
}
