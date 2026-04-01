using SkagenBooking.Core.Entities;
using SkagenBooking.Core.ValueObjects;

namespace SkagenBooking.Core.Services;

/// <summary>
/// Defines operations for checking room availability.
/// </summary>
public interface IAvailabilityService
{
    /// <summary>
    /// Determines whether a room is available for a given date range.
    /// </summary>
    /// <param name="room">The room to check.</param>
    /// <param name="requestedRange">The requested booking date range.</param>
    /// <param name="existingBookings">Existing bookings for the room.</param>
    /// <returns>
    /// <c>true</c> if the room is available; otherwise, <c>false</c>.
    /// </returns>
    bool IsRoomAvailable(
        Room room,
        DateRange requestedRange,
        List<Booking> existingBookings
    );
}