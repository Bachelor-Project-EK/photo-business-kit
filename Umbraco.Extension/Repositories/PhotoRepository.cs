using NPoco.Linq;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extension.Models;

namespace Umbraco.Extension.Repositories
{
    public class PhotoRepository(IUmbracoDatabaseFactory factory) : IPhotoRepository
    {
        public async Task<IAsyncQueryProvider<Photos>> GetPhotosByAlbumIdAsync(Guid albumId, CancellationToken cancellationToken = default)
        {
            await using var database = factory.CreateDatabase();
            var result = database.QueryAsync<Photos>()
                .Where(x => x.AlbumId == albumId);

            return result;
        }
        public async Task<Photos?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await using var database = factory.CreateDatabase();
            var result = await database.SingleOrDefaultByIdAsync<Photos>(id, cancellationToken);
            return result;
        }
    }
}
