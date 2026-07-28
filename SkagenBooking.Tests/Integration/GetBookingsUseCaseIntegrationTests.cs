using SkagenBooking.Application.Bookings.Commands.CreateBooking;
using SkagenBooking.Application.Bookings.Events;
using SkagenBooking.Application.Bookings.Queries.GetBookings;
using SkagenBooking.Application.Common.DomainEvents;
using SkagenBooking.Core.Policies;
using SkagenBooking.Core.Services;
using SkagenBooking.Infrastructure.Persistence;
using SkagenBooking.Infrastructure.Repositories;
using SkagenBooking.Tests.Fakes;

namespace SkagenBooking.Tests.Integration;

public class GetBookingsUseCaseIntegrationTests
{
    [Fact]
    public async Task GetBookings_Should_Include_Status_And_Late_Arrival_Fields()
    {
        var roomRepo = new InMemoryRoomRepository();
        var bookingRepo = new InMemoryBookingRepository();
        var parkingRepo = new InMemoryParkingRepository();
        var propertyRepo = new InMemoryPropertyRepository();
        var dispatcher = new InMemoryDomainEventDispatcher();
        dispatcher.Register(new BookingCreatedDomainEventHandler());
        var createUseCase = new CreateBookingUseCase(
            roomRepo,
            bookingRepo,
            parkingRepo,
            propertyRepo,
            new BasicPricingService(),
            new BookingWindowPolicy(),
            new AvailabilityService(),
            new ParkingAvailabilityService(),
            dispatcher,
            new InMemoryOutbox(),
            new InMemoryUnitOfWork(),
            new FakeClock(new DateTime(2026, 4, 1)));

        await createUseCase.ExecuteAsync(new CreateBookingCommand
        {
            UserId = "test-user-1",
            PropertyId = 1,
            RoomId = 1,
            CheckInDate = new DateTime(2026, 4, 20, 14, 0, 0),
            CheckOutDate = new DateTime(2026, 4, 22, 11, 0, 0),
            GuestCount = 1,
            NeedsParking = false,
            IsLateArrival = true,
            EstimatedArrivalTime = new TimeOnly(21, 0)
        }, CancellationToken.None);

        var getUseCase = new GetBookingsUseCase(bookingRepo);
        var bookings = await getUseCase.ExecuteAsync(
            new GetBookingsQuery { UserId = "test-user-1" },
            CancellationToken.None);

        Assert.Single(bookings);
        Assert.Equal("Pending", bookings[0].Status);
        Assert.True(bookings[0].IsLateArrival);
        Assert.Equal(new TimeOnly(21, 0), bookings[0].EstimatedArrivalTime);
    }
}
