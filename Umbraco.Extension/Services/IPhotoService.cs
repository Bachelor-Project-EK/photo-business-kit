using Umbraco.Extension.Dtos;
using Umbraco.Extension.Dtos.Filters;
using Umbraco.Extension.Models;

namespace Umbraco.Extension.Services
{
    public interface IPhotoService
    {
        // GetByIdAsync
        Task<Photos?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Photos>> GetPhotosByAlbumIdAsync(
            Guid albumId,
            Pagination pagination,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Photos>> UploadToAlbumAsync(
            PhotoDto dto,
            CancellationToken cancellationToken = default);
    }
}
