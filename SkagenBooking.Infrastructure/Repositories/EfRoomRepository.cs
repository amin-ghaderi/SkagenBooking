using Microsoft.EntityFrameworkCore;
using SkagenBooking.Core.Entities;
using SkagenBooking.Core.Interfaces;
using SkagenBooking.Infrastructure.Persistence;

namespace SkagenBooking.Infrastructure.Repositories;

public sealed class EfRoomRepository : IRoomRepository
{
    private readonly SkagenBookingDbContext _dbContext;

    public EfRoomRepository(SkagenBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Room>> GetAllAsync(int? propertyId, CancellationToken cancellationToken)
    {
        var query = _dbContext.Rooms.AsQueryable();
        if (propertyId.HasValue)
        {
            query = query.Where(x => x.PropertyId == propertyId.Value);
        }

        return await query
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Room?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.Rooms.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}

