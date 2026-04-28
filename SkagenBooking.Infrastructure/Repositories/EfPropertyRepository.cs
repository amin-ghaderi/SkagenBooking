using Microsoft.EntityFrameworkCore;
using SkagenBooking.Core.Entities;
using SkagenBooking.Core.Interfaces;
using SkagenBooking.Infrastructure.Persistence;

namespace SkagenBooking.Infrastructure.Repositories;

public sealed class EfPropertyRepository : IPropertyRepository
{
    private readonly SkagenBookingDbContext _dbContext;

    public EfPropertyRepository(SkagenBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Property?> GetByIdAsync(int propertyId, CancellationToken cancellationToken)
    {
        return _dbContext.Properties.FirstOrDefaultAsync(x => x.Id == propertyId, cancellationToken);
    }

    public async Task<IReadOnlyList<Property>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Properties
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }
}

