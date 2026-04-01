using SkagenBooking.Core.Common;

namespace SkagenBooking.Application.Abstractions;

/// <summary>
/// Abstraction over an outbox store used to persist domain events for later processing.
/// </summary>
public interface IOutbox
{
    Task EnqueueAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken);
}
