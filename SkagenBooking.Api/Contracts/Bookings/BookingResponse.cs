namespace SkagenBooking.Api.Contracts.Bookings;

public sealed class BookingResponse
{
    public int Id { get; init; }
    public int PropertyId { get; init; }
    public int RoomId { get; init; }
    public DateTime CheckInDate { get; init; }
    public DateTime CheckOutDate { get; init; }
    public int GuestCount { get; init; }
    public bool NeedsParking { get; init; }
    public string Status { get; init; } = string.Empty;
    public bool IsLateArrival { get; init; }
    public TimeOnly? EstimatedArrivalTime { get; init; }
}

