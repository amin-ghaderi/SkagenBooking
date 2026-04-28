using SkagenBooking.Application.Bookings.Commands.CancelBooking;
using SkagenBooking.Application.Bookings.Commands.CreateBooking;
using SkagenBooking.Application.Bookings.Commands.UpdateBooking;
using SkagenBooking.Application.Bookings.Events;
using SkagenBooking.Application.Common.DomainEvents;
using SkagenBooking.Core.Policies;
using SkagenBooking.Core.Services;
using SkagenBooking.Infrastructure.Persistence;
using SkagenBooking.Infrastructure.Repositories;
using SkagenBooking.Tests.Fakes;

namespace SkagenBooking.Tests.Integration;

public class UpdateAndCancelBookingUseCaseIntegrationTests
{
    private static (
        CreateBookingUseCase createUseCase,
        UpdateBookingUseCase updateUseCase,
        CancelBookingUseCase cancelUseCase)
        BuildUseCases()
    {
        var roomRepo = new InMemoryRoomRepository();
        var bookingRepo = new InMemoryBookingRepository();
        var parkingRepo = new InMemoryParkingRepository();
        var pricing = new BasicPricingService();
        var policy = new BookingWindowPolicy();
        var availabilityService = new AvailabilityService();
        var parkingAvailabilityService = new ParkingAvailabilityService();
        var propertyRepo = new InMemoryPropertyRepository();
        var dispatcher = new InMemoryDomainEventDispatcher();
        dispatcher.Register(new BookingCreatedDomainEventHandler());
        var outbox = new InMemoryOutbox();
        var uow = new InMemoryUnitOfWork();
        var clock = new FakeClock(new DateTime(2026, 4, 1));

        var createUseCase = new CreateBookingUseCase(
            roomRepo,
            bookingRepo,
            parkingRepo,
            propertyRepo,
            pricing,
            policy,
            availabilityService,
            parkingAvailabilityService,
            dispatcher,
            outbox,
            uow,
            clock);

        var updateUseCase = new UpdateBookingUseCase(
            bookingRepo,
            roomRepo,
            propertyRepo,
            parkingRepo,
            availabilityService,
            parkingAvailabilityService,
            policy,
            uow,
            clock);

        var cancelUseCase = new CancelBookingUseCase(
            bookingRepo,
            parkingRepo,
            uow);

        return (createUseCase, updateUseCase, cancelUseCase);
    }

    [Fact]
    public async Task UpdateBooking_Should_Fail_With_Conflict_On_Overlap()
    {
        var (createUseCase, updateUseCase, _) = BuildUseCases();

        var first = await createUseCase.ExecuteAsync(new CreateBookingCommand
        {
            PropertyId = 1,
            RoomId = 2,
            CheckInDate = new DateTime(2026, 4, 20, 14, 0, 0),
            CheckOutDate = new DateTime(2026, 4, 22, 11, 0, 0),
            GuestCount = 2,
            NeedsParking = false,
            IsLateArrival = false
        }, CancellationToken.None);

        var second = await createUseCase.ExecuteAsync(new CreateBookingCommand
        {
            PropertyId = 1,
            RoomId = 2,
            CheckInDate = new DateTime(2026, 4, 24, 14, 0, 0),
            CheckOutDate = new DateTime(2026, 4, 26, 11, 0, 0),
            GuestCount = 2,
            NeedsParking = false,
            IsLateArrival = false
        }, CancellationToken.None);

        var update = await updateUseCase.ExecuteAsync(new UpdateBookingCommand
        {
            BookingId = second.BookingId!.Value,
            CheckInDate = new DateTime(2026, 4, 21, 14, 0, 0),
            CheckOutDate = new DateTime(2026, 4, 23, 11, 0, 0),
            GuestCount = 2,
            NeedsParking = false,
            IsLateArrival = false
        }, CancellationToken.None);

        Assert.True(first.IsCreated);
        Assert.True(second.IsCreated);
        Assert.False(update.IsSuccess);
        Assert.Equal(UpdateBookingError.Conflict, update.Error);
    }

    [Fact]
    public async Task CancelBooking_Should_Free_Parking_Slot()
    {
        var (createUseCase, _, cancelUseCase) = BuildUseCases();

        var first = await createUseCase.ExecuteAsync(new CreateBookingCommand
        {
            PropertyId = 1,
            RoomId = 1,
            CheckInDate = new DateTime(2026, 4, 20, 14, 0, 0),
            CheckOutDate = new DateTime(2026, 4, 22, 11, 0, 0),
            GuestCount = 1,
            NeedsParking = true,
            IsLateArrival = false
        }, CancellationToken.None);

        var second = await createUseCase.ExecuteAsync(new CreateBookingCommand
        {
            PropertyId = 1,
            RoomId = 2,
            CheckInDate = new DateTime(2026, 4, 20, 14, 0, 0),
            CheckOutDate = new DateTime(2026, 4, 22, 11, 0, 0),
            GuestCount = 2,
            NeedsParking = true,
            IsLateArrival = false
        }, CancellationToken.None);

        var thirdBeforeCancel = await createUseCase.ExecuteAsync(new CreateBookingCommand
        {
            PropertyId = 1,
            RoomId = 3,
            CheckInDate = new DateTime(2026, 4, 20, 14, 0, 0),
            CheckOutDate = new DateTime(2026, 4, 22, 11, 0, 0),
            GuestCount = 2,
            NeedsParking = true,
            IsLateArrival = false
        }, CancellationToken.None);

        var cancel = await cancelUseCase.ExecuteAsync(new CancelBookingCommand { BookingId = first.BookingId!.Value }, CancellationToken.None);

        var thirdAfterCancel = await createUseCase.ExecuteAsync(new CreateBookingCommand
        {
            PropertyId = 1,
            RoomId = 3,
            CheckInDate = new DateTime(2026, 4, 20, 14, 0, 0),
            CheckOutDate = new DateTime(2026, 4, 22, 11, 0, 0),
            GuestCount = 2,
            NeedsParking = true,
            IsLateArrival = false
        }, CancellationToken.None);

        Assert.True(first.IsCreated);
        Assert.True(second.IsCreated);
        Assert.False(thirdBeforeCancel.IsCreated);
        Assert.True(cancel.IsSuccess);
        Assert.True(thirdAfterCancel.IsCreated);
    }
}

