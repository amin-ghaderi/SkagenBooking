# SkagenBooking Architecture Notes

This repository is being prepared for:

- database-ready infrastructure,
- UI-agnostic application layer,
- easy migration from Console UI to Web API.

## Layer Intent

- `SkagenBooking.Core`: domain entities, value objects, enums, contracts.
- `SkagenBooking.Application`: use-case contracts and DTOs.
- `SkagenBooking.Infrastructure`: repository/persistence implementations.
- `SkagenBooking.Console`: temporary presentation adapter.

## Next Execution Step

1. Move booking orchestration from Core services into Application use-cases.
2. Make Console call use-cases only.
3. Add async repository implementations in Infrastructure.
