# Library Inventory API

ASP.NET Core Web API for managing **books**, **members**, and library circulation workflows.  
This repository follows the product scope in `PRD.MD` and implementation steps in `IMPLEMENTATION_PLAN.md`.

## Project Structure

```text
LibraryInventory/
├── LibraryInventory.Api/             # HTTP API (controllers, DTOs, validators, Program.cs)
├── LibraryInventory.Core/            # Domain entities, interfaces, shared result models
├── LibraryInventory.Infrastructure/  # EF Core DbContext, migrations, service implementations
└── LibraryInventory.sln
```

## Tech Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core + SQLite
- FluentValidation
- OpenAPI (development environment)

## Current Implementation Status

| Area | Status | Notes |
|------|--------|-------|
| Book CRUD (`/api/books`) | ✅ Implemented | List/get/create/update/delete with validation and unique ISBN rule |
| Member list (`/api/members`) | ✅ Implemented | Paginated listing endpoint available |
| Member create/update/delete | ⚠️ Partially implemented | Service layer exists; controller endpoints are not fully exposed yet |
| Checkout/return workflow | ❌ Not implemented | Planned from PRD but endpoints/services are pending |
| Overdue reporting | ❌ Not implemented | Planned from PRD but pending |
| Custom middleware (logging/exception handling) | ❌ Not implemented | Planned from PRD but pending |
| `LibrarySettings` via `IOptions<T>` | ❌ Not implemented | Planned configuration enhancement |

## Planned API Scope (from PRD)

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/api/books` | List books (with filtering support in final scope) |
| GET | `/api/books/{id}` | Get a single book |
| POST | `/api/books` | Add a new book |
| PUT | `/api/books/{id}` | Update book details |
| DELETE | `/api/books/{id}` | Delete book (blocked if checked out) |
| GET | `/api/members` | List all members |
| POST | `/api/members` | Register a member |
| POST | `/api/checkouts` | Checkout book(s) to a member |
| PUT | `/api/checkouts/{id}/return` | Return checked out book(s) |
| GET | `/api/checkouts/overdue` | List overdue checkouts |

## Business Rules (Target Behavior)

- A member can check out at most **3** books at once.
- Default checkout period is **14 days**.
- A checked-out book cannot be deleted.
- Availability should be derived from active checkouts.
- Overdue checkouts are active checkouts past due date.
- Return response should include `DaysOverdue`.

## Getting Started

### Prerequisites

- .NET SDK 10+

### Run the API

```bash
dotnet restore
dotnet build
dotnet run --project LibraryInventory.Api
```

On startup, EF Core migrations are applied automatically and seed data is inserted for books and members.

### OpenAPI

In Development, OpenAPI is mapped by the API project. Run locally and open the exposed OpenAPI endpoint from the application URL output.

## Configuration

`LibraryInventory.Api/appsettings.json` includes:

- `ConnectionStrings:DefaultConnection` (SQLite file path: `App_Data/library.db`)

Future iterations will add `LibrarySettings` (e.g., `MaxCheckoutDays`, `MaxBooksPerMember`) and consume them with `IOptions<T>`.
