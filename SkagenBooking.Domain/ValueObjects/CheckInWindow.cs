namespace SkagenBooking.Core.ValueObjects;

public readonly record struct CheckInWindow(TimeOnly Start, TimeOnly End)
{
    public bool Contains(TimeOnly time) => time >= Start && time <= End;
}
