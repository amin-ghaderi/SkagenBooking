namespace SkagenBooking.Api.Contracts.Rooms;

/// <summary>
/// Room available for booking at a property.
/// </summary>
public sealed class RoomResponse
{
    public int Id { get; init; }
    public int PropertyId { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Capacity { get; init; }
    public decimal NightlyRate { get; init; }
    public string Currency { get; init; } = string.Empty;
}
