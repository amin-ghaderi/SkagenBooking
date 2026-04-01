namespace SkagenBooking.Core.Interfaces;

public interface IParkingRepository
{
    Task<bool> HasFreeSlotAsync(int propertyId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
}
