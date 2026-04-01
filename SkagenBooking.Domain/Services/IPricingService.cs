using SkagenBooking.Core.Entities;
using SkagenBooking.Core.ValueObjects;

namespace SkagenBooking.Core.Services;

/// <summary>
/// Defines pricing calculation logic.
/// </summary>
public interface IPricingService
{
    Money CalculatePrice(Room room, DateRange range);
}