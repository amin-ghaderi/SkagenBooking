namespace SkagenBooking.Core.Common;

/// <summary>
/// Marker interface for domain events raised by aggregates.
/// </summary>
public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
