using SkagenBooking.Core.Entities;
using SkagenBooking.Core.Enums;
using SkagenBooking.Core.Interfaces;

namespace SkagenBooking.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of room repository.
/// </summary>
public class InMemoryRoomRepository : IRoomRepository
{
    private readonly List<Room> _rooms = new()
    {
        new Room(1, RoomType.Single, 1),
        new Room(2, RoomType.Double, 2),
        new Room(3, RoomType.Double, 2),
        new Room(4, RoomType.Family, 3)
    };

    public List<Room> GetAll()
    {
        return _rooms;
    }

    public Room? GetById(int id)
    {
        return _rooms.FirstOrDefault(r => r.Id == id);
    }
}