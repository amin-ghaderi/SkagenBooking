using SkagenBooking.Core.Common;

namespace SkagenBooking.Application.Abstractions;

public interface IOutbox
{
    Task EnqueueAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken);
}
