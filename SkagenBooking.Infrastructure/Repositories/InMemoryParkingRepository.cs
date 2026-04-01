using SkagenBooking.Core.Entities;
using SkagenBooking.Core.Interfaces;

namespace SkagenBooking.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of parking allocation repository.
/// </summary>
public class InMemoryParkingRepository : IParkingAllocationRepository
{
    private readonly List<ParkingAllocation> _allocations = new();

    public Task<IReadOnlyList<ParkingAllocation>> GetByPropertyAsync(int propertyId, CancellationToken cancellationToken)
    {
        IReadOnlyList<ParkingAllocation> result = _allocations
            .Where(a => a.PropertyId == propertyId)
            .ToList();

        return Task.FromResult(result);
    }

    public Task AddAsync(ParkingAllocation allocation, CancellationToken cancellationToken)
    {
        _allocations.Add(allocation);
        return Task.CompletedTask;
    }
}
