namespace SkagenBooking.Api.Contracts.Bookings;

public sealed class BookingResponse
{
    public int Id { get; init; }
    public int PropertyId { get; init; }
    public int RoomId { get; init; }
    public DateTime CheckIn { get; init; }
    public DateTime CheckOut { get; init; }
    public int GuestCount { get; init; }
    public bool NeedsParking { get; init; }
}

