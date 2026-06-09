using SkagenBooking.Api.Contracts.Rooms;
using SkagenBooking.Application.Rooms.Queries.GetRooms;

namespace SkagenBooking.Api.Mapping;

internal static class RoomResponseMapper
{
    public static RoomResponse From(RoomListItemDto room) => new()
    {
        Id = room.Id,
        PropertyId = room.PropertyId,
        Name = room.Name,
        Capacity = room.Capacity,
        NightlyRate = room.NightlyRate,
        Currency = room.Currency
    };
}
