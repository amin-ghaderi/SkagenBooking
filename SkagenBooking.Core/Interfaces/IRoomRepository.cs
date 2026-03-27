using SkagenBooking.Core.Entities;

namespace SkagenBooking.Core.Interfaces;

/// <summary>
/// Provides access to room data.
/// </summary>
public interface IRoomRepository
{
    /// <summary>
    /// Gets all available rooms.
    /// </summary>
    List<Room> GetAll();

    /// <summary>
    /// Gets a room by its identifier.
    /// </summary>
    Room? GetById(int id);
}