using SkagenBooking.Application.Abstractions;
using SkagenBooking.Core.Interfaces;

namespace SkagenBooking.Application.Bookings.Commands.CancelBooking;

public sealed class CancelBookingUseCase : ICancelBookingUseCase
{
    private readonly IBookingAggregateRepository _bookingRepository;
    private readonly IParkingAllocationRepository _parkingAllocationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelBookingUseCase(
        IBookingAggregateRepository bookingRepository,
        IParkingAllocationRepository parkingAllocationRepository,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _parkingAllocationRepository = parkingAllocationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CancelBookingResult> ExecuteAsync(CancelBookingCommand command, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(command.BookingId, cancellationToken);
        if (booking is null)
        {
            return new CancelBookingResult { IsSuccess = false, Error = CancelBookingError.NotFound, Message = "Booking not found." };
        }

        if (booking.Status != SkagenBooking.Core.Enums.BookingStatus.Cancelled)
        {
            booking.Cancel();
        }

        var allocation = await _parkingAllocationRepository.GetByBookingIdAsync(booking.Id, cancellationToken);
        if (allocation is not null)
        {
            await _parkingAllocationRepository.RemoveAsync(allocation, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelBookingResult { IsSuccess = true, Error = CancelBookingError.None };
    }
}

