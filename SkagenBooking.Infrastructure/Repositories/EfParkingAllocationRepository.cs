using Microsoft.EntityFrameworkCore;
using SkagenBooking.Core.Entities;
using SkagenBooking.Core.Interfaces;
using SkagenBooking.Infrastructure.Persistence;

namespace SkagenBooking.Infrastructure.Repositories;

public sealed class EfParkingAllocationRepository : IParkingAllocationRepository
{
    private readonly SkagenBookingDbContext _dbContext;

    public EfParkingAllocationRepository(SkagenBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ParkingAllocation>> GetByPropertyAsync(int propertyId, CancellationToken cancellationToken)
    {
        return await _dbContext.ParkingAllocations
            .Where(x => x.PropertyId == propertyId)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<ParkingAllocation?> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken)
    {
        return _dbContext.ParkingAllocations.FirstOrDefaultAsync(x => x.BookingId == bookingId, cancellationToken);
    }

    public async Task AddAsync(ParkingAllocation allocation, CancellationToken cancellationToken)
    {
        await _dbContext.ParkingAllocations.AddAsync(allocation, cancellationToken);
    }

    public Task RemoveAsync(ParkingAllocation allocation, CancellationToken cancellationToken)
    {
        _dbContext.ParkingAllocations.Remove(allocation);
        return Task.CompletedTask;
    }
}

