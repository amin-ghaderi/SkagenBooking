using SkagenBooking.Core.Interfaces;

namespace SkagenBooking.Application.Bookings.Queries.GetBookings;

public sealed class GetBookingsUseCase : IGetBookingsUseCase
{
    private readonly IBookingAggregateRepository _bookingRepository;

    public GetBookingsUseCase(IBookingAggregateRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<IReadOnlyList<BookingListItemDto>> ExecuteAsync(GetBookingsQuery query, CancellationToken cancellationToken)
    {
        var bookings = await _bookingRepository.GetAllAsync(cancellationToken);
        return bookings
            .Where(b => !query.PropertyId.HasValue || b.PropertyId == query.PropertyId.Value)
            .Select(b => new BookingListItemDto
            {
                Id = b.Id,
                PropertyId = b.PropertyId,
                RoomId = b.RoomId,
                CheckIn = b.DateRange.CheckIn,
                CheckOut = b.DateRange.CheckOut,
                GuestCount = b.GuestCount,
                NeedsParking = b.NeedsParking
            })
            .OrderBy(b => b.CheckIn)
            .ThenBy(b => b.RoomId)
            .ToList();
    }
}
