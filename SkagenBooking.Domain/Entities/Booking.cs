using SkagenBooking.Core.Enums;
using SkagenBooking.Core.Events;
using SkagenBooking.Core.ValueObjects;
using SkagenBooking.Core.Common;

namespace SkagenBooking.Core.Entities;

/// <summary>
/// Represents a booking for a specific room within a given date range.
/// </summary>
public class Booking : AggregateRoot
{
    /// <summary>
    /// Gets the unique identifier of the booking.
    /// </summary>
    public int Id { get; private set; }
    public int PropertyId { get; private set; }

    /// <summary>
    /// Gets the identifier of the booked room.
    /// </summary>
    public int RoomId { get; private set; }

    /// <summary>
    /// Gets the date range of the booking.
    /// </summary>
    public DateRange DateRange { get; private set; }
    public int GuestCount { get; private set; }
    public bool NeedsParking { get; private set; }
    public TimeOnly? EstimatedArrivalTime { get; private set; }

    /// <summary>
    /// Gets the current status of the booking.
    /// </summary>
    public BookingStatus Status { get; private set; }

    /// <summary>
    /// Initializes a new booking with a pending status.
    /// </summary>
    /// <param name="roomId">The room being booked.</param>
    /// <param name="dateRange">The booking date range.</param>
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