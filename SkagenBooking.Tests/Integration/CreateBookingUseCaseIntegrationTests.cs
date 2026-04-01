using SkagenBooking.Application.Bookings.Commands.CreateBooking;
using SkagenBooking.Application.Bookings.Events;
using SkagenBooking.Application.Common.DomainEvents;
using SkagenBooking.Application.Policies;
using SkagenBooking.Core.Services;
using SkagenBooking.Infrastructure.Persistence;
using SkagenBooking.Infrastructure.Repositories;

namespace SkagenBooking.Tests.Integration;

public class CreateBookingUseCaseIntegrationTests
{
    private static CreateBookingUseCase BuildUseCase(
        InMemoryRoomRepository roomRepo,
        InMemoryBookingRepository bookingRepo,
        InMemoryParkingRepository parkingRepo)
    {
        var pricing = new BasicPricingService();
        var policy = new BookingWindowPolicy();
        var dispatcher = new InMemoryDomainEventDispatcher();
        dispatcher.Register(new BookingCreatedDomainEventHandler());
        var outbox = new InMemoryOutbox();
        var uow = new InMemoryUnitOfWork();

        return new CreateBookingUseCase(roomRepo, bookingRepo, parkingRepo, pricing, policy, dispatcher, outbox, uow);
    }

    [Fact]
    public async Task CreateBooking_Should_Succeed_For_Valid_Request()
    {
        var roomRepo = new InMemoryRoomRepository();
        var bookingRepo = new InMemoryBookingRepository();
        var parkingRepo = new InMemoryParkingRepository();
        var useCase = BuildUseCase(roomRepo, bookingRepo, parkingRepo);

        var result = await useCase.ExecuteAsync(new CreateBookingCommand
        {
            PropertyId = 1,
            RoomId = 2,
            CheckInDate = new DateTime(2026, 4, 20, 14, 0, 0),
            CheckOutDate = new DateTime(2026, 4, 22, 11, 0, 0),
            GuestCount = 2,
            NeedsParking = false,
            IsLateArrival = false
        }, CancellationToken.None);

        Assert.True(result.IsCreated);
    }

    [Fact]
    public async Task CreateBooking_Should_Fail_When_Room_Overlaps()
    {
        var roomRepo = new InMemoryRoomRepository();
        var bookingRepo = new InMemoryBookingRepository();
        var parkingRepo = new InMemoryParkingRepository();
        var useCase = BuildUseCase(roomRepo, bookingRepo, parkingRepo);

        var command = new CreateBookingCommand
        {
            PropertyId = 1,
            RoomId = 3,
            CheckInDate = new DateTime(2026, 4, 20, 14, 0, 0),
            CheckOutDate = new DateTime(2026, 4, 23, 11, 0, 0),
            GuestCount = 2,
            NeedsParking = false,
            IsLateArrival = false
        };

        var first = await useCase.ExecuteAsync(command, CancellationToken.None);
        var second = await useCase.ExecuteAsync(command, CancellationToken.None);

        Assert.True(first.IsCreated);
        Assert.False(second.IsCreated);
    }

    [Fact]
    public async Task CreateBooking_Should_Fail_When_CheckIn_Outside_Window()
    {
        var useCase = BuildUseCase(new InMemoryRoomRepository(), new InMemoryBookingRepository(), new InMemoryParkingRepository());

        var result = await useCase.ExecuteAsync(new CreateBookingCommand
        {
            PropertyId = 1,
            RoomId = 1,
            CheckInDate = new DateTime(2026, 4, 20, 12, 0, 0),
            CheckOutDate = new DateTime(2026, 4, 21, 10, 0, 0),
            GuestCount = 1,
            NeedsParking = false,
            IsLateArrival = false
        }, CancellationToken.None);

        Assert.False(result.IsCreated);
    }
}
