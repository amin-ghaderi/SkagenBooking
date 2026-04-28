using Microsoft.Extensions.DependencyInjection;
using SkagenBooking.Application.Bookings.Commands.CancelBooking;
using SkagenBooking.Application.Bookings.Commands.CreateBooking;
using SkagenBooking.Application.Bookings.Commands.UpdateBooking;
using SkagenBooking.Application.Bookings.Queries.GetBookings;
using SkagenBooking.Application.Rooms.Queries.GetRooms;

namespace SkagenBooking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Use cases
        services.AddTransient<IGetRoomsUseCase, GetRoomsUseCase>();
        services.AddTransient<IGetBookingsUseCase, GetBookingsUseCase>();
        services.AddTransient<ICreateBookingUseCase, CreateBookingUseCase>();
        services.AddTransient<IUpdateBookingUseCase, UpdateBookingUseCase>();
        services.AddTransient<ICancelBookingUseCase, CancelBookingUseCase>();

        return services;
    }
}
