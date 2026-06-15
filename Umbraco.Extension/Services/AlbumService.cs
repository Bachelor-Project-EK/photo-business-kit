using Umbraco.Extension.Dtos;
using Umbraco.Extension.Models;
using Umbraco.Extension.Repositories;

namespace Umbraco.Extension.Services
{
    public class AlbumService(AlbumRepository albumRepository)
    {
        public async Task<List<Albums>?> GetAllAsync()
        {
            return await albumRepository.GetAllAsync();
        }
        public async Task<Albums?> GetAlbumByBookingIdAsync(Guid bookingId)
        {
            return await albumRepository.GetAlbumByBookingIdAsync(bookingId);
        }
        public async Task<AlbumDto> CreateAsync(
            Albums album,
            CancellationToken cancellationToken = default)
        {
            await albumRepository.InsertAsync(album, cancellationToken);

            return new AlbumDto
            {
                BookingId = album.BookingId,
                Name = album.Name
            };
        }
    }
}
