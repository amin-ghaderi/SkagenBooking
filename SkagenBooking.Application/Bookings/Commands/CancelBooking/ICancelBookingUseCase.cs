namespace SkagenBooking.Application.Bookings.Commands.CancelBooking;

public interface ICancelBookingUseCase
{
    Task<CancelBookingResult> ExecuteAsync(CancelBookingCommand command, CancellationToken cancellationToken);
}

