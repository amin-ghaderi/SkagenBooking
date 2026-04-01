using SkagenBooking.Core.Entities;
using SkagenBooking.Core.ValueObjects;

namespace SkagenBooking.Core.Interfaces;

public interface IBookingAggregateRepository
{
    Task AddAsync(Booking booking, CancellationToken cancellationToken);
    Task<IReadOnlyList<Booking>> GetByRoomAsync(int roomId, CancellationToken cancellationToken);
    Task<bool> ExistsOverlapAsync(int roomId, DateRange requestedRange, CancellationToken cancellationToken);
}