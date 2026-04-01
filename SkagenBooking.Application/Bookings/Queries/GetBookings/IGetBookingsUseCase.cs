namespace SkagenBooking.Application.Bookings.Queries.GetBookings;

public interface IGetBookingsUseCase
{
    Task<IReadOnlyList<BookingListItemDto>> ExecuteAsync(GetBookingsQuery query, CancellationToken cancellationToken);
}
