using SkagenBooking.Core.Common;
using SkagenBooking.Core.ValueObjects;

namespace SkagenBooking.Core.Entities;

/// <summary>
/// Aggregate root representing a parking allocation for a specific booking.
/// </summary>
public class ParkingAllocation : AggregateRoot
{
    public int Id { get; private set; }

    /// <summary>
    /// Identifier of the property where the parking slot is located.
    /// </summary>
    public int PropertyId { get; private set; }

    /// <summary>
    /// Identifier of the booking that owns this parking allocation.
    /// </summary>
    public int BookingId { get; private set; }

    /// <summary>
    /// Date range during which the parking allocation is active.
    /// </summary>
    public DateRange DateRange { get; private set; }

    private ParkingAllocation(int propertyId, int bookingId, DateRange dateRange)
    {
        PropertyId = propertyId;
        BookingId = bookingId;
        DateRange = dateRange;
    }

    /// <summary>
    /// Creates a parking allocation that is fully tied to an existing booking.
    /// </summary>
    public static ParkingAllocation CreateFromBooking(Booking booking)
    {
        if (booking is null) throw new ArgumentNullException(nameof(booking));

        return new ParkingAllocation(
            booking.PropertyId,
            booking.Id,
            booking.DateRange);
    }
}

