using System.ComponentModel.DataAnnotations;

namespace SkagenBooking.Api.Contracts.Bookings;

public sealed class CreateBookingRequest : IValidatableObject
{
    [Range(1, int.MaxValue)]
    public int PropertyId { get; init; }

    [Range(1, int.MaxValue)]
    public int RoomId { get; init; }

    public DateTime CheckInDate { get; init; }
    public DateTime CheckOutDate { get; init; }

    [Range(1, int.MaxValue)]
    public int GuestCount { get; init; }
    public bool NeedsParking { get; init; }
    public bool IsLateArrival { get; init; }
    public TimeOnly? EstimatedArrivalTime { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CheckInDate == default)
        {
            yield return new ValidationResult("CheckInDate is required.", new[] { nameof(CheckInDate) });
        }

        if (CheckOutDate == default)
        {
            yield return new ValidationResult("CheckOutDate is required.", new[] { nameof(CheckOutDate) });
        }

        if (CheckInDate != default && CheckOutDate != default && CheckOutDate <= CheckInDate)
        {
            yield return new ValidationResult("CheckOutDate must be after CheckInDate.", new[] { nameof(CheckOutDate) });
        }
    }
}

