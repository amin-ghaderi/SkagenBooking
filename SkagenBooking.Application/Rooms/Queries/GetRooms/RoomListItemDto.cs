namespace SkagenBooking.Application.Rooms.Queries.GetRooms;

public sealed class RoomListItemDto
{
    public int Id { get; init; }
    public int PropertyId { get; init; }
    public string Type { get; init; } = string.Empty;
    public int Capacity { get; init; }
}
