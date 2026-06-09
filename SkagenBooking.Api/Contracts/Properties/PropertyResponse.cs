namespace SkagenBooking.Api.Contracts.Properties;

/// <summary>
/// Bookable property (bed and breakfast).
/// </summary>
public sealed class PropertyResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
}
