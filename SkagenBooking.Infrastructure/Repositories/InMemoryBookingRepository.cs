using SkagenBooking.Core.Entities;
using SkagenBooking.Core.Interfaces;

namespace SkagenBooking.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of booking repository.
/// </summary>
public class InMemoryBookingRepository : IBookingRepository
{
    private readonly List<Booking> _bookings = new();

    public void Add(Booking booking)
    {
        _bookings.Add(booking);
    }

    public List<Booking> GetAll()
    {
        return _bookings;
    }
}