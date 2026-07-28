using SkagenBooking.Application.Abstractions;

namespace SkagenBooking.Application.Bookings.Queries.GetBookings;

public sealed class GetBookingsQuery : IQuery<IReadOnlyList<BookingListItemDto>>
{
    public required string UserId { get; init; }
    public int? PropertyId { get; init; }
}
