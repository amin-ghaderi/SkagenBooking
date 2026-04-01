using SkagenBooking.Core.Entities;
using SkagenBooking.Core.ValueObjects;

namespace SkagenBooking.Core.Services;

/// <summary>
/// Provides pricing logic based on room and date range.
/// </summary>
public class BasicPricingService : IPricingService
{
    /// <summary>
    /// Calculates total price for a booking.
    /// </summary>
    public Money CalculatePrice(Room room, DateRange range)
    {
        var days = range.GetTotalDays();
        return room.NightlyRate * days;
    }
}