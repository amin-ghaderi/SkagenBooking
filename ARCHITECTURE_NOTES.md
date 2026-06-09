# Architecture Notes

The project follows a simplified Clean Architecture approach.

## Layers

### Domain
Contains business rules and core entities:
- Booking
- Room
- Property
- ParkingAllocation

### Application
Contains use cases and application workflows.

### Infrastructure
Provides database access, repository implementations, and EF Core configuration.

### API
Exposes HTTP endpoints and handles request/response mapping.

### Frontend
React application consuming the API.

## Design Goals

- Clear separation of concerns
- Testable business logic
- Simple persistence layer
- Easy extension for future features
