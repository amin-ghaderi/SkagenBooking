# Skagen Booking System

## Overview

SkagenBooking is a production-style booking system for a bed & breakfast in Skagen, built using Domain-Driven Design (DDD) and Clean Architecture.

The system supports:
- Room reservations
- Availability validation (no overlaps)
- Pricing calculation
- Parking allocation with capacity constraints
- Full CRUD operations via Web API

The project is designed to be testable, maintainable, and easily extensible.

---

## Architecture

The solution follows Clean Architecture:

- Domain (SkagenBooking.Domain)
  Core business logic (entities, value objects, domain services)

- Application (SkagenBooking.Application)
  Use cases (Create, Update, Cancel, Query bookings)

- Infrastructure (SkagenBooking.Infrastructure)
  EF Core + SQLite, repositories, UnitOfWork, migrations

- API (SkagenBooking.Api)
  ASP.NET Core Web API (Controllers, DTOs, validation, Swagger)

- Console (SkagenBooking.Console)
  Demo adapter (legacy)

- Tests (SkagenBooking.Tests)
  Integration tests

---

## Features

- Create / Update / Cancel bookings
- Prevent overlapping bookings
- Parking capacity management
- Validation (API + Domain)
- Swagger UI
- EF Core persistence

---

## Run

dotnet build  
dotnet run --project SkagenBooking.Api  

Swagger:
http://localhost:5023/swagger

---

## Tests

dotnet test

---

## Database

SQLite + EF Core

dotnet ef database update --project SkagenBooking.Infrastructure --startup-project SkagenBooking.Api

Seed data runs automatically.

---

## Status

Phase 1: API ✔  
Phase 2: EF Core ✔  
Phase 3: Update/Cancel ✔  

System is fully functional end-to-end.