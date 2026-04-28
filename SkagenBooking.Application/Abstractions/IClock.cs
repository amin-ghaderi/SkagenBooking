namespace SkagenBooking.Application.Abstractions;

public interface IClock
{
    DateTime Today { get; }
}

