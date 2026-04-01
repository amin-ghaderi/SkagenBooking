using SkagenBooking.Core.Entities;
using SkagenBooking.Core.Interfaces;
using SkagenBooking.Core.ValueObjects;

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

    public Task<IReadOnlyList<Booking>> GetByRoomAsync(int roomId, CancellationToken cancellationToken)
    {
        IReadOnlyList<Booking> bookings = _bookings
            .Where(b => b.RoomId == roomId)
            .ToList();
        return Task.FromResult(bookings);
    }

    public Task<bool> ExistsOverlapAsync(int roomId, DateRange requestedRange, CancellationToken cancellationToken)
    {
        var overlaps = _bookings.Any(b => b.RoomId == roomId && b.DateRange.Overlaps(requestedRange));
        return Task.FromResult(overlaps);
    }
}