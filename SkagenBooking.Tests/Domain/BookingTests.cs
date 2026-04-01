using SkagenBooking.Core.Entities;
using SkagenBooking.Core.Enums;
using SkagenBooking.Core.ValueObjects;
using SkagenBooking.Core.Policies;

namespace SkagenBooking.Tests.Domain;

public class BookingTests
{
    private static readonly BookingWindowPolicy Policy = new();

    [Fact]
    public void TryCreate_Should_Start_With_Pending_Status()
    {
        var room = new Room(2, 1, RoomType.Double, 2, new Money(700m, "DKK"));
        var result = Booking.TryCreate(
            room,
            new DateRange(new DateTime(2026, 4, 10, 14, 0, 0), new DateTime(2026, 4, 12, 11, 0, 0)),
            guestCount: 2,
            needsParking: false,
            isLateArrival: false,
            estimatedArrivalTime: null,
            Policy,
            currentDate: new DateTime(2026, 4, 1));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Booking);
        Assert.Equal(BookingStatus.Pending, result.Booking!.Status);
    }

    [Fact]
    public void TryCreate_Should_Require_Eta_For_Late_Arrival()
    {
        var room = new Room(2, 1, RoomType.Double, 2, new Money(700m, "DKK"));
        var result = Booking.TryCreate(
            room,
            new DateRange(new DateTime(2026, 4, 10, 20, 30, 0), new DateTime(2026, 4, 12, 11, 0, 0)),
            guestCount: 2,
            needsParking: false,
            isLateArrival: true,
            estimatedArrivalTime: null,
            Policy,
            currentDate: new DateTime(2026, 4, 1));

        Assert.False(result.IsSuccess);
        Assert.Equal("Estimated arrival time is required for arrivals after 20:00.", result.Error);
    }

    [Fact]
    public void Confirm_After_Cancel_Should_Throw()
    {
        var room = new Room(2, 1, RoomType.Double, 2, new Money(700m, "DKK"));
        var createResult = Booking.TryCreate(
            room,
            new DateRange(new DateTime(2026, 4, 10, 14, 0, 0), new DateTime(2026, 4, 12, 11, 0, 0)),
            guestCount: 2,
            needsParking: false,
            isLateArrival: false,
            estimatedArrivalTime: null,
            Policy,
            currentDate: new DateTime(2026, 4, 1));

        var booking = createResult.Booking!;
        booking.Cancel();

        Assert.Throws<InvalidOperationException>(() => booking.Confirm());
    }
}

