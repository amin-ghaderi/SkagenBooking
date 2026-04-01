using SkagenBooking.Core.Common;

namespace SkagenBooking.Core.Events;

public sealed class BookingCreatedDomainEvent : IDomainEvent
{
    public BookingCreatedDomainEvent(int propertyId, int roomId)
    {
        PropertyId = propertyId;
        RoomId = roomId;
    }

    public int PropertyId { get; }
    public int RoomId { get; }
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
