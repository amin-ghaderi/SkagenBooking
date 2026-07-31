using SkagenBooking.Application.Abstractions;

namespace SkagenBooking.Application.Bookings.Commands.UpdateBooking;

/// <summary>
/// Command for updating a booking owned by the authenticated user.
/// </summary>
public sealed class UpdateBookingCommand : ICommand<UpdateBookingResult>
{
    public required string UserId { get; init; }
    public int BookingId { get; init; }
    public DateTime CheckInDate { get; init; }
    public DateTime CheckOutDate { get; init; }
    public int GuestCount { get; init; }
    public bool NeedsParking { get; init; }
    public bool IsLateArrival { get; init; }
    public TimeOnly? EstimatedArrivalTime { get; init; }
}

