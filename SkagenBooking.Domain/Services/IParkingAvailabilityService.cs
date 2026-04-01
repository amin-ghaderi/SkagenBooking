using SkagenBooking.Core.Entities;
using SkagenBooking.Core.ValueObjects;

namespace SkagenBooking.Core.Services;

public interface IParkingAvailabilityService
{
    bool HasFreeSlot(
        IReadOnlyList<ParkingAllocation> existingAllocations,
        DateRange requestedRange,
        int capacityPerProperty);
}

