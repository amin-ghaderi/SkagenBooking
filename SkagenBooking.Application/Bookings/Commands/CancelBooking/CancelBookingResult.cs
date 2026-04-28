namespace SkagenBooking.Application.Bookings.Commands.CancelBooking;

public enum CancelBookingError
{
    None = 0,
    NotFound = 1
}

public sealed class CancelBookingResult
{
    public bool IsSuccess { get; init; }
    public CancelBookingError Error { get; init; }
    public string? Message { get; init; }
}

