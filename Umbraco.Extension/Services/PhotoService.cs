using FluentValidation;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extension.Dtos;
using Umbraco.Extension.Dtos.Filters;
using Umbraco.Extension.Models;
using Umbraco.Extension.Repositories;
using Umbraco.Extension.Services.Interface;


namespace Umbraco.Extension.Services
{
    public class PhotoService(
        IPhotoRepository photoRepository,
        IUmbracoDatabaseFactory databaseFactory,
        IAzureBlobPhotoStorageService photoStorageService,
        IValidator<PhotoDto> validator) : IPhotoService
    {
        public Task<Photos?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return photoRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<IReadOnlyList<Photos>> GetPhotosByAlbumIdAsync(Guid albumId, Pagination pagination,
            CancellationToken cancellationToken = default)
        {
            var photos =  await photoRepository.GetPhotosByAlbumIdAsync(albumId, cancellationToken);
            
            var pagedPhotos = await photos.ToPage(pagination.Page, pagination.PageSize, cancellationToken);
            return pagedPhotos.Items;
        }

        public async Task<IReadOnlyList<Photos>> UploadToAlbumAsync(
            PhotoDto dto,
            CancellationToken cancellationToken = default)
        {
            await validator.ValidateAndThrowAsync(dto, cancellationToken);

            await using var database = databaseFactory.CreateDatabase();
            {
                cancellationToken.ThrowIfCancellationRequested();

                var albumExists = await database.SingleOrDefaultByIdAsync<Albums>(dto.AlbumId, cancellationToken);

                if (albumExists is null)
                {
                    throw new InvalidOperationException($"Album with ID {dto.AlbumId} does not exist.");
                }


                //await database.CompleteTransactionAsync(cancellationToken);

            }
            var createdPhotos = new List<Photos>();
            var uploadedBlobNames = new List<string>();

            try
            {
                // upload every validated JPEG as its own blob
                foreach (var file in dto.Files!)
                {
                    var photoId = Guid.NewGuid();

                    var blobName = await photoStorageService.UploadAsync(
                        dto.AlbumId,
                        photoId,
                        file,
                        cancellationToken);

                    uploadedBlobNames.Add(blobName);

                    var now = DateTimeOffset.UtcNow;

                    var photo = new Photos
                    {
                        Id = photoId,
                        AlbumId = dto.AlbumId,
                        OriginalFileName = Path.GetFileName(file.FileName),
                        FileSizeInBytes = file.Length,
                        Link = "",
                        ContentType = file.ContentType,
                        BlobPath = blobName,
                        CreatedOn = now,
                        UpdatedOn = now
                    };

                    createdPhotos.Add(photo);
                }

                using (var scope = databaseFactory.CreateDatabase())
                {
                    foreach (var photo in createdPhotos)
                    {
                        await scope.InsertAsync(photo, cancellationToken);
                    }

                    await scope.CompleteTransactionAsync(cancellationToken);
                }
                // 5. Return information about all created photos.
                return createdPhotos;

            }
            catch
            {
                // If Blob Storage succeeded but database saving failed,
                // delete uploaded blobs again.
                foreach (var blobName in uploadedBlobNames)
                {
                    await photoStorageService.DeleteIfExistsAsync(
                        blobName,
                        CancellationToken.None);
                }

                throw;
            }

        }
    }

}