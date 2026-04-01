namespace SkagenBooking.Application.Bookings.Commands.CreateBooking;

public interface ICreateBookingUseCase
{
    Task<CreateBookingResult> ExecuteAsync(CreateBookingCommand command, CancellationToken cancellationToken);
}
