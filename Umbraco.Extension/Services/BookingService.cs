using Umbraco.Cms.Core.Services;
using Umbraco.Extension.Dtos;
using Umbraco.Extension.Dtos.Filters;
using Umbraco.Extension.Enums;
using Umbraco.Extension.Models;
using Umbraco.Extension.Repositories;

namespace Umbraco.Extension.Services;

public class BookingService
{
    private readonly BookingRepository _repository;
    private readonly IMemberService _memberService;

    public BookingService(BookingRepository repository, IMemberService memberService)
    {
        _repository = repository;
        _memberService = memberService;
    }

    public async Task<Bookings?> GetAsync(
        Guid guid,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAsync(guid, cancellationToken);
    }

    public async Task<IEnumerable<Bookings>?> GetAllAsync(
        Pagination pagination,
        CancellationToken cancellationToken)
    {
        var query = await _repository.GetAllAsync(cancellationToken);
        var paginated = await query.ToPage(pagination.Page, pagination.PageSize, cancellationToken);
        return paginated.Items;
    }

    public async Task<object?> CreateAsync(
        BookingDto dto,
        string email,
        CancellationToken cancellationToken)
    {
        var member = _memberService.GetByEmail(email);

        if (member is not null)
        {
            var booking = new Bookings()
            {
                Id = Guid.NewGuid(),
                NodeId = member.Id,
                CreatedOn = DateTimeOffset.UtcNow,
                UpdatedOn = DateTimeOffset.UtcNow,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Comment = dto.Comment,
                PhotoPackageId = dto.PhotoPackageId,
                Status = BookingStatus.Propose.ToString()
            };
            
            return await _repository.CreateAsync(booking, cancellationToken);
        }        

        return null;
    }

    public async Task<int?> ProposeUpdateAsync(
        Guid guid,
        BookingDto dto,
        CancellationToken cancellationToken)
    {
        var booking = await _repository.GetAsync(guid, cancellationToken);
        if (booking is not null && (booking.Status == BookingStatus.Pending.ToString()
            || booking.Status == BookingStatus.Propose.ToString()))
        {
            booking.UpdatedOn = DateTimeOffset.UtcNow;
            booking.StartDate = dto.StartDate;
            booking.EndDate = dto.EndDate;
            booking.Comment = dto.Comment;
            booking.PhotoPackageId = dto.PhotoPackageId;
            booking.Status = BookingStatus.Propose.ToString();
            return await _repository.UpdateAsync(booking, cancellationToken);
        }
        return null;
    }

    public async Task<int?> ApproveAsync(
        Guid guid,
        CancellationToken cancellationToken)
    {
        var booking = await _repository.GetAsync(guid, cancellationToken);
        if (booking is not null && booking.Status == BookingStatus.Pending.ToString())
        {
            booking.UpdatedOn = DateTimeOffset.UtcNow;
            booking.Status = BookingStatus.Approved.ToString();
            return await _repository.UpdateAsync(booking, cancellationToken);
        }
        return null;
    }

    public async Task<int?> RejectAsync(
        Guid guid,
        CancellationToken cancellationToken)
    {
        var booking = await _repository.GetAsync(guid, cancellationToken);
        if (booking is not null && (booking.Status == BookingStatus.Pending.ToString()
            || booking.Status == BookingStatus.Propose.ToString()))
        {
            booking.UpdatedOn = DateTimeOffset.UtcNow;
            booking.Status = BookingStatus.Rejected.ToString();
            return await _repository.UpdateAsync(booking, cancellationToken);
        }
        return null;
    }

    public async Task<int?> DeleteAsync(
        Guid guid,
        CancellationToken cancellationToken)
    {
        var booking = await _repository.GetAsync(guid, cancellationToken);
        if (booking is not null)
        {
            return await _repository.DeleteAsync(booking, cancellationToken);
        }
        return null;
    }
}
