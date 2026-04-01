using SkagenBooking.Core.Entities;
using SkagenBooking.Core.Enums;
using SkagenBooking.Core.ValueObjects;

namespace SkagenBooking.Tests.Domain;

public class BookingTests
{
    [Fact]
    public void Create_Should_Start_With_Pending_Status()
    {
        var booking = Booking.Create(
            propertyId: 1,
            roomId: 2,
            dateRange: new DateRange(new DateTime(2026, 4, 10), new DateTime(2026, 4, 12)),
            guestCount: 2,
            needsParking: false,
            isLateArrival: false,
            estimatedArrivalTime: null);

        Assert.Equal(BookingStatus.Pending, booking.Status);
    }

    [Fact]
    public void Create_Should_Require_Eta_For_Late_Arrival()
    {
        Assert.Throws<ArgumentException>(() =>
            Booking.Create(
                propertyId: 1,
                roomId: 2,
                dateRange: new DateRange(new DateTime(2026, 4, 10), new DateTime(2026, 4, 12)),
                guestCount: 2,
                needsParking: false,
                isLateArrival: true,
                estimatedArrivalTime: null));
    }

    [Fact]
    public void Confirm_After_Cancel_Should_Throw()
    {
        var booking = Booking.Create(
            propertyId: 1,
            roomId: 2,
            dateRange: new DateRange(new DateTime(2026, 4, 10), new DateTime(2026, 4, 12)),
            guestCount: 2,
            needsParking: false,
            isLateArrival: false,
            estimatedArrivalTime: null);

        booking.Cancel();

        Assert.Throws<InvalidOperationException>(() => booking.Confirm());
    }
}
