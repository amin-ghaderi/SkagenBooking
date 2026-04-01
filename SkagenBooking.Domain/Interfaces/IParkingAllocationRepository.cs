using SkagenBooking.Core.Entities;

namespace SkagenBooking.Core.Interfaces;

public interface IParkingAllocationRepository
{
    Task<IReadOnlyList<ParkingAllocation>> GetByPropertyAsync(int propertyId, CancellationToken cancellationToken);
    Task AddAsync(ParkingAllocation allocation, CancellationToken cancellationToken);
}

