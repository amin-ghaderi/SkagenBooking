namespace SkagenBooking.Core.ValueObjects;

/// <summary>
/// Value object representing the date range for a booking.
/// The check-out date is exclusive, meaning the room becomes available on that day.
/// </summary>
public class DateRange
{
    public DateTime CheckIn { get; }

    public DateTime CheckOut { get; }

    public DateRange(DateTime checkIn, DateTime checkOut)
    {
        if (checkOut <= checkIn)
            throw new ArgumentException("Check-out must be after check-in.");

        CheckIn = checkIn;
        CheckOut = checkOut;
    }

    public int GetTotalDays()
    {
        return (CheckOut.Date - CheckIn.Date).Days;
    }

    public bool Overlaps(DateRange other)
    {
        return CheckIn < other.CheckOut && CheckOut > other.CheckIn;
    }
}