# SkagenBooking – Architecture Notes

## Goal

Clean, production-ready architecture where:
- Domain owns business rules
- Application orchestrates workflows
- Infrastructure handles persistence
- API exposes functionality

---

## Layers

### Domain
- Booking (aggregate)
- ParkingAllocation
- Value Objects: DateRange, Money
- Services: Availability, Parking, Pricing

No external dependencies.

---

### Application
Use cases:
- CreateBooking
- UpdateBooking
- CancelBooking
- GetBookings

Handles orchestration and transactions.

---

### Infrastructure
- EF Core + SQLite
- DbContext
- Repositories
- UnitOfWork
- Migrations + Seed

---

### API
- Controllers
- DTOs
- Validation
- ProblemDetails
- Swagger

Thin layer over Application.

---

## Flow

API → Application → Domain → Infrastructure → DB

---

## Key Decisions

- IClock for testable time
- 409 Conflict for overlaps
- EF mapping for value objects

---

## Current State

- Full booking lifecycle implemented
- Database persistence enabled
- API ready

---

## Next Steps

- API integration tests
- Outbox persistence
- Concurrency handling
- Pagination