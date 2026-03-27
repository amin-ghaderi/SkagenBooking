using SkagenBooking.Core.Entities;
using SkagenBooking.Core.ValueObjects;

namespace SkagenBooking.Core.Services;

/// <summary>
/// Provides pricing logic based on room and date range.
/// </summary>
public class BasicPricingService : IPricingService
{
    private readonly Dictionary<int, decimal> _prices = new()
    {
        { 1, 550 },
        { 2, 700 },
        { 3, 765 },
        { 4, 850 }
    };

    /// <summary>
    /// Calculates total price for a booking.
    /// </summary>
    public Money CalculatePrice(Room room, DateRange range)
    {
        if (!_prices.ContainsKey(room.Id))
            throw new Exception("Price not defined for this room.");

        var pricePerDay = _prices[room.Id];
        var days = range.GetTotalDays();

        var total = pricePerDay * days;

        return new Money(total, "DKK");
    }
}