using SkagenBooking.Application.Abstractions;

namespace SkagenBooking.Application.Rooms.Queries.GetRooms;

public sealed class GetRoomsQuery : IQuery<IReadOnlyList<RoomListItemDto>>
{
    public int? PropertyId { get; init; }
}
