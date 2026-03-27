using SkagenBooking.Core.Entities;

namespace SkagenBooking.Core.Interfaces;

/// <summary>
/// Defines booking data access operations.
/// </summary>
public interface IBookingRepository
{
    /// <summary>
    /// Adds a new booking.
    /// </summary>
    void Add(Booking booking);

    /// <summary>
    /// Gets all bookings.
    /// </summary>
    List<Booking> GetAll();
}