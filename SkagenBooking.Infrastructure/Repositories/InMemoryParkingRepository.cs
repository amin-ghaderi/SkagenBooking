using SkagenBooking.Core.Interfaces;

namespace SkagenBooking.Infrastructure.Repositories;

public class InMemoryParkingRepository : IParkingRepository
{
    private const int ParkingCapacityPerProperty = 2;
    private readonly Dictionary<int, int> _reservedSlots = new();

    public Task<bool> HasFreeSlotAsync(int propertyId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        _reservedSlots.TryGetValue(propertyId, out var reserved);
        return Task.FromResult(reserved < ParkingCapacityPerProperty);
    }
}
