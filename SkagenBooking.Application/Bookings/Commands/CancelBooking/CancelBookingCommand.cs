using SkagenBooking.Application.Abstractions;

namespace SkagenBooking.Application.Bookings.Commands.CancelBooking;

/// <summary>
/// Command for cancelling a booking owned by the authenticated user.
/// </summary>
public sealed class CancelBookingCommand : ICommand<CancelBookingResult>
{
    public required string UserId { get; init; }
    public int BookingId { get; init; }
}

