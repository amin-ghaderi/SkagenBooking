using SkagenBooking.Core.Enums;
using SkagenBooking.Core.ValueObjects;

namespace SkagenBooking.Core.Entities;

/// <summary>
/// Represents a booking for a specific room within a given date range.
/// </summary>
public class Booking
{
    /// <summary>
    /// Gets the unique identifier of the booking.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Gets the identifier of the booked room.
    /// </summary>
    public int RoomId { get; private set; }

    /// <summary>
    /// Gets the date range of the booking.
    /// </summary>
    public DateRange DateRange { get; private set; }

    /// <summary>
    /// Gets the current status of the booking.
    /// </summary>
    public BookingStatus Status { get; private set; }

    /// <summary>
    /// Initializes a new booking with a pending status.
    /// </summary>
    /// <param name="roomId">The room being booked.</param>
    /// <param name="dateRange">The booking date range.</param>
    public Booking(int roomId, DateRange dateRange)
    {
        RoomId = roomId;
        DateRange = dateRange;
        Status = BookingStatus.Pending;
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