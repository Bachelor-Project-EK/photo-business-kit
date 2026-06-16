using System;
using System.Collections.Generic;
using System.Text;
using NPoco.Linq;
using Umbraco.Extension.Models;

namespace Umbraco.Extension.Repositories
{
    public interface IPhotoRepository
    {
        Task<IAsyncQueryProvider<Photos>> GetPhotosByAlbumIdAsync(Guid albumId, CancellationToken cancellationToken = default);
        Task<Photos?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
