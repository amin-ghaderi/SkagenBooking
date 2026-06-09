namespace SkagenBooking.Application.Properties.Queries.GetProperties;

public interface IGetPropertiesUseCase
{
    Task<IReadOnlyList<PropertyListItemDto>> ExecuteAsync(GetPropertiesQuery query, CancellationToken cancellationToken);
}
