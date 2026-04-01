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
        // In-memory repository does not filter by property, but DTO supports it for future DB.
        var result = new List<BookingListItemDto>();

        // naive in-memory listing: iterate over rooms 1-4 and collect bookings
        for (var roomId = 1; roomId <= 4; roomId++)
        {
            var bookings = await _bookingRepository.GetByRoomAsync(roomId, cancellationToken);
            result.AddRange(bookings.Select(b => new BookingListItemDto
            {
                Id = b.Id,
                PropertyId = b.PropertyId,
                RoomId = b.RoomId,
                CheckIn = b.DateRange.CheckIn,
                CheckOut = b.DateRange.CheckOut,
                GuestCount = b.GuestCount,
                NeedsParking = b.NeedsParking
            }));
        }

        return result
            .OrderBy(b => b.CheckIn)
            .ThenBy(b => b.RoomId)
            .ToList();
    }
}
