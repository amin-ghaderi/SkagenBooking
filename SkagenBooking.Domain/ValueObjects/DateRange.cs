namespace SkagenBooking.Core.ValueObjects;

/// <summary>
/// Represents a date range for a booking.
/// The check-out date is considered exclusive,
/// meaning the room becomes available on that day.
/// </summary>
public class DateRange
{
    /// <summary>
    /// Gets the check-in date.
    /// </summary>
    public DateTime CheckIn { get; }

    /// <summary>
    /// Gets the check-out date (exclusive).
    /// </summary>
    public DateTime CheckOut { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DateRange"/> class.
    /// </summary>
    /// <param name="checkIn">The check-in date.</param>
    /// <param name="checkOut">The check-out date (must be after check-in).</param>
    /// <exception cref="ArgumentException">
    /// Thrown when check-out is not after check-in.
    /// </exception>
    public DateRange(DateTime checkIn, DateTime checkOut)
    {
        if (checkOut <= checkIn)
            throw new ArgumentException("Check-out must be after check-in.");

        CheckIn = checkIn;
        CheckOut = checkOut;
    }

    /// <summary>
    /// Calculates the total number of nights in the date range.
    /// </summary>
    /// <returns>The number of days between check-in and check-out.</returns>
    public int GetTotalDays()
    {
        return (CheckOut.Date - CheckIn.Date).Days;
    }

    /// <summary>
    /// Determines whether this date range overlaps with another date range.
    /// Two ranges overlap if they share at least one common day.
    /// </summary>
    /// <param name="other">Another date range to compare with.</param>
    /// <returns>
    /// <c>true</c> if the date ranges overlap; otherwise, <c>false</c>.
    /// </returns>
    public bool Overlaps(DateRange other)
    {
        return CheckIn < other.CheckOut && CheckOut > other.CheckIn;
    }
}