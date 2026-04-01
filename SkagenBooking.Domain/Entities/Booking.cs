using SkagenBooking.Core.Enums;
using SkagenBooking.Core.Events;
using SkagenBooking.Core.ValueObjects;
using SkagenBooking.Core.Common;

namespace SkagenBooking.Core.Entities;

/// <summary>
/// Aggregate root representing a room booking for a property over a date range.
/// </summary>
public class Booking : AggregateRoot
{
    public int Id { get; private set; }
    public int PropertyId { get; private set; }

    public int RoomId { get; private set; }

    public DateRange DateRange { get; private set; }
    public int GuestCount { get; private set; }
    public bool NeedsParking { get; private set; }
    public TimeOnly? EstimatedArrivalTime { get; private set; }

    public BookingStatus Status { get; private set; }

    private Booking(int propertyId, int roomId, DateRange dateRange, int guestCount, bool needsParking, TimeOnly? estimatedArrivalTime)
    {
        PropertyId = propertyId;
        RoomId = roomId;
        DateRange = dateRange;
        GuestCount = guestCount;
        NeedsParking = needsParking;
        EstimatedArrivalTime = estimatedArrivalTime;
        Status = BookingStatus.Pending;
    }

    /// <summary>
    /// Factory method that creates a new booking while enforcing domain invariants.
    /// </summary>
    /// <param name="propertyId">Identifier of the property that owns the room.</param>
    /// <param name="roomId">Identifier of the booked room.</param>
    /// <param name="dateRange">Date range of the stay (check-in/check-out).</param>
    /// <param name="guestCount">Number of guests included in the booking.</param>
    /// <param name="needsParking">Whether the booking should reserve parking capacity.</param>
    /// <param name="isLateArrival">Whether the guest will arrive after the late-arrival threshold.</param>
    /// <param name="estimatedArrivalTime">Estimated time of arrival when arriving late.</param>
    /// <returns>The newly created booking aggregate.</returns>
    public static Booking Create(
        int propertyId,
        int roomId,
        DateRange dateRange,
        int guestCount,
        bool needsParking,
        bool isLateArrival,
        TimeOnly? estimatedArrivalTime)
    {
        if (guestCount <= 0)
            throw new ArgumentException("Guest count must be greater than zero.");

        // Business rule from project statement: late arrivals after 20:00 should provide ETA.
        if (isLateArrival && estimatedArrivalTime is null)
            throw new ArgumentException("Estimated arrival time is required for arrivals after 20:00.");

        var booking = new Booking(propertyId, roomId, dateRange, guestCount, needsParking, estimatedArrivalTime);
        booking.Raise(new BookingCreatedDomainEvent(propertyId, roomId));
        return booking;
    }

    /// <summary>
    /// Confirms the booking.
    /// </summary>
    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Only pending bookings can be confirmed.");

        Status = BookingStatus.Confirmed;
    }

    /// <summary>
    /// Cancels the booking.
    /// </summary>
    public void Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Booking is already cancelled.");

        Status = BookingStatus.Cancelled;
    }
}