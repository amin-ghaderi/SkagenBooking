using SkagenBooking.Api.Contracts.Properties;
using SkagenBooking.Application.Properties.Queries.GetProperties;

namespace SkagenBooking.Api.Mapping;

internal static class PropertyResponseMapper
{
    public static PropertyResponse From(PropertyListItemDto property) => new()
    {
        Id = property.Id,
        Name = property.Name,
        City = property.City
    };
}
