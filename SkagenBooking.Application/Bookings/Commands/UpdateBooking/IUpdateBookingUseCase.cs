namespace SkagenBooking.Application.Bookings.Commands.UpdateBooking;

public interface IUpdateBookingUseCase
{
    Task<UpdateBookingResult> ExecuteAsync(UpdateBookingCommand command, CancellationToken cancellationToken);
}

