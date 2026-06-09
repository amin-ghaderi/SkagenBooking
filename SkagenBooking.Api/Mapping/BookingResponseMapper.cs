using SkagenBooking.Api.Contracts.Bookings;
using SkagenBooking.Application.Bookings.Queries.GetBookings;

namespace SkagenBooking.Api.Mapping;

internal static class BookingResponseMapper
{
    public static BookingResponse From(BookingListItemDto booking) => new()
    {
        Id = booking.Id,
        PropertyId = booking.PropertyId,
        RoomId = booking.RoomId,
        CheckInDate = booking.CheckIn,
        CheckOutDate = booking.CheckOut,
        GuestCount = booking.GuestCount,
        NeedsParking = booking.NeedsParking,
        Status = booking.Status,
        IsLateArrival = booking.IsLateArrival,
        EstimatedArrivalTime = booking.EstimatedArrivalTime
    };
}
