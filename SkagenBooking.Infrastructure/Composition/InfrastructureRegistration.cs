using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkagenBooking.Application.Abstractions;
using SkagenBooking.Application.Bookings.Events;
using SkagenBooking.Application.Common.DomainEvents;
using SkagenBooking.Core.Interfaces;
using SkagenBooking.Core.Policies;
using SkagenBooking.Core.Services;
using SkagenBooking.Infrastructure.Persistence;
using SkagenBooking.Infrastructure.Repositories;
using SkagenBooking.Infrastructure.Time;

namespace SkagenBooking.Infrastructure.Composition;

public static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructureSqlite(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=skagenbooking.db";

        services.AddDbContext<SkagenBookingDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IClock, SystemClock>();

        // Repositories (EF Core)
        services.AddScoped<IRoomRepository, EfRoomRepository>();
        services.AddScoped<IPropertyRepository, EfPropertyRepository>();
        services.AddScoped<IBookingAggregateRepository, EfBookingRepository>();
        services.AddScoped<IParkingAllocationRepository, EfParkingAllocationRepository>();

        // Domain services / policies
        services.AddScoped<IPricingService, BasicPricingService>();
        services.AddScoped<BookingWindowPolicy>();
        services.AddScoped<IAvailabilityService, AvailabilityService>();
        services.AddScoped<IParkingAvailabilityService, ParkingAvailabilityService>();

        // Cross-cutting
        services.AddScoped<IDomainEventDispatcher>(_ =>
        {
            var dispatcher = new InMemoryDomainEventDispatcher();
            dispatcher.Register(new BookingCreatedDomainEventHandler());
            return dispatcher;
        });
        services.AddScoped<IOutbox, InMemoryOutbox>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        return services;
    }

    public static IServiceCollection AddInfrastructureInMemory(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();

        // Repositories (in-memory)
        services.AddSingleton<IRoomRepository, InMemoryRoomRepository>();
        services.AddSingleton<IPropertyRepository, InMemoryPropertyRepository>();
        services.AddSingleton<IBookingAggregateRepository, InMemoryBookingRepository>();
        services.AddSingleton<IParkingAllocationRepository, InMemoryParkingRepository>();

        // Domain services / policies
        services.AddSingleton<IPricingService, BasicPricingService>();
        services.AddSingleton<BookingWindowPolicy>();
        services.AddSingleton<IAvailabilityService, AvailabilityService>();
        services.AddSingleton<IParkingAvailabilityService, ParkingAvailabilityService>();

        // Cross-cutting (in-memory placeholders)
        services.AddSingleton<IDomainEventDispatcher>(_ =>
        {
            var dispatcher = new InMemoryDomainEventDispatcher();
            dispatcher.Register(new BookingCreatedDomainEventHandler());
            return dispatcher;
        });
        services.AddSingleton<IOutbox, InMemoryOutbox>();
        services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();

        return services;
    }
}
