namespace SkagenBooking.Core.Policies;

/// <summary>
/// Domain policy that defines time-window rules for check-in, check-out and late-arrival handling.
/// </summary>
public sealed class BookingWindowPolicy
{
    public TimeOnly CheckInStart { get; } = new(14, 0);
    public TimeOnly CheckInEnd { get; } = new(22, 30);
    public TimeOnly CheckOutDeadline { get; } = new(12, 0);
    public TimeOnly LateArrivalThreshold { get; } = new(20, 0);

    public bool IsValidCheckIn(TimeOnly checkInTime) => checkInTime >= CheckInStart && checkInTime <= CheckInEnd;
    public bool IsValidCheckOut(TimeOnly checkOutTime) => checkOutTime <= CheckOutDeadline;
}

