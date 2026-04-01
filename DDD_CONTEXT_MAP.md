# DDD Context Map (Current v1)

## Bounded Contexts

### BookingContext
- Owns booking lifecycle: create, confirm, cancel.
- Owns booking invariants:
  - no invalid date range,
  - guest count must be positive,
  - late arrival requires ETA.
- Aggregate Root: `Booking`.

### InventoryContext
- Owns `Property`, `Room`, and room availability inventory.
- Supplies room data and capacity limits to booking use cases.
- Aggregate Root candidates:
  - `Property` (future),
  - `Room` (currently modeled as entity reference).

### ParkingContext
- Owns parking slot capacity and parking availability checks.
- Current rule: max two slots per property.
- Aggregate Root candidate: `ParkingSlotAllocation` (future persistence model).

### PricingContext
- Owns price calculation policy.
- Current implementation: room nightly rate * number of nights.
- Designed to evolve to seasonal/strategy pricing.

## Application Layer Role

`SkagenBooking.Application` orchestrates cross-context workflows:
- `CreateBookingUseCase` coordinates Booking + Inventory + Parking + Pricing.
- It does not own business invariants that belong to domain objects.

## Strategic Notes

- Console is a temporary adapter and not part of domain model.
- Repositories are ports; Infrastructure provides in-memory adapters today and EF Core adapters later.
- Next DDD step: add explicit aggregate persistence boundaries and domain event handlers.
