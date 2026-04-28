using SkagenBooking.Application.Abstractions;

namespace SkagenBooking.Tests.Fakes;

public sealed class FakeClock : IClock
{
    public FakeClock(DateTime today)
    {
        Today = today;
    }

    public DateTime Today { get; }
}

