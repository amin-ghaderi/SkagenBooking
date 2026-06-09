# SkagenBooking

A simple booking management system for a Bed & Breakfast in Skagen.

## Features

- Create bookings
- View bookings
- Update bookings
- Cancel bookings
- Parking capacity management
- Booking conflict validation

## Technology Stack

### Backend
- ASP.NET Core Web API
- Clean Architecture
- Entity Framework Core
- SQLite
- Swagger

### Frontend
- React
- Vite
- Tailwind CSS
- Axios

## Solution Structure

- SkagenBooking.Api
- SkagenBooking.Application
- SkagenBooking.Domain
- SkagenBooking.Infrastructure
- SkagenBooking.Tests
- frontend

## Run Backend

```bash
dotnet run --project SkagenBooking.Api
```

API: `http://localhost:5023`  
Swagger: `http://localhost:5023/swagger`

The database is created and seeded automatically on startup.

## Run Frontend

```bash
cd frontend
npm install
npm run dev
```

App: `http://localhost:5173`

In development, Vite proxies `/api` requests to the backend. Ensure the API is running before using the frontend.

## Tests

```bash
dotnet test
```

Frontend:

```bash
cd frontend
npm run lint
npm run build
```

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/properties` | List properties |
| GET | `/api/rooms?propertyId={id}` | List rooms |
| GET | `/api/bookings` | List bookings |
| GET | `/api/bookings/{id}` | Get booking |
| POST | `/api/bookings` | Create booking |
| PUT | `/api/bookings/{id}` | Update booking |
| DELETE | `/api/bookings/{id}` | Cancel booking |

## Frontend Routes

| Route | Page |
|-------|------|
| `/` | Home |
| `/bookings` | Booking list |
| `/bookings/new` | Create booking |
| `/bookings/:id/edit` | Edit booking |
