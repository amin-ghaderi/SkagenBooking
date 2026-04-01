using SkagenBooking.Core.Common;

namespace SkagenBooking.Application.Common.DomainEvents;

public interface IDomainEventHandler<in TDomainEvent> where TDomainEvent : IDomainEvent
{
    Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken);
}
