using SkagenBooking.Core.Common;

namespace SkagenBooking.Application.Common.DomainEvents;

/// <summary>
/// Handles a specific type of domain event.
/// </summary>
/// <typeparam name="TDomainEvent">Type of event being handled.</typeparam>
public interface IDomainEventHandler<in TDomainEvent> where TDomainEvent : IDomainEvent
{
    Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken);
}
