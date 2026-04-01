using SkagenBooking.Application.Common.DomainEvents;
using SkagenBooking.Core.Events;

namespace SkagenBooking.Application.Bookings.Events;

public sealed class BookingCreatedDomainEventHandler : IDomainEventHandler<BookingCreatedDomainEvent>
{
    public Task HandleAsync(BookingCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // Placeholder for side effects like notifications/outbox.
        return Task.CompletedTask;
    }
}
