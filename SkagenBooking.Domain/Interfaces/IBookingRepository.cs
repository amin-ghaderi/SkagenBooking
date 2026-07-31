using SkagenBooking.Core.Entities;

namespace SkagenBooking.Core.Interfaces;

/// <summary>
/// Repository abstraction for accessing and persisting booking aggregates.
/// </summary>
public interface IBookingAggregateRepository
{
    Task AddAsync(Booking booking, CancellationToken cancellationToken);
    Task<Booking?> GetByIdAsync(int bookingId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken cancellationToken);
    /// <summary>
    /// Returns bookings owned by the specified authenticated user.
    /// </summary>
    Task<IReadOnlyList<Booking>> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Booking>> GetByRoomAsync(int roomId, CancellationToken cancellationToken);
}
