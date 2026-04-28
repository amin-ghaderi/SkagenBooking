using SkagenBooking.Application.Abstractions;

namespace SkagenBooking.Infrastructure.Time;

public sealed class SystemClock : IClock
{
    public DateTime Today => DateTime.Today;
}

