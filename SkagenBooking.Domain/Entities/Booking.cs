using System.Threading;
using SkagenBooking.Core.Enums;
using SkagenBooking.Core.Events;
using SkagenBooking.Core.ValueObjects;
using SkagenBooking.Core.Common;
using SkagenBooking.Core.Policies;

namespace SkagenBooking.Core.Entities;

/// <summary>
/// Aggregate root representing a room booking for a property over a date range.
/// </summary>
public class Booking : AggregateRoot
{
    private static int _nextId;

    public int Id { get; private set; }
    public int PropertyId { get; private set; }

    public int RoomId { get; private set; }

    public DateRange DateRange { get; private set; }
    public int GuestCount { get; private set; }
    public bool NeedsParking { get; private set; }
    public TimeOnly? EstimatedArrivalTime { get; private set; }

    public BookingStatus Status { get; private set; }

    private Booking()
    {
        DateRange = null!;
    }

    private Booking(int propertyId, int roomId, DateRange dateRange, int guestCount, bool needsParking, TimeOnly? estimatedArrivalTime)
    {
        Id = Interlocked.Increment(ref _nextId);
        PropertyId = propertyId;
        RoomId = roomId;
        DateRange = dateRange;
        GuestCount = guestCount;
        NeedsParking = needsParking;
        EstimatedArrivalTime = estimatedArrivalTime;
        Status = BookingStatus.Pending;
    }

    /// <summary>
    /// Attempts to create a booking aggregate while enforcing domain invariants and policies.
    /// </summary>
    /// <param name="room">Room the guest wants to book.</param>
    /// <param name="dateRange">Requested date range for the stay.</param>
    /// <param name="guestCount">Number of guests.</param>
    /// <param name="needsParking">Whether parking should be reserved.</param>
    /// <param name="isLateArrival">Whether the guest will arrive after the late-arrival threshold.</param>
    /// <param name="estimatedArrivalTime">Estimated arrival time when arriving late.</param>
    /// <param name="policy">Booking time-window policy.</param>
    /// <param name="currentDate">Current date used to prevent bookings in the past.</param>
    /// <returns>Result containing either the created booking or an error message.</returns>
    public static BookingCreationResult TryCreate(
        Room room,
        DateRange dateRange,
        int guestCount,
        bool needsParking,
        bool isLateArrival,
        TimeOnly? estimatedArrivalTime,
        BookingWindowPolicy policy,
        DateTime currentDate)
    {
        if (guestCount <= 0)
            return BookingCreationResult.Failure("Guest count must be greater than zero.");

        if (guestCount > room.Capacity)
            return BookingCreationResult.Failure("Guest count exceeds room capacity.");

        if (dateRange.CheckIn.Date < currentDate.Date)
            return BookingCreationResult.Failure("Check-in date cannot be in the past.");

        if (!policy.IsValidCheckIn(TimeOnly.FromDateTime(dateRange.CheckIn)))
            return BookingCreationResult.Failure("Check-in time must be between 14:00 and 22:30.");

        if (!policy.IsValidCheckOut(TimeOnly.FromDateTime(dateRange.CheckOut)))
            return BookingCreationResult.Failure("Check-out time must be no later than 12:00.");

        // Business rule from project statement: late arrivals after 20:00 should provide ETA.
        if (isLateArrival && estimatedArrivalTime is null)
            return BookingCreationResult.Failure("Estimated arrival time is required for arrivals after 20:00.");

        var booking = new Booking(room.PropertyId, room.Id, dateRange, guestCount, needsParking, estimatedArrivalTime);
        booking.Raise(new BookingCreatedDomainEvent(room.PropertyId, room.Id));
        return BookingCreationResult.Success(booking);
    }

    public static void InitializeNextId(int currentMaxId)
    {
        if (currentMaxId > _nextId)
        {
            _nextId = currentMaxId;
        }
    }

    /// <summary>
    /// Confirms the booking.
    /// </summary>
    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Only pending bookings can be confirmed.");

        Status = BookingStatus.Confirmed;
    }

    /// <summary>
    /// Cancels the booking.
    /// </summary>
    public void Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Booking is already cancelled.");

        Status = BookingStatus.Cancelled;
    }
}