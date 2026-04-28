using SkagenBooking.Application.Abstractions;

namespace SkagenBooking.Application.Bookings.Commands.CancelBooking;

public sealed class CancelBookingCommand : ICommand<CancelBookingResult>
{
    public int BookingId { get; init; }
}

