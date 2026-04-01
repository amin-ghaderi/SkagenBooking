using SkagenBooking.Core.Entities;
using SkagenBooking.Core.ValueObjects;

namespace SkagenBooking.Core.Services;

/// <summary>
/// Provides logic to determine room availability based on existing bookings.
/// </summary>
public class AvailabilityService : IAvailabilityService
{
    /// <summary>
    /// Checks if the given room is available for the requested date range.
    /// A room is considered unavailable if any existing booking overlaps
    /// with the requested date range.
    /// </summary>
    public bool IsRoomAvailable(
        Room room,
        DateRange requestedRange,
        List<Booking> existingBookings
    )
    {
        foreach (var booking in existingBookings)
        {
            if (booking.RoomId != room.Id)
                continue;

            if (booking.DateRange.Overlaps(requestedRange))
                return false;
        }

        return true;
    }
}