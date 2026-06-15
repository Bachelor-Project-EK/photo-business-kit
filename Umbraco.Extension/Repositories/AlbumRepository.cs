using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extension.Models;

namespace Umbraco.Extension.Repositories;

public class AlbumRepository(IUmbracoDatabaseFactory factory)
{
    public async Task<List<Albums>?> GetAllAsync()
    {
        await using var database = factory.CreateDatabase();

        var result = await database.FetchAsync<Albums>();
        return result;
    }

    public async Task<Albums?> GetAlbumByBookingIdAsync(Guid bookingId)
    {
        await using var database = factory.CreateDatabase();

        var album = await database.Query<Albums>()
            .FirstOrDefaultAsync(x => x.BookingId == bookingId);

        //album.Photos = await database.Query<Photos>()
        //    .Where(x => x.AlbumId == album.Id)
        //    .ToListAsync();
        return album ?? null;
    }

    public async Task<object> InsertAsync(
        Albums album,
        CancellationToken cancellationToken = default)
    {
        await using var database = factory.CreateDatabase();

        return await database.InsertAsync(album, cancellationToken);
    }

    public async Task<int> UpdateAsync(Albums album)
    {
        await using var database = factory.CreateDatabase();
        return await database.UpdateAsync(album);
    }

    public async Task<int> DeleteAsync(Albums album)
    {
        await using var database = factory.CreateDatabase();
        return await database.DeleteAsync(album);
    }
}