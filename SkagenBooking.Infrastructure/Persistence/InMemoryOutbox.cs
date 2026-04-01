using System.Text.Json;
using SkagenBooking.Application.Abstractions;
using SkagenBooking.Core.Common;

namespace SkagenBooking.Infrastructure.Persistence;

public sealed class InMemoryOutbox : IOutbox
{
    private readonly List<OutboxMessage> _messages = new();

    public Task EnqueueAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in domainEvents)
        {
            _messages.Add(new OutboxMessage
            {
                Type = domainEvent.GetType().FullName ?? domainEvent.GetType().Name,
                Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                OccurredOnUtc = domainEvent.OccurredOnUtc
            });
        }

        return Task.CompletedTask;
    }

    public IReadOnlyList<OutboxMessage> Messages => _messages;
}
