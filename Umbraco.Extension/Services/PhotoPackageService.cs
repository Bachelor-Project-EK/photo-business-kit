using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extension.Dtos.Commands;
using Umbraco.Extension.Dtos.Queries;
using Umbraco.Extension.Models;

namespace Umbraco.Extension.Services;

public class PhotoPackageService
{
    private readonly IUmbracoDatabaseFactory _databaseFactory;

    public PhotoPackageService(IUmbracoDatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }

    public async Task<PhotoPackageCommandDto> CreateAsync(PhotoPackageCommandDto dto)
    {
        var photoPackage = new PhotoPackages
        {
            Id = Guid.NewGuid(),
            EventTypeId = dto.EventTypeId,
            Name = dto.PhotoPackageName,
            PhotoCount = dto.PhotoCount,
            PhotoPrice = dto.PhotoPrice,
            HourlyPrice = dto.HourlyPrice
        };

        using var database = _databaseFactory.CreateDatabase();

        await database.InsertAsync(photoPackage);

        return new PhotoPackageCommandDto
        {
            EventTypeId = photoPackage.EventTypeId,
            PhotoPackageName = photoPackage.Name,
            PhotoCount = photoPackage.PhotoCount,
            PhotoPrice = photoPackage.PhotoPrice,
            HourlyPrice = photoPackage.HourlyPrice
        };
    }

    public IEnumerable<PhotoPackageQueryDto> GetAll()
    {
        using var database = _databaseFactory.CreateDatabase();

        return database.Fetch<PhotoPackages>()
            .Select(photoPackage => new PhotoPackageQueryDto
            {
                Id = photoPackage.Id,
                EventTypeId = photoPackage.EventTypeId,
                PhotoPackageName = photoPackage.Name,
                PhotoCount = photoPackage.PhotoCount,
                PhotoPrice = photoPackage.PhotoPrice,
                HourlyPrice = photoPackage.HourlyPrice
            })
            .ToList();
    }

    public async Task<PhotoPackageCommandDto?> UpdateAsync(PhotoPackageCommandDto dto, Guid id)
    {
        using var database = _databaseFactory.CreateDatabase();

        var photoPackage = await database.SingleOrDefaultByIdAsync<PhotoPackages>(id);

        if (photoPackage is null)
        {
            return null;
        }

        photoPackage.EventTypeId = dto.EventTypeId;
        photoPackage.Name = dto.PhotoPackageName;
        photoPackage.PhotoCount = dto.PhotoCount;
        photoPackage.PhotoPrice = dto.PhotoPrice;
        photoPackage.HourlyPrice = dto.HourlyPrice;

        await database.UpdateAsync(photoPackage);

        return new PhotoPackageCommandDto
        {
            EventTypeId = photoPackage.EventTypeId,
            PhotoPackageName = photoPackage.Name,
            PhotoCount = photoPackage.PhotoCount,
            PhotoPrice = photoPackage.PhotoPrice,
            HourlyPrice = photoPackage.HourlyPrice
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var database = _databaseFactory.CreateDatabase();

        var photoPackage = await database.SingleOrDefaultByIdAsync<PhotoPackages>(id);

        if (photoPackage is null)
        {
            return false;
        }

        await database.DeleteAsync(photoPackage);

        return true;
    }
}
