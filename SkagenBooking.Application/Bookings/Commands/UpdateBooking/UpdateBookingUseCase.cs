using SkagenBooking.Application.Abstractions;
using SkagenBooking.Core.Entities;
using SkagenBooking.Core.Interfaces;
using SkagenBooking.Core.Policies;
using SkagenBooking.Core.Services;
using SkagenBooking.Core.ValueObjects;

namespace SkagenBooking.Application.Bookings.Commands.UpdateBooking;

public sealed class UpdateBookingUseCase : IUpdateBookingUseCase
{
    private readonly IBookingAggregateRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IParkingAllocationRepository _parkingAllocationRepository;
    private readonly IAvailabilityService _availabilityService;
    private readonly IParkingAvailabilityService _parkingAvailabilityService;
    private readonly BookingWindowPolicy _bookingWindowPolicy;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public UpdateBookingUseCase(
        IBookingAggregateRepository bookingRepository,
        IRoomRepository roomRepository,
        IPropertyRepository propertyRepository,
        IParkingAllocationRepository parkingAllocationRepository,
        IAvailabilityService availabilityService,
        IParkingAvailabilityService parkingAvailabilityService,
        BookingWindowPolicy bookingWindowPolicy,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _propertyRepository = propertyRepository;
        _parkingAllocationRepository = parkingAllocationRepository;
        _availabilityService = availabilityService;
        _parkingAvailabilityService = parkingAvailabilityService;
        _bookingWindowPolicy = bookingWindowPolicy;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<UpdateBookingResult> ExecuteAsync(UpdateBookingCommand command, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(command.BookingId, cancellationToken);
        if (booking is null)
        {
            return new UpdateBookingResult { IsSuccess = false, Error = UpdateBookingError.NotFound, Message = "Booking not found." };
        }

        var room = await _roomRepository.GetByIdAsync(booking.RoomId, cancellationToken);
        if (room is null)
        {
            return new UpdateBookingResult { IsSuccess = false, Error = UpdateBookingError.NotFound, Message = "Room not found." };
        }

        var property = await _propertyRepository.GetByIdAsync(booking.PropertyId, cancellationToken);
        if (property is null)
        {
            return new UpdateBookingResult { IsSuccess = false, Error = UpdateBookingError.NotFound, Message = "Property not found." };
        }

        var range = new DateRange(command.CheckInDate, command.CheckOutDate);

        var existingBookings = await _bookingRepository.GetByRoomAsync(room.Id, cancellationToken);
        var overlappingCandidates = existingBookings.Where(x => x.Id != booking.Id).ToList();
        var isAvailable = _availabilityService.IsRoomAvailable(room, range, overlappingCandidates);
        if (!isAvailable)
        {
            return new UpdateBookingResult { IsSuccess = false, Error = UpdateBookingError.Conflict, Message = "Room is not available for selected dates." };
        }

        var existingAllocations = await _parkingAllocationRepository.GetByPropertyAsync(booking.PropertyId, cancellationToken);
        var allocationForThisBooking = existingAllocations.FirstOrDefault(x => x.BookingId == booking.Id);
        var competingAllocations = existingAllocations.Where(x => x.BookingId != booking.Id).ToList();

        if (command.NeedsParking)
        {
            var hasParking = _parkingAvailabilityService.HasFreeSlot(competingAllocations, range, property.ParkingCapacity);
            if (!hasParking)
            {
                return new UpdateBookingResult { IsSuccess = false, Error = UpdateBookingError.Conflict, Message = "No parking slot available for selected dates." };
            }
        }

        var updateResult = booking.UpdateDetails(
            room,
            range,
            command.GuestCount,
            command.NeedsParking,
            command.IsLateArrival,
            command.EstimatedArrivalTime,
            _bookingWindowPolicy,
            _clock.Today);

        if (!updateResult.IsSuccess)
        {
            return new UpdateBookingResult { IsSuccess = false, Error = UpdateBookingError.Validation, Message = updateResult.Error };
        }

        if (command.NeedsParking)
        {
            if (allocationForThisBooking is null)
            {
                await _parkingAllocationRepository.AddAsync(ParkingAllocation.CreateFromBooking(booking), cancellationToken);
            }
            else
            {
                await _parkingAllocationRepository.RemoveAsync(allocationForThisBooking, cancellationToken);
                await _parkingAllocationRepository.AddAsync(ParkingAllocation.CreateFromBooking(booking), cancellationToken);
            }
        }
        else if (allocationForThisBooking is not null)
        {
            await _parkingAllocationRepository.RemoveAsync(allocationForThisBooking, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateBookingResult { IsSuccess = true, Error = UpdateBookingError.None };
    }
}

