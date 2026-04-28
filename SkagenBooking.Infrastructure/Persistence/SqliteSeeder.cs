using Microsoft.EntityFrameworkCore;
using SkagenBooking.Core.Entities;
using SkagenBooking.Core.Enums;
using SkagenBooking.Core.ValueObjects;

namespace SkagenBooking.Infrastructure.Persistence;

public static class SqliteSeeder
{
    public static async Task SeedAsync(SkagenBookingDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Properties.AnyAsync(cancellationToken))
        {
            dbContext.Properties.Add(new Property(1, "Pernille's Bed & Breakfast", "Skagen", 2));
        }

        if (!await dbContext.Rooms.AnyAsync(cancellationToken))
        {
            dbContext.Rooms.AddRange(
                new Room(1, 1, RoomType.Single, 1, new Money(550m, "DKK")),
                new Room(2, 1, RoomType.Double, 2, new Money(700m, "DKK")),
                new Room(3, 1, RoomType.Double, 2, new Money(765m, "DKK")),
                new Room(4, 1, RoomType.Family, 3, new Money(850m, "DKK")));
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var maxBookingId = await dbContext.Bookings.Select(x => (int?)x.Id).MaxAsync(cancellationToken) ?? 0;
        Booking.InitializeNextId(maxBookingId);
    }
}

