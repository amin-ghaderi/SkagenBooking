using System.Linq;
using SkagenBooking.Core.Entities;
using SkagenBooking.Core.Interfaces;
using SkagenBooking.Core.ValueObjects;

namespace SkagenBooking.Core.Services;

/// <summary>
/// Handles booking creation process.
/// </summary>
public class BookingService
{
    private readonly IRoomRepository _roomRepo;
    private readonly IBookingRepository _bookingRepo;
    private readonly IAvailabilityService _availabilityService;
    private readonly IPricingService _pricingService;

    public BookingService(
        IRoomRepository roomRepo,
        IBookingRepository bookingRepo,
        IAvailabilityService availabilityService,
        IPricingService pricingService)
    {
        _roomRepo = roomRepo;
        _bookingRepo = bookingRepo;
        _availabilityService = availabilityService;
        _pricingService = pricingService;
    }

    /// <summary>
    /// Creates a booking if the room is available.
    /// </summary>
    public bool CreateBooking(int roomId, DateRange range, out Money? price)
    {
        price = null;

        // 1. Get room
        var room = _roomRepo.GetById(roomId);
        if (room == null)
            return false;

        // 2. Get bookings for this room
        var bookings = _bookingRepo.GetAll()
            .Where(b => b.RoomId == roomId)
            .ToList();

        // 3. Check availability
        var isAvailable = _availabilityService.IsRoomAvailable(room, range, bookings);
        if (!isAvailable)
            return false;

        // 4. Calculate price
        price = _pricingService.CalculatePrice(room, range);

        // 5. Create booking
        var booking = new Booking(room.Id, range);

        // 6. Save booking
        _bookingRepo.Add(booking);

        return true;
    }
}