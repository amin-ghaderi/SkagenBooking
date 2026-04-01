using SkagenBooking.Application.Common.DomainEvents;
using SkagenBooking.Application.Abstractions;
using SkagenBooking.Core.Common;
using SkagenBooking.Core.Entities;
using SkagenBooking.Core.Interfaces;
using SkagenBooking.Core.Policies;
using SkagenBooking.Core.Services;
using SkagenBooking.Core.ValueObjects;

namespace SkagenBooking.Application.Bookings.Commands.CreateBooking;

/// <summary>
/// Application use case that orchestrates booking creation while delegating
/// business validation to the domain layer.
/// </summary>
public sealed class CreateBookingUseCase : ICreateBookingUseCase
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingAggregateRepository _bookingRepository;
    private readonly IParkingAllocationRepository _parkingAllocationRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IPricingService _pricingService;
    private readonly BookingWindowPolicy _bookingWindowPolicy;
    private readonly IAvailabilityService _availabilityService;
    private readonly IParkingAvailabilityService _parkingAvailabilityService;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly IOutbox _outbox;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookingUseCase(
        IRoomRepository roomRepository,
        IBookingAggregateRepository bookingRepository,
        IParkingAllocationRepository parkingAllocationRepository,
        IPropertyRepository propertyRepository,
        IPricingService pricingService,
        BookingWindowPolicy bookingWindowPolicy,
        IAvailabilityService availabilityService,
        IParkingAvailabilityService parkingAvailabilityService,
        IDomainEventDispatcher domainEventDispatcher,
        IOutbox outbox,
        IUnitOfWork unitOfWork)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
        _parkingAllocationRepository = parkingAllocationRepository;
        _propertyRepository = propertyRepository;
        _pricingService = pricingService;
        _bookingWindowPolicy = bookingWindowPolicy;
        _availabilityService = availabilityService;
        _parkingAvailabilityService = parkingAvailabilityService;
        _domainEventDispatcher = domainEventDispatcher;
        _outbox = outbox;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateBookingResult> ExecuteAsync(CreateBookingCommand command, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(command.RoomId, cancellationToken);
        if (room is null || room.PropertyId != command.PropertyId)
        {
            return new CreateBookingResult { IsCreated = false, Message = "Room not found for selected property." };
        }

        var property = await _propertyRepository.GetByIdAsync(command.PropertyId, cancellationToken);
        if (property is null)
        {
            return new CreateBookingResult { IsCreated = false, Message = "Property not found." };
        }

        var range = new DateRange(command.CheckInDate, command.CheckOutDate);

        var existingBookings = await _bookingRepository.GetByRoomAsync(room.Id, cancellationToken);
        var isAvailable = _availabilityService.IsRoomAvailable(room, range, existingBookings.ToList());
        if (!isAvailable)
        {
            return new CreateBookingResult { IsCreated = false, Message = "Room is not available for selected dates." };
        }

        if (command.NeedsParking)
        {
            var existingAllocations = await _parkingAllocationRepository
                .GetByPropertyAsync(command.PropertyId, cancellationToken);

            var hasParking = _parkingAvailabilityService.HasFreeSlot(
                existingAllocations,
                range,
                property.ParkingCapacity);

            if (!hasParking)
            {
                return new CreateBookingResult
                {
                    IsCreated = false,
                    Message = "No parking slot available for selected dates."
                };
            }
        }

        var creationResult = Booking.TryCreate(
            room,
            range,
            command.GuestCount,
            command.NeedsParking,
            command.IsLateArrival,
            command.EstimatedArrivalTime,
            _bookingWindowPolicy,
            DateTime.Today);

        if (!creationResult.IsSuccess)
        {
            return new CreateBookingResult
            {
                IsCreated = false,
                Message = creationResult.Error
            };
        }

        var booking = creationResult.Booking!;

        await _bookingRepository.AddAsync(booking, cancellationToken);

        if (command.NeedsParking)
        {
            var allocation = ParkingAllocation.CreateFromBooking(booking);
            await _parkingAllocationRepository.AddAsync(allocation, cancellationToken);
        }
        await _outbox.EnqueueAsync(booking.DomainEvents, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _domainEventDispatcher.DispatchAsync(booking.DomainEvents, cancellationToken);
        booking.ClearDomainEvents();

        var total = _pricingService.CalculatePrice(room, range);

        return new CreateBookingResult
        {
            IsCreated = true,
            BookingId = booking.Id,
            TotalAmount = total.Amount,
            Currency = total.Currency,
            Message = "Booking created."
        };
    }
}
