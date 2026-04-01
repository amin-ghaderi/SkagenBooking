using SkagenBooking.Application.Bookings.Commands.CreateBooking;
using SkagenBooking.Application.Bookings.Queries.GetBookings;
using SkagenBooking.Core.Services;
using SkagenBooking.Core.ValueObjects;
using SkagenBooking.Core.Interfaces;
using SkagenBooking.Application.Rooms.Queries.GetRooms;

namespace SkagenBooking.Console.Application;

/// <summary>
/// Facade over application use cases tailored for the console user interface.
/// </summary>
public sealed class ConsoleAppService
{
    private readonly IGetRoomsUseCase _getRoomsUseCase;
    private readonly ICreateBookingUseCase _createBookingUseCase;

    private readonly IGetBookingsUseCase _getBookingsUseCase;

    private readonly IRoomRepository _roomRepository;
    private readonly IPricingService _pricingService;

    public ConsoleAppService(
        IGetRoomsUseCase getRoomsUseCase,
        ICreateBookingUseCase createBookingUseCase,
        IGetBookingsUseCase getBookingsUseCase,
        IRoomRepository roomRepository,
        IPricingService pricingService)
    {
        _getRoomsUseCase = getRoomsUseCase;
        _createBookingUseCase = createBookingUseCase;
        _getBookingsUseCase = getBookingsUseCase;
        _roomRepository = roomRepository;
        _pricingService = pricingService;
    }

    public Task<IReadOnlyList<RoomListItemDto>> GetRoomsAsync(int propertyId, CancellationToken cancellationToken)
    {
        return _getRoomsUseCase.ExecuteAsync(new GetRoomsQuery { PropertyId = propertyId }, cancellationToken);
    }

    public Task<CreateBookingResult> CreateBookingAsync(CreateBookingCommand command, CancellationToken cancellationToken)
    {
        return _createBookingUseCase.ExecuteAsync(command, cancellationToken);
    }

    public Task<IReadOnlyList<BookingListItemDto>> GetBookingsAsync(int? propertyId, CancellationToken cancellationToken)
    {
        return _getBookingsUseCase.ExecuteAsync(new GetBookingsQuery { PropertyId = propertyId }, cancellationToken);
    }

    public async Task<Money?> GetEstimatedPriceAsync(int roomId, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(roomId, cancellationToken);
        if (room is null)
        {
            return null;
        }

        var range = new DateRange(checkIn, checkOut);
        var price = _pricingService.CalculatePrice(room, range);
        return price;
    }
}
