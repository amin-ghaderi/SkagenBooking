using SkagenBooking.Application.Rooms.Queries.GetRooms;
using SkagenBooking.Infrastructure.Repositories;

namespace SkagenBooking.Tests.Integration;

public class GetRoomsUseCaseIntegrationTests
{
    [Fact]
    public async Task GetRooms_Should_Return_All_Rooms_With_Rates()
    {
        var useCase = new GetRoomsUseCase(new InMemoryRoomRepository());

        var rooms = await useCase.ExecuteAsync(new GetRoomsQuery { PropertyId = null }, CancellationToken.None);

        Assert.Equal(4, rooms.Count);
        Assert.Contains(rooms, r => r.Id == 2 && r.Name == "Double" && r.NightlyRate == 700m && r.Currency == "DKK");
    }

    [Fact]
    public async Task GetRooms_Should_Filter_By_PropertyId()
    {
        var useCase = new GetRoomsUseCase(new InMemoryRoomRepository());

        var rooms = await useCase.ExecuteAsync(new GetRoomsQuery { PropertyId = 1 }, CancellationToken.None);

        Assert.Equal(4, rooms.Count);
        Assert.All(rooms, r => Assert.Equal(1, r.PropertyId));
    }
}
