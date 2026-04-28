# DDD Context Map – SkagenBooking

## Overview

The system is modeled using bounded contexts coordinated via the Application layer.

---

## Contexts

### BookingContext
Aggregate: Booking

Handles:
- Create / Update / Cancel
- Date rules
- Guest count
- Status transitions
- Late arrival rules

---

### InventoryContext
Entities:
- Property
- Room

Provides:
- Capacity
- Pricing data

---

### ParkingContext
Aggregate: ParkingAllocation

Handles:
- Parking capacity per property
- Linked to Booking

Rule:
No ParkingAllocation without Booking

---

### PricingContext
Handles:
- Price calculation

Formula:
NightlyRate × Nights

---

## Coordination

Handled by Application layer

Example:
CreateBookingUseCase uses:
- Room (Inventory)
- Booking (Domain)
- Parking (Capacity)
- Pricing

---

## Principles

- Booking is main aggregate
- Parking is derived
- Domain owns rules
- Application orchestrates

---

## Status

- Create / Update / Cancel implemented
- EF Core integrated
- API complete