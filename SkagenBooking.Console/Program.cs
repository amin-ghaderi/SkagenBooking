using SkagenBooking.Core.Services;
using SkagenBooking.Core.ValueObjects;
using SkagenBooking.Infrastructure.Repositories;

// repositories
var roomRepo = new InMemoryRoomRepository();
var bookingRepo = new InMemoryBookingRepository();

// services
var availabilityService = new AvailabilityService();
var pricingService = new BasicPricingService();
var bookingService = new BookingService(
    roomRepo,
    bookingRepo,
    availabilityService,
    pricingService
);

while (true)
{
    Console.WriteLine("\n=== Skagen Booking System ===");

    // 1. show rooms
    var rooms = roomRepo.GetAll();

    Console.WriteLine("\nAvailable Rooms:");
    foreach (var r in rooms)
    {
        Console.WriteLine($"Room {r.Id} | Type: {r.Type} | Capacity: {r.Capacity}");
    }

    // 2. select room
    Console.Write("\nEnter Room Id (or 0 to exit): ");
    var roomInput = Console.ReadLine();

    if (roomInput == "0")
        break;

    if (!int.TryParse(roomInput, out var roomId))
    {
        Console.WriteLine("Invalid input!");
        continue;
    }

    // 3. enter dates
    Console.Write("Start date (yyyy-mm-dd): ");
    var startInput = Console.ReadLine();

    Console.Write("End date (yyyy-mm-dd): ");
    var endInput = Console.ReadLine();

    if (!DateTime.TryParse(startInput, out var start) ||
        !DateTime.TryParse(endInput, out var end))
    {
        Console.WriteLine("Invalid date format!");
        continue;
    }

    var range = new DateRange(start, end);

    // 4. try booking
    // get room
    var room = roomRepo.GetById(roomId);
    if (room == null)
    {
        Console.WriteLine("Room not found!");
        continue;
    }

    // get bookings
    var bookings = bookingRepo.GetAll()
        .Where(b => b.RoomId == roomId)
        .ToList();

    // check availability
    var isAvailable = availabilityService.IsRoomAvailable(room, range, bookings);

    if (!isAvailable)
    {
        Console.WriteLine("\n❌ Room is not available for selected dates");
        continue;
    }

    // calculate price BEFORE booking
    var price = pricingService.CalculatePrice(room, range);

    Console.WriteLine($"\n💰 Price for this stay: {price.Amount} {price.Currency}");

    // confirmation
    Console.Write("Do you want to book? (y/n): ");
    var confirm = Console.ReadLine();

    if (confirm?.ToLower() != "y")
    {
        Console.WriteLine("Booking cancelled.");
        continue;
    }

    // create booking via service
    var success = bookingService.CreateBooking(roomId, range, out _);

    if (success)
    {
        Console.WriteLine("\n✅ Booking successful!");
    }
    else
    {
        Console.WriteLine("\n❌ Booking failed unexpectedly");
    }

    if (success)
    {
        Console.WriteLine("\n✅ Booking successful!");
        Console.WriteLine($"💰 Total price: {price!.Amount} {price.Currency}");
    }
    else
    {
        Console.WriteLine("\n❌ Room is not available or not found");
    }
}