using SkagenBooking.Application.Common.DomainEvents;
using SkagenBooking.Application.Policies;
using SkagenBooking.Application.Abstractions;
using SkagenBooking.Core.Entities;
using SkagenBooking.Core.Interfaces;
using SkagenBooking.Core.Services;
using SkagenBooking.Core.ValueObjects;

namespace SkagenBooking.Application.Bookings.Commands.CreateBooking;

public sealed class CreateBookingUseCase : ICreateBookingUseCase
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingAggregateRepository _bookingRepository;
    private readonly IParkingRepository _parkingRepository;
    private readonly IPricingService _pricingService;
    private readonly BookingWindowPolicy _bookingWindowPolicy;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly IOutbox _outbox;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookingUseCase(
        IRoomRepository roomRepository,
        IBookingAggregateRepository bookingRepository,
        IParkingRepository parkingRepository,
        IPricingService pricingService,
        BookingWindowPolicy bookingWindowPolicy,
        IDomainEventDispatcher domainEventDispatcher,
        IOutbox outbox,
        IUnitOfWork unitOfWork)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
        _parkingRepository = parkingRepository;
        _pricingService = pricingService;
        _bookingWindowPolicy = bookingWindowPolicy;
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

        if (command.CheckInDate.Date < DateTime.Today)
        {
            return new CreateBookingResult
            {
                IsCreated = false,
                Message = "Check-in date cannot be in the past."
            };
        }

        if (command.GuestCount > room.Capacity)
        {
            return new CreateBookingResult { IsCreated = false, Message = "Guest count exceeds room capacity." };
        }

        var range = new DateRange(command.CheckInDate, command.CheckOutDate);
        if (!_bookingWindowPolicy.IsValidCheckIn(TimeOnly.FromDateTime(command.CheckInDate)))
        {
            return new CreateBookingResult { IsCreated = false, Message = "Check-in time must be between 14:00 and 22:30." };
        }

        if (!_bookingWindowPolicy.IsValidCheckOut(TimeOnly.FromDateTime(command.CheckOutDate)))
        {
            return new CreateBookingResult { IsCreated = false, Message = "Check-out time must be no later than 12:00." };
        }

        var overlaps = await _bookingRepository.ExistsOverlapAsync(room.Id, range, cancellationToken);
        if (overlaps)
        {
            return new CreateBookingResult { IsCreated = false, Message = "Room is not available for selected dates." };
        }

        if (command.NeedsParking)
        {
            var hasParking = await _parkingRepository.HasFreeSlotAsync(command.PropertyId, command.CheckInDate, command.CheckOutDate, cancellationToken);
            if (!hasParking)
            {
                return new CreateBookingResult { IsCreated = false, Message = "No parking slot available for selected dates." };
            }
        }

        var booking = Booking.Create(
            command.PropertyId,
            room.Id,
            range,
            command.GuestCount,
            command.NeedsParking,
            command.IsLateArrival,
            command.EstimatedArrivalTime);

        await _bookingRepository.AddAsync(booking, cancellationToken);
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
