using SkagenBooking.Core.Interfaces;

namespace SkagenBooking.Application.Properties.Queries.GetProperties;

public sealed class GetPropertiesUseCase : IGetPropertiesUseCase
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertiesUseCase(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<IReadOnlyList<PropertyListItemDto>> ExecuteAsync(GetPropertiesQuery query, CancellationToken cancellationToken)
    {
        var properties = await _propertyRepository.GetAllAsync(cancellationToken);
        return properties
            .Select(p => new PropertyListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                City = p.City
            })
            .OrderBy(p => p.Id)
            .ToList();
    }
}
