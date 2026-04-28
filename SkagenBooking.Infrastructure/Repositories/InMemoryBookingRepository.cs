using SkagenBooking.Core.Entities;
using SkagenBooking.Core.Interfaces;

namespace SkagenBooking.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of booking repository.
/// </summary>
public class InMemoryBookingRepository : IBookingAggregateRepository
{
    private readonly List<Booking> _bookings = new();

    public Task AddAsync(Booking booking, CancellationToken cancellationToken)
    {
        _bookings.Add(booking);
        return Task.CompletedTask;
    }

    public Task<Booking?> GetByIdAsync(int bookingId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_bookings.FirstOrDefault(x => x.Id == bookingId));
    }

    public Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Booking> bookings = _bookings
            .OrderBy(x => x.DateRange.CheckIn)
            .ThenBy(x => x.RoomId)
            .ToList();
        return Task.FromResult(bookings);
    }

    public Task<IReadOnlyList<Booking>> GetByRoomAsync(int roomId, CancellationToken cancellationToken)
    {
        IReadOnlyList<Booking> bookings = _bookings
            .Where(b => b.RoomId == roomId)
            .ToList();
        return Task.FromResult(bookings);
    }
}