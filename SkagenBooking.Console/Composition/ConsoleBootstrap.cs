using SkagenBooking.Application.Bookings.Commands.CreateBooking;
using SkagenBooking.Application.Bookings.Events;
using SkagenBooking.Application.Bookings.Queries.GetBookings;
using SkagenBooking.Application.Common.DomainEvents;
using SkagenBooking.Application.Rooms.Queries.GetRooms;
using SkagenBooking.Console.Application;
using SkagenBooking.Core.Policies;
using SkagenBooking.Core.Services;
using SkagenBooking.Infrastructure.Time;
using SkagenBooking.Infrastructure.Persistence;
using SkagenBooking.Infrastructure.Repositories;

namespace SkagenBooking.Console.Composition;

/// <summary>
/// Composition root for the console application. Builds the object graph used by Program.cs.
/// </summary>
public static class ConsoleBootstrap
{
    public static (ConsoleAppService appService, CancellationToken cancellationToken) Build()
    {
        var cancellationToken = CancellationToken.None;

        // Infrastructure adapters (in-memory for now)
        var roomRepo = new InMemoryRoomRepository();
        var propertyRepo = new InMemoryPropertyRepository();
        var bookingRepo = new InMemoryBookingRepository();
        var parkingAllocationRepo = new InMemoryParkingRepository();

        // Domain services
        var pricingService = new BasicPricingService();
        var bookingWindowPolicy = new BookingWindowPolicy();
        var availabilityService = new AvailabilityService();
        var parkingAvailabilityService = new ParkingAvailabilityService();
        var domainEventDispatcher = new InMemoryDomainEventDispatcher();
        domainEventDispatcher.Register(new BookingCreatedDomainEventHandler());
        var outbox = new InMemoryOutbox();
        var unitOfWork = new InMemoryUnitOfWork();
        var clock = new SystemClock();

        // Application use cases
        var getRoomsUseCase = new GetRoomsUseCase(roomRepo);
        var getBookingsUseCase = new GetBookingsUseCase(bookingRepo);
        var createBookingUseCase = new CreateBookingUseCase(
            roomRepo,
            bookingRepo,
            parkingAllocationRepo,
            propertyRepo,
            pricingService,
            bookingWindowPolicy,
            availabilityService,
            parkingAvailabilityService,
            domainEventDispatcher,
            outbox,
            unitOfWork,
            clock);

        var appService = new ConsoleAppService(getRoomsUseCase, createBookingUseCase, getBookingsUseCase, roomRepo, pricingService);
        return (appService, cancellationToken);
    }
}
