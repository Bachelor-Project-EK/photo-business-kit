using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Extension.Dtos;
using Umbraco.Extension.Models;

namespace Umbraco.Extension.Mappers
{
    public static class BookingDtoMapping
    {
        public static BookingDto ToDto(this Bookings bookings)
        {
            return new BookingDto
            {
                StartDate = bookings.StartDate,
                EndDate = bookings.EndDate,
                Comment = bookings.Comment,
            };
        }

        public static Bookings ToDomain(this BookingDto bookingDto, Bookings bookings)
        {
            return new Bookings
            {
                Id = Guid.NewGuid(),
                NodeId = bookings.NodeId,
                CreatedOn = bookings.CreatedOn,
                UpdatedOn = bookings.UpdatedOn,
                StartDate = bookingDto.StartDate,
                EndDate = bookingDto.EndDate,
                StatusValue = bookings.Status.ToString(),
                Status = bookings.Status,
                PhotoPackageId = bookings.PhotoPackageId,
            };
        }
    }
}
