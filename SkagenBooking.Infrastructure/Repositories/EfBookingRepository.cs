using Microsoft.EntityFrameworkCore;
using SkagenBooking.Core.Entities;
using SkagenBooking.Core.Interfaces;
using SkagenBooking.Infrastructure.Persistence;

namespace SkagenBooking.Infrastructure.Repositories;

public sealed class EfBookingRepository : IBookingAggregateRepository
{
    private readonly SkagenBookingDbContext _dbContext;

    public EfBookingRepository(SkagenBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken)
    {
        await _dbContext.Bookings.AddAsync(booking, cancellationToken);
    }

    public async Task<IReadOnlyList<Booking>> GetByRoomAsync(int roomId, CancellationToken cancellationToken)
    {
        return await _dbContext.Bookings
            .Where(x => x.RoomId == roomId)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }
}

