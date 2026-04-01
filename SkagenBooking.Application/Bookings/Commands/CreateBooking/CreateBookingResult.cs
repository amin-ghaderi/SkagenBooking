namespace SkagenBooking.Application.Bookings.Commands.CreateBooking;

public sealed class CreateBookingResult
{
    public bool IsCreated { get; init; }
    public int? BookingId { get; init; }
    public decimal? TotalAmount { get; init; }
    public string Currency { get; init; } = "DKK";
    public string? Message { get; init; }
}
