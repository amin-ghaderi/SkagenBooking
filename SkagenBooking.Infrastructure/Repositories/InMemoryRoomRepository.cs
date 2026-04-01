using SkagenBooking.Core.Entities;
using SkagenBooking.Core.Enums;
using SkagenBooking.Core.Interfaces;
using SkagenBooking.Core.ValueObjects;

namespace SkagenBooking.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of room repository.
/// </summary>
public class InMemoryRoomRepository : IRoomRepository
{
    private readonly List<Room> _rooms = new()
    {
        new Room(1, 1, RoomType.Single, 1, new Money(550m, "DKK")),
        new Room(2, 1, RoomType.Double, 2, new Money(700m, "DKK")),
        new Room(3, 1, RoomType.Double, 2, new Money(765m, "DKK")),
        new Room(4, 1, RoomType.Family, 3, new Money(850m, "DKK"))
    };

    public Task<IReadOnlyList<Room>> GetAllAsync(int? propertyId, CancellationToken cancellationToken)
    {
        IReadOnlyList<Room> rooms = propertyId.HasValue
            ? _rooms.Where(r => r.PropertyId == propertyId.Value).ToList()
            : _rooms;
        return Task.FromResult(rooms);
    }

    public Task<Room?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_rooms.FirstOrDefault(r => r.Id == id));
    }
}