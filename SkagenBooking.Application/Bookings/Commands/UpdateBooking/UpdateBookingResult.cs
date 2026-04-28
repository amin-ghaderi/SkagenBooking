namespace SkagenBooking.Application.Bookings.Commands.UpdateBooking;

public enum UpdateBookingError
{
    None = 0,
    NotFound = 1,
    Conflict = 2,
    Validation = 3
}

public sealed class UpdateBookingResult
{
    public bool IsSuccess { get; init; }
    public UpdateBookingError Error { get; init; }
    public string? Message { get; init; }
}

