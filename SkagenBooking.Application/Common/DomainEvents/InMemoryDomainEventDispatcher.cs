using SkagenBooking.Core.Common;

namespace SkagenBooking.Application.Common.DomainEvents;

/// <summary>
/// Simple in-memory implementation of <see cref="IDomainEventDispatcher"/> used for testing and console scenarios.
/// </summary>
public sealed class InMemoryDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly Dictionary<Type, List<object>> _handlers = new();

    public void Register<TDomainEvent>(IDomainEventHandler<TDomainEvent> handler)
        where TDomainEvent : IDomainEvent
    {
        var key = typeof(TDomainEvent);
        if (!_handlers.TryGetValue(key, out var handlers))
        {
            handlers = new List<object>();
            _handlers[key] = handlers;
        }

        handlers.Add(handler);
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in events)
        {
            var key = domainEvent.GetType();
            if (!_handlers.TryGetValue(key, out var handlers))
            {
                continue;
            }

            foreach (var handler in handlers)
            {
                await ((dynamic)handler).HandleAsync((dynamic)domainEvent, cancellationToken);
            }
        }
    }
}
