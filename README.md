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
| Member endpoints (`/api/members`) | ✅ Implemented | Paged list, get, create, update and delete |
| Checkout/return workflow | ✅ Implemented | Database-enforced one active checkout per book |
| Overdue reporting | ✅ Implemented | `/api/checkouts/overdue` |
| Book search/availability/paging | ✅ Implemented | `search`, `available`, `page`, `pageSize` |
| `LibrarySettings` via `IOptions<T>` | ✅ Implemented | Standard/Extended due-date strategies |

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

## API examples

With the API running at `http://localhost:5043`:

```bash
curl http://localhost:5043/api/books
curl 'http://localhost:5043/api/books?search=clean&available=true&page=1&pageSize=20'
curl http://localhost:5043/api/books/1
curl -X POST http://localhost:5043/api/books -H 'Content-Type: application/json' \
  -d '{"bookName":"Domain-Driven Design","author":"Eric Evans","isbnNumber":"9780321125217","pageCount":560,"price":49.99,"pageMaterial":1,"photoUrl":"https://example.com/ddd"}'
curl -X PUT http://localhost:5043/api/books/1 -H 'Content-Type: application/json' \
  -d '{"bookName":"Clean Code","author":"Robert C. Martin","pageCount":464,"price":32.99,"pageMaterial":0,"photoUrl":"https://example.com/clean"}'
curl -X DELETE http://localhost:5043/api/books/1
curl 'http://localhost:5043/api/members?page=1&pageSize=20'
curl http://localhost:5043/api/members/1
curl -X POST http://localhost:5043/api/members -H 'Content-Type: application/json' \
  -d '{"memberName":"Carol Jones","email":"carol@example.com"}'
curl -X PUT http://localhost:5043/api/members/1 -H 'Content-Type: application/json' \
  -d '{"memberName":"Carol Jones","email":"carol@example.com"}'
curl -X DELETE http://localhost:5043/api/members/1
curl -X POST http://localhost:5043/api/checkouts -H 'Content-Type: application/json' \
  -d '{"bookId":1,"memberId":1}'
curl http://localhost:5043/api/checkouts/1
curl -X PUT http://localhost:5043/api/checkouts/1/return
curl http://localhost:5043/api/checkouts/overdue
```

## Configuration

`LibraryInventory.Api/appsettings.json` includes:

- `ConnectionStrings:DefaultConnection` (SQLite file path: `App_Data/library.db`)

`LibrarySettings` is bound from configuration with `MaxCheckoutDays`, `MaxBooksPerMember`, `DueDatePolicy` (`Standard` or `Extended`) and `ExtendedLoanMultiplier`.

### Business-rule limitations

The member checkout limit is a count check and can be exceeded by simultaneous requests; the filtered unique SQLite index guarantees only one active checkout per book. SQLite also serializes concurrent writers. Apply migrations with `dotnet ef database update --project LibraryInventory.Infrastructure --startup-project LibraryInventory.Api` (the API applies them automatically at startup).
