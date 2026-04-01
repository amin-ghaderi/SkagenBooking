using SkagenBooking.Core.Common;

namespace SkagenBooking.Application.Common.DomainEvents;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken);
}
