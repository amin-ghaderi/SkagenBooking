using SkagenBooking.Application.Abstractions;

namespace SkagenBooking.Application.Bookings.Commands.CreateBooking;

public sealed class CreateBookingCommand : ICommand<CreateBookingResult>
{
    public int PropertyId { get; init; }
    public int RoomId { get; init; }
    public DateTime CheckInDate { get; init; }
    public DateTime CheckOutDate { get; init; }
    public int GuestCount { get; init; }
    public bool NeedsParking { get; init; }
    public bool IsLateArrival { get; init; }
    public TimeOnly? EstimatedArrivalTime { get; init; }
}
