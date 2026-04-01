using SkagenBooking.Core.Enums;
using SkagenBooking.Core.ValueObjects;

namespace SkagenBooking.Core.Entities;

/// <summary>
/// Represents a room in the booking system.
/// </summary>
public class Room
{
    /// <summary>
    /// Gets the unique identifier of the room.
    /// </summary>
    public int Id { get; private set; }
    public int PropertyId { get; private set; }

    /// <summary>
    /// Gets the type of the room (e.g., Single, Double, Family).
    /// </summary>
    public RoomType Type { get; private set; }

    /// <summary>
    /// Gets the maximum number of guests allowed in the room.
    /// </summary>
    public int Capacity { get; private set; }
    public Money NightlyRate { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Room"/> class.
    /// </summary>
    /// <param name="id">The room identifier.</param>
    /// <param name="type">The room type.</param>
    /// <param name="capacity">The maximum capacity of the room.</param>
    public Room(int id, int propertyId, RoomType type, int capacity, Money nightlyRate)
    {
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero.");

        Id = id;
        PropertyId = propertyId;
        Type = type;
        Capacity = capacity;
        NightlyRate = nightlyRate;
    }
}