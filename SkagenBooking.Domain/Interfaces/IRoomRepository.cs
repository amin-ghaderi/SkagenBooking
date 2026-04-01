using SkagenBooking.Core.Entities;

namespace SkagenBooking.Core.Interfaces;

/// <summary>
/// Provides access to room data.
/// </summary>
public interface IRoomRepository
{
    Task<IReadOnlyList<Room>> GetAllAsync(int? propertyId, CancellationToken cancellationToken);
    Task<Room?> GetByIdAsync(int id, CancellationToken cancellationToken);
}