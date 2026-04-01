using SkagenBooking.Core.Entities;

namespace SkagenBooking.Core.Interfaces;

public interface IPropertyRepository
{
    Task<Property?> GetByIdAsync(int propertyId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Property>> GetAllAsync(CancellationToken cancellationToken);
}
