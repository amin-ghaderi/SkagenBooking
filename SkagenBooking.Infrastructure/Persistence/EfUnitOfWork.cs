using SkagenBooking.Application.Abstractions;

namespace SkagenBooking.Infrastructure.Persistence;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly SkagenBookingDbContext _dbContext;

    public EfUnitOfWork(SkagenBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}

