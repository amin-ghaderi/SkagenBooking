using SkagenBooking.Core.Interfaces;

namespace SkagenBooking.Application.Rooms.Queries.GetRooms;

public sealed class GetRoomsUseCase : IGetRoomsUseCase
{
    private readonly IRoomRepository _roomRepository;

    public GetRoomsUseCase(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<IReadOnlyList<RoomListItemDto>> ExecuteAsync(GetRoomsQuery query, CancellationToken cancellationToken)
    {
        var rooms = await _roomRepository.GetAllAsync(query.PropertyId, cancellationToken);
        return rooms
            .Select(r => new RoomListItemDto
            {
                Id = r.Id,
                PropertyId = r.PropertyId,
                Type = r.Type.ToString(),
                Capacity = r.Capacity
            })
            .ToList();
    }
}
