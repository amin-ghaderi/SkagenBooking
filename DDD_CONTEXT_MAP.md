# DDD Context Map

## Core Domain

The core domain is booking management for a Bed & Breakfast.

## Main Concepts

### Booking
Represents a reservation made by a guest.

### Room
Represents an available room with capacity and pricing.

### Property
Represents the Bed & Breakfast location.

### Parking Allocation
Tracks parking usage during booking periods.

## Key Rules

- A room cannot be double-booked.
- Guest count cannot exceed room capacity.
- Parking capacity must not be exceeded.
- Cancelled bookings release parking allocations.

## Booking Lifecycle

Pending
→ Updated
→ Cancelled
