using SkagenBooking.Core.Entities;
using SkagenBooking.Core.Interfaces;

namespace SkagenBooking.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of property repository with a single property (Pernille's B&B).
/// </summary>
public class InMemoryPropertyRepository : IPropertyRepository
{
    private readonly List<Property> _properties = new()
    {
        new Property(
            id: 1,
            name: "Pernille's Bed & Breakfast",
            city: "Skagen",
            parkingCapacity: 2)
    };

    public Task<Property?> GetByIdAsync(int propertyId, CancellationToken cancellationToken)
    {
        var property = _properties.FirstOrDefault(p => p.Id == propertyId);
        return Task.FromResult(property);
    }

    public Task<IReadOnlyList<Property>> GetAllAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Property> result = _properties;
        return Task.FromResult(result);
    }
}

