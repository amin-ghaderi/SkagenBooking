# Skagen Booking System

## 📌 Overview

Skagen Booking is a small, production-style booking system for a bed & breakfast in Skagen.  
It focuses on clean, maintainable design using Domain‑Driven Design (DDD) and Clean Architecture, with support for room reservations, availability checks, pricing, and parking allocation.

## 🧱 Architecture

The solution is organized into clear layers:

- **Domain (`SkagenBooking.Domain`)**  
  Contains the core domain model, value objects, domain services, policies, and repository interfaces. It has no dependencies on UI, database, or frameworks.

- **Application (`SkagenBooking.Application`)**  
  Implements use cases (e.g. `CreateBookingUseCase`, `GetRoomsUseCase`, `GetBookingsUseCase`). It orchestrates flows between domain objects and handles cross‑cutting concerns like unit of work and outbox, but does not own business rules.

- **Infrastructure (`SkagenBooking.Infrastructure`)**  
  Provides in‑memory implementations of domain/application interfaces (repositories, outbox, unit of work). This layer is where a real database (e.g. EF Core) can be plugged in later without changing domain or application code.

- **Console (`SkagenBooking.Console`)**  
  A console UI used as a demo and testing adapter. It handles user interaction, delegates all business operations to the Application layer via `ConsoleAppService`, and wires dependencies in `ConsoleBootstrap`.

## 🧠 Domain Concepts

- **Booking (aggregate root)**  
  The main aggregate for reservations. It enforces core invariants such as guest count, date rules, late arrival requirements, and status transitions (pending, confirmed, cancelled).

- **ParkingAllocation (linked to Booking)**  
  An aggregate representing a parking reservation derived from a `Booking`. It is created via a domain factory (`ParkingAllocation.CreateFromBooking`) so that no parking allocation can exist without an associated booking.

- **Property (with ParkingCapacity)**  
  Represents a property (e.g. Pernille’s B&B) and holds configuration such as `ParkingCapacity`, which defines how many parking spots are available for that property.

- **Value Objects**  
  - `DateRange` – encapsulates a start/end pair and provides overlap and night‑counting logic.  
  - `Money` – represents monetary amounts with currency and supports safe operations like multiplication.

## ⚙️ Business Rules

- **No overlapping bookings**  
  Room availability is enforced by `AvailabilityService`, which uses `DateRange` overlap logic and existing bookings for the room.

- **Check‑in/check‑out time rules**  
  `BookingWindowPolicy` and `DateRange` ensure check‑in and check‑out times fall within allowed windows (e.g. check‑in after 14:00, check‑out before 12:00).

- **Capacity validation**  
  `Booking` validates that guest count is positive and does not exceed the room capacity.

- **Late arrival requires ETA**  
  When a late arrival is indicated (after a defined threshold), an estimated time of arrival must be provided; otherwise the booking cannot be created.

- **Parking capacity per property**  
  `ParkingAvailabilityService` checks parking allocations per property and date range against the `ParkingCapacity` defined on `Property`. Parking allocations are created only when there is remaining capacity.

## ▶️ How to Run

From the repository root:

```bash
dotnet build
dotnet run --project SkagenBooking.Console
```

## 🧪 Tests

Run the full test suite:

```bash
dotnet test
```

## 📄 Documentation

- `ARCHITECTURE_NOTES.md` – describes the overall architecture, layers, and responsibilities.  
- `DDD_CONTEXT_MAP.md` – documents bounded contexts, aggregates, and how they interact.

## 🎯 Design Goals

- **Domain owns business logic** – entities, value objects, and domain services enforce all important rules.  
- **Application orchestrates** – use cases coordinate domain operations and technical concerns without embedding rules.  
- **Infrastructure is simple** – repositories and adapters are data‑only and easy to swap (e.g. in‑memory to EF Core).  
- **Clean Architecture boundaries** – dependencies point inward toward the domain, keeping the core independent and testable.

## 🚀 Status

- Booking system implemented with DDD‑style aggregates and value objects.  
- Parking redesigned using `ParkingAllocation` and `ParkingAvailabilityService`.  
- Console UI working for creating and listing bookings.  
- All automated tests currently passing.

## 👨‍💻 Author

Amin Ghaderi

