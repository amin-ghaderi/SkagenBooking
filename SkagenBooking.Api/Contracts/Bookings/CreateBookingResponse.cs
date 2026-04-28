namespace SkagenBooking.Api.Contracts.Bookings;

public sealed class CreateBookingResponse
{
    public int BookingId { get; init; }
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; } = "DKK";
    public string? Message { get; init; }
}

