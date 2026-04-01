using SkagenBooking.Core.Common;

namespace SkagenBooking.Application.Common.DomainEvents;

/// <summary>
/// Dispatches domain events to their registered handlers.
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken);
}
