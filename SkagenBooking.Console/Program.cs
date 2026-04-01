using SkagenBooking.Application.Bookings.Commands.CreateBooking;
using SkagenBooking.Application.Bookings.Queries.GetBookings;
using SkagenBooking.Application.Rooms.Queries.GetRooms;
using SkagenBooking.Console.Composition;
using SkagenBooking.Core.ValueObjects;

var (appService, cancellationToken) = ConsoleBootstrap.Build();

while (true)
{
    Console.Clear();
    Console.WriteLine("==================================================");
    Console.WriteLine("         Pernille's Bed & Breakfast - Skagen      ");
    Console.WriteLine("==================================================");
    Console.WriteLine("1) New booking");
    Console.WriteLine("2) List current bookings");
    Console.WriteLine("0) Exit");
    Console.Write("\nChoose an option: ");
    var menuInput = Console.ReadLine();

    if (menuInput == "0")
        break;

    if (menuInput == "2")
    {
        Console.Clear();
        Console.WriteLine("Current bookings");
        Console.WriteLine("----------------");
        var bookings = await appService.GetBookingsAsync(propertyId: null, cancellationToken);
        if (!bookings.Any())
        {
            Console.WriteLine("No bookings yet.");
        }
        else
        {
            foreach (var b in bookings)
            {
                Console.WriteLine($"Booking #{b.Id} | Room {b.RoomId} | Guests {b.GuestCount}");
                Console.WriteLine($"   {b.CheckIn:yyyy-MM-dd}  ->  {b.CheckOut:yyyy-MM-dd} | Parking: {(b.NeedsParking ? "Yes" : "No")}");
            }
        }

        Console.WriteLine("\nPress ENTER to return to main menu...");
        Console.ReadLine();
        continue;
    }

    if (menuInput != "1")
        continue;

    Console.Clear();
    Console.WriteLine("Available rooms (current property)");
    Console.WriteLine("----------------------------------");

    var propertyId = 1;
    var rooms = await appService.GetRoomsAsync(propertyId, cancellationToken);

    foreach (var r in rooms)
    {
        var description = r.Id switch
        {
            1 => "11m², single room, 1 single bed, flat-screen TV",
            2 => "14m², double room, 2 single beds, flat-screen TV",
            3 => "16m², double room, 1 double bed, flat-screen TV",
            4 => "24m², family room, 3 single beds, flat-screen TV",
            _ => "Room"
        };

        Console.WriteLine($"Room {r.Id} | Type: {r.Type} | Capacity: {r.Capacity}");
        Console.WriteLine($"   {description}");
    }

    Console.WriteLine("\nWiFi: free  |  Shared bathroom  |  Parking: 2 free spots (on availability)");

    int roomId;
    while (true)
    {
        Console.Write("\nEnter Room Id to book (or 0 to go back): ");
        var roomInput = Console.ReadLine();
        if (roomInput == "0")
            goto ContinueLoop;
        if (int.TryParse(roomInput, out roomId) && rooms.Any(r => r.Id == roomId))
            break;

        Console.WriteLine("Please enter a valid room id from the list.");
    }

    DateTime start;
    DateTime end;
    while (true)
    {
        Console.WriteLine("\nChoose check-in date:");
        Console.WriteLine("1) Today");
        Console.WriteLine("2) Tomorrow");
        Console.WriteLine("3) In 7 days");
        Console.WriteLine("4) Custom date (yyyy-mm-dd)");
        Console.Write("Select option (1-4, ENTER = Today): ");
        var option = Console.ReadLine();

        var today = DateTime.Today;
        if (string.IsNullOrWhiteSpace(option) || option == "1")
        {
            start = today;
        }
        else if (option == "2")
        {
            start = today.AddDays(1);
        }
        else if (option == "3")
        {
            start = today.AddDays(7);
        }
        else if (option == "4")
        {
            Console.Write($"Enter custom check-in date (yyyy-mm-dd) [min {today:yyyy-MM-dd}]: ");
            var startInput = Console.ReadLine();
            if (!DateTime.TryParse(startInput, out start))
            {
                Console.WriteLine("Invalid date format, please try again.");
                continue;
            }
        }
        else
        {
            Console.WriteLine("Please choose a valid option (1-4).");
            continue;
        }

        if (start.Date < today)
        {
            Console.WriteLine("Check-in date cannot be before today.");
            continue;
        }

        int nights;
        while (true)
        {
            Console.Write("How many nights? (1-30): ");
            var nightsInput = Console.ReadLine();
            if (int.TryParse(nightsInput, out nights) && nights is >= 1 and <= 30)
            {
                break;
            }

            Console.WriteLine("Please enter a number between 1 and 30.");
        }

        // normalize times to default check-in/check-out times
        start = start.Date.AddHours(14);   // 14:00 check-in
        end = start.Date.AddDays(nights).AddHours(11); // check-out 11:00 after N nights

        break;
    }

    int guestCount;
    while (true)
    {
        Console.Write("\nNumber of guests: ");
        var guestInput = Console.ReadLine();
        if (int.TryParse(guestInput, out guestCount) && guestCount > 0)
            break;

        Console.WriteLine("Please enter a positive number for guests.");
    }

    bool needsParking;
    while (true)
    {
        Console.Write("Need parking? (y/n): ");
        var parkingInput = Console.ReadLine()?.Trim().ToLower();
        if (parkingInput == "y")
        {
            needsParking = true;
            break;
        }
        if (parkingInput == "n")
        {
            needsParking = false;
            break;
        }
        Console.WriteLine("Please answer 'y' or 'n'.");
    }

    bool isLateArrival;
    TimeOnly? estimatedArrivalTime = null;
    while (true)
    {
        Console.Write("Arrival after 20:00? (y/n): ");
        var lateInput = Console.ReadLine()?.Trim().ToLower();
        if (lateInput == "y")
        {
            isLateArrival = true;
            while (true)
            {
                Console.Write("Estimated arrival time (HH:mm): ");
                var etaInput = Console.ReadLine();
                if (TimeOnly.TryParse(etaInput, out var eta))
                {
                    estimatedArrivalTime = eta;
                    break;
                }
                Console.WriteLine("Invalid time format, please try again.");
            }
            break;
        }
        if (lateInput == "n")
        {
            isLateArrival = false;
            break;
        }
        Console.WriteLine("Please answer 'y' or 'n'.");
    }

    Console.Clear();
    Console.WriteLine("Booking summary");
    Console.WriteLine("---------------");
    var selectedRoom = rooms.First(r => r.Id == roomId);
    Console.WriteLine($"Room: {selectedRoom.Id} ({selectedRoom.Type})");
    Console.WriteLine($"Guests: {guestCount}");
    Console.WriteLine($"Check-in:  {start:yyyy-MM-dd}");
    Console.WriteLine($"Check-out: {end:yyyy-MM-dd}");
    Console.WriteLine($"Needs parking: {(needsParking ? "Yes" : "No")}");
    if (isLateArrival)
    {
        Console.WriteLine($"Late arrival with ETA: {estimatedArrivalTime:HH\\:mm}");
    }

    // price preview before final confirmation
    var pricePreviewMoney = await appService.GetEstimatedPriceAsync(roomId, start, end, cancellationToken);
    if (pricePreviewMoney is not null)
    {
        Console.WriteLine($"Estimated price: {pricePreviewMoney.Value.Amount} {pricePreviewMoney.Value.Currency}");
    }

    Console.Write("\nConfirm booking? (y/n): ");
    var confirm = Console.ReadLine();
    if (confirm?.Trim().ToLower() != "y")
    {
        Console.WriteLine("Booking cancelled by user.");
        goto ContinueLoop;
    }

    var result = await appService.CreateBookingAsync(new CreateBookingCommand
    {
        PropertyId = propertyId,
        RoomId = roomId,
        CheckInDate = start,
        CheckOutDate = end,
        GuestCount = guestCount,
        NeedsParking = needsParking,
        IsLateArrival = isLateArrival,
        EstimatedArrivalTime = estimatedArrivalTime
    }, cancellationToken);

    Console.WriteLine();
    if (!result.IsCreated)
    {
        Console.WriteLine($"❌ Booking failed: {result.Message}");
    }
    else
    {
        Console.WriteLine("✅ Booking successful.");
        Console.WriteLine($"💰 Total price: {result.TotalAmount} {result.Currency}");
    }

    Console.WriteLine("\nPress ENTER to return to main menu...");
    Console.ReadLine();

ContinueLoop:
    ;
}