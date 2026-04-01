namespace SkagenBooking.Application.Rooms.Queries.GetRooms;

public interface IGetRoomsUseCase
{
    Task<IReadOnlyList<RoomListItemDto>> ExecuteAsync(GetRoomsQuery query, CancellationToken cancellationToken);
}
