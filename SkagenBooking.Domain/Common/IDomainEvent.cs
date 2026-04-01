namespace SkagenBooking.Core.Common;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
