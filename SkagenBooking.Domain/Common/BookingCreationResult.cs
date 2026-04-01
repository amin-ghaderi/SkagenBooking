using SkagenBooking.Core.Entities;

namespace SkagenBooking.Core.Common;

/// <summary>
/// Result type for creating a booking aggregate.
/// </summary>
public sealed class BookingCreationResult
{
    private BookingCreationResult(bool isSuccess, Booking? booking, string? error)
    {
        IsSuccess = isSuccess;
        Booking = booking;
        Error = error;
    }

    public bool IsSuccess { get; }

    public Booking? Booking { get; }

    public string? Error { get; }

    public static BookingCreationResult Success(Booking booking) =>
        new(true, booking, null);

    public static BookingCreationResult Failure(string error) =>
        new(false, null, error);
}

