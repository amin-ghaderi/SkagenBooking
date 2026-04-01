namespace SkagenBooking.Application.Abstractions;

/// <summary>
/// Abstraction for coordinating the commit of a set of changes as a single unit.
/// </summary>
public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
