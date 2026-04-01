namespace SkagenBooking.Core.Enums;

/// <summary>
/// Represents the current status of a booking.
/// </summary>
public enum BookingStatus
{
    /// <summary>
    /// Booking is created but not yet confirmed.
    /// </summary>
    Pending,

    /// <summary>
    /// Booking has been confirmed.
    /// </summary>
    Confirmed,

    /// <summary>
    /// Booking has been cancelled.
    /// </summary>
    Cancelled
}