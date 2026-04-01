namespace SkagenBooking.Core.Entities;

public class ParkingSlot
{
    public int Id { get; private set; }
    public int PropertyId { get; private set; }
    public bool IsActive { get; private set; }

    public ParkingSlot(int id, int propertyId, bool isActive = true)
    {
        Id = id;
        PropertyId = propertyId;
        IsActive = isActive;
    }
}
