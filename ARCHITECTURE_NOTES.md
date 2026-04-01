# SkagenBooking – Architecture Notes

This document describes the current architecture of the SkagenBooking solution and
the responsibilities of each layer/project. The goals are:

- Keep booking logic independent from any specific UI.
- Make it easy to plug in a database, Web API, and front‑end without changing core logic.
- Provide a structure that looks professional and is maintainable in a real‑world team.

---

## 1. Projects / Layers

### 1.1. `SkagenBooking.Domain`

**Responsibilities**

- Domain model:
  - `Booking`, `Room`, `Property`, `Guest`, `ParkingSlot`
- Value Objects:
  - `DateRange`, `Money`, `CheckInWindow`
- Enums:
  - `BookingStatus`, `RoomType`
- Domain ports (interfaces):
  - `IBookingRepository`, `IRoomRepository`, `IParkingRepository`, `IPropertyRepository`
- Domain services:
  - `AvailabilityService`, `BasicPricingService`, `IAvailabilityService`, `IPricingService`
- DDD building blocks:
  - `AggregateRoot`, `IDomainEvent`, `BookingCreatedDomainEvent`

**Key points**

- This project has **no dependency** on infrastructure, database, UI, or web frameworks.
- If all other projects were removed, `SkagenBooking.Domain` should still build on its own.

---

### 1.2. `SkagenBooking.Application`

**Responsibilities**

- Use cases / application services:
  - `CreateBookingUseCase` – create a booking
  - `GetRoomsUseCase` – list rooms
  - `GetBookingsUseCase` – list existing bookings
- Application contracts / patterns:
  - `IUseCase`, `ICommand<T>`, `IQuery<T>`
  - `Result` wrapper for success/failure
- Technical abstractions:
  - `IUnitOfWork` – transaction boundary
  - `IOutbox` – for the Outbox pattern (future DB integration)
  - Domain events:
    - `IDomainEventHandler<T>`, `IDomainEventDispatcher`
    - `InMemoryDomainEventDispatcher`
- Policies:
  - `BookingWindowPolicy` (check‑in/check‑out window, late arrival rules)

**Key points**

- Application orchestrates flows between domain objects, but **does not own** core invariants.
- Depends on the `Domain` project and its own abstractions, but not on any UI.
- This is the layer that Console, Web API, or any other UI will call.

---

### 1.3. `SkagenBooking.Infrastructure`

**Responsibilities**

- Concrete implementations for domain and application interfaces:
  - In‑memory repositories:
    - `InMemoryRoomRepository`
    - `InMemoryBookingRepository`
    - `InMemoryParkingRepository`
- Persistence skeleton:
  - `OutboxMessage`, `InMemoryOutbox`
  - `InMemoryUnitOfWork`
  - `PersistenceOptions` (future connection‑string configuration)
- Composition helpers (future):
  - `InfrastructureRegistration` (placeholder for DI registration)

**Key points**

- Everything is currently in‑memory to focus on domain and application first.
- When a real database is added, EF Core will live here:
  - `DbContext`, entity configurations, and mapping of domain entities to tables.
- Application only depends on interfaces, so swapping in‑memory repos with EF Core
  should not require use‑case changes.

---

### 1.4. `SkagenBooking.Console`

**Responsibilities**

- Temporary UI for testing and demo:
  - Shows the four rooms with descriptions matching the PDF.
  - Collects booking input (room, dates, guests, parking, late arrival).
  - Shows a booking summary and estimated price before confirmation.
  - Creates bookings through `CreateBookingUseCase`.
  - Provides a “List current bookings” menu option.
- Console application service:
  - `ConsoleAppService` as a façade over use cases.
- Composition root:
  - `ConsoleBootstrap.Build()` builds the entire object graph for the console app.
  - `Program.cs` only deals with user interaction through `appService`, not wiring.

**Why this matters**

- Console is just an **adapter** over Application.
- Adding a Web API means:
  - Domain and Application stay as they are.
  - A new adapter project (e.g. `SkagenBooking.Api`) is added on top.

---

### 1.5. `SkagenBooking.Tests`

**Responsibilities**

- Domain tests:
  - `BookingTests` – core booking behaviour (initial status, ETA rule, cancel/confirm rules).
  - `DateRangeTests` – overlap logic and day counting.
- Integration tests:
  - `CreateBookingUseCaseIntegrationTests` – end‑to‑end scenarios with in‑memory infrastructure.
- The test project references `Domain`, `Application`, and `Infrastructure`.

---

## 2. Main Booking Flow

1. **UI (Console):**
   - User selects room, dates, guest count, parking, and late arrival.
   - Input is mapped to `CreateBookingCommand`.
2. **Application (Use Case):**
   - `CreateBookingUseCase`:
     - Loads the room via `IRoomRepository`.
     - Checks room capacity against `GuestCount`.
     - Validates dates using `BookingWindowPolicy` and `DateRange`.
     - Checks for overlapping bookings via `IBookingRepository.ExistsOverlapAsync`.
     - Checks parking availability via `IParkingRepository` when required.
     - Creates the aggregate via `Booking.Create(...)`.
     - Persists the booking via `IBookingRepository.AddAsync`.
     - Enqueues and dispatches domain events (Outbox + dispatcher).
3. **Domain:**
   - `Booking` enforces its own invariants (guest count, late‑arrival ETA, status transitions).
   - `DateRange` handles overlap logic and night counting.
   - `BasicPricingService` calculates final price based on nights and nightly rate.
4. **Infrastructure:**
   - In‑memory repositories store data for the current process only.
   - Later these will be replaced (or complemented) by EF Core and a real database.

---

## 3. Next Evolution Steps

The architecture has been shaped so further steps can be added without heavy refactoring:

1. **Add Web API (`SkagenBooking.Api`)**
   - Re‑use existing use cases.
   - Map HTTP requests/responses to commands/DTOs.
2. **Add Database (e.g. SQL Server + EF Core)**
   - Introduce `DbContext` and configurations in Infrastructure.
   - Implement repositories on top of EF Core.
   - Bind `PersistenceOptions` to configuration for connection strings.
3. **Add Frontend (React or similar)**
   - Consume API endpoints to:
     - list rooms,
     - check availability,
     - create bookings.
4. **Strengthen DDD**
   - Introduce real domain event handlers and a proper Outbox pipeline.
   - Split Bounded Contexts into separate projects if/when the system grows.

This document is meant to help any new developer understand, within a few minutes,
where each responsibility lives and how the pieces of the system fit together.
