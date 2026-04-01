# DDD Context Map – SkagenBooking

This document captures the Domain‑Driven Design view of the SkagenBooking
system: which bounded contexts we have, what each one owns, and how they
interact via the Application layer.

---

## 1. Bounded Contexts

In the current implementation all contexts live in a single solution and in
the same `Domain` project, but conceptually they can be separated as follows.

### 1.1. BookingContext

**Responsibilities**

- Booking life‑cycle:
  - Create (`Booking.Create`)
  - Confirm (`Confirm`)
  - Cancel (`Cancel`)
- Core invariants:
  - Valid date range (`DateRange`, `CheckOut > CheckIn`)
  - Positive guest count
  - A completed booking cannot be cancelled; a cancelled booking cannot be confirmed
  - For arrivals after 20:00, an ETA is required

**Aggregate Root**

- `Booking`

**Domain Events**

- `BookingCreatedDomainEvent` – used to trigger side‑effects such as notifications/outbox.

---

### 1.2. InventoryContext

**Responsibilities**

- Describing the property and its rooms:
  - `Property` (e.g. Pernille’s B&B)
  - `Room` (Single, Double, Family) with capacity and nightly rate
- Maintaining room inventory:
  - Through `Room` and its relation to bookings

**Aggregate Root candidates**

- Today:
  - `Room` is used as an entity referenced from the `Booking` aggregate.
- Future:
  - `Property` could become its own aggregate root when supporting multiple properties.

**Role in the system**

- Source of truth for:
  - room type, capacity, and price per night,
  - data consumed by Availability and Pricing logic.

---

### 1.3. ParkingContext

**Responsibilities**

- Managing parking capacity per property:
  - Current rule: maximum parking spots per property, defined on `Property`.
- Answering the question:
  - “Is there a free parking spot for this property in the requested date range?”

**Aggregate Root**

- `ParkingAllocation`

**Description**

- `ParkingAllocation`:
  - Represents a parking reservation linked to a `Booking`.
  - Contains:
    - `BookingId`
    - `PropertyId`
    - `DateRange`
  - Rules:
    - Parking allocations are created only from `Booking`.
    - No allocation exists without a `Booking`.

---

### 1.4. PricingContext

**Responsibilities**

- Pricing policy:
  - Currently: flat nightly rate per room, matching the initial PDF.
  - Price calculation: `NightlyRate * NumberOfNights`.

**Key types**

- `Money` (amount + currency)
-, `BasicPricingService` and `IPricingService`

**Evolution path**

- Add:
  - seasonal pricing,
  - discounts/promotions,
  - refund/cancellation rules.

---

## 2. Application Layer’s Role Across Contexts

`SkagenBooking.Application` coordinates workflows that span multiple contexts.
It does not own core business rules – those stay in the Domain model.

### 2.1. Example: `CreateBookingUseCase`

This use case touches several contexts:

- From **InventoryContext**:
  - loads a `Room` and checks capacity.
- From **BookingContext**:
  - calls `Booking.Create(...)` to build the aggregate,
  - lets `Booking` enforce its invariants.
- From **ParkingContext**:
  - checks parking availability when `NeedsParking = true`.
- From **PricingContext**:
  - calculates final price via `IPricingService`.

The Application layer:

- acts as an orchestrator and **does not own** core domain rules,
- manages cross‑cutting concerns like ordering of calls and shaping responses for the UI.

---

## 3. Strategic Design Considerations

- **UI (Console / future Web UI)**  
  - UIs are just adapters over Application. No domain logic should live in them.

- **Domain as the shared core for all contexts**  
  - Today all contexts share a single `Domain` project.
  - If the system grows, important contexts can be split into separate assemblies,
    e.g. `SkagenBooking.Domain.Booking`, `SkagenBooking.Domain.Pricing`, etc.

- **Repositories as ports**  
  - Interfaces live in Domain, implementations in Infrastructure.
  - Now they are in‑memory; later they can be EF Core with a real database.

- **Outbox + Domain Events**  
  - Currently we have an in‑memory Outbox and dispatcher.
  - Later the Outbox can be persisted to DB with a background worker to
    reliably handle email/SMS/notification delivery.

---

## 4. Next DDD Steps

- Add real domain event handlers:
  - e.g. `BookingCreatedDomainEventHandler` that persists an outbox message
    or triggers notifications.
- Optionally split bounded contexts into separate projects when the system grows.
- Clarify aggregate roots for Parking/Inventory if their rules become more complex.

This map is intended to make it easier for a growing team to understand where
each piece of behaviour belongs and how responsibilities are divided across
bounded contexts and layers.

---

## 5. Key Domain Principles

- Booking is the main aggregate for reservations.
- ParkingAllocation is derived from Booking; no ParkingAllocation exists without a Booking.
- Domain services are the single source of business rules; repositories are data-only.
