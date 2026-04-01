using SkagenBooking.Core.Entities;
using SkagenBooking.Core.ValueObjects;

namespace SkagenBooking.Core.Services;

/// <summary>
/// Domain service that determines parking availability for a property
/// based on existing allocations and a requested date range.
/// </summary>
public sealed class ParkingAvailabilityService : IParkingAvailabilityService
{
    public bool HasFreeSlot(
        IReadOnlyList<ParkingAllocation> existingAllocations,
        DateRange requestedRange,
        int capacityPerProperty)
    {
        var overlappingCount = existingAllocations
            .Count(a => a.DateRange.Overlaps(requestedRange));

        return overlappingCount < capacityPerProperty;
    }
}

