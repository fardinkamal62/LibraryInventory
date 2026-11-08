# Library Inventory API

ASP.NET Core Web API for books, members, and library circulation. The implementation follows
`PRD.MD` and uses Clean Architecture with EF Core and SQLite.

## Project structure

```text
LibraryInventory/
├── LibraryInventory.Api/
│   ├── Controllers/       # HTTP endpoints and status-code mapping
│   ├── Dtos/              # Request and response contracts
│   ├── Extensions/        # DI and result helpers
│   ├── Middleware/        # Request logging and exception handling
│   └── Program.cs
├── LibraryInventory.Core/
│   ├── Commands/          # Service input models
│   ├── Common/            # Result, paging, and shared types
│   ├── Entities/          # Book, Member, and Checkout
│   ├── Interfaces/        # Service contracts
│   └── Settings/          # LibrarySettings
├── LibraryInventory.Infrastructure/
│   ├── Data/              # DbContext, seeding, and database helpers
│   ├── Migrations/        # EF Core migrations
│   └── Services/          # Service implementations and due-date policies
└── LibraryInventory.sln
```

```text
HTTP request
    |
    v
Api controllers and middleware
    |
    v
Core interfaces, commands, and entities
    |
    v
Infrastructure services and AppDbContext
    |
    v
SQLite (App_Data/library.db)
```

Controllers contain HTTP plumbing only. Business rules live in the infrastructure services
behind interfaces defined by the Core project.

## Requirements and setup

- .NET SDK 10+

```bash
dotnet restore
dotnet build
```

Apply migrations explicitly with:

```bash
dotnet ef database update \
  --project LibraryInventory.Infrastructure \
  --startup-project LibraryInventory.Api
```

The API also applies pending migrations at startup. In Development it seeds example books and
members. Run it with:

```bash
dotnet run --project LibraryInventory.Api
```

The default connection is `App_Data/library.db`. The examples below assume
`http://localhost:5043`; use the URL printed by `dotnet run` if it differs.

## Status codes

| Code | Meaning |
|---|---|
| 200 | Successful read, update, return, or overdue query |
| 201 | Successful create, including a `Location` header |
| 204 | Successful delete |
| 400 | Request-shape or input validation failed |
| 404 | Referenced resource does not exist |
| 409 | Conflict, such as a duplicate ISBN, duplicate email, active checkout, or delete with checkout history |
| 422 | Semantically invalid domain data or business rule violation, such as an invalid entity value or member checkout limit |
| 500 | Unexpected exception, returned by exception-handling middleware |

## API endpoints

Every endpoint below is implemented by the API. JSON uses the default ASP.NET Core camel-case
contract.

| Method | Route | Success | Common failure statuses |
|---|---|---|---|
| GET | `/api/books` | 200 | - |
| GET | `/api/books/{id}` | 200 | 404 |
| POST | `/api/books` | 201 | 400, 409, 422 |
| PUT | `/api/books/{id}` | 200 | 400, 404, 422 |
| DELETE | `/api/books/{id}` | 204 | 404, 409 |
| GET | `/api/members` | 200 | - |
| GET | `/api/members/{id}` | 200 | 404 |
| POST | `/api/members` | 201 | 400, 409, 422 |
| PUT | `/api/members/{id}` | 200 | 400, 404, 422 |
| DELETE | `/api/members/{id}` | 204 | 404, 409 |
| POST | `/api/checkouts` | 201 | 400, 404, 409, 422 |
| GET | `/api/checkouts/{id}` | 200 | 404 |
| PUT | `/api/checkouts/{id}/return` | 200 | 404, 409 |
| GET | `/api/checkouts/overdue` | 200 | - |

### Books

```bash
curl 'http://localhost:5043/api/books?search=clean&available=true&page=1&pageSize=20'
curl http://localhost:5043/api/books/1
curl -X POST http://localhost:5043/api/books \
  -H 'Content-Type: application/json' \
  -d '{"bookName":"Domain-Driven Design","author":"Eric Evans","isbnNumber":"9780321125217","pageCount":560,"price":49.99,"pageMaterial":1,"photoUrl":"https://example.com/ddd"}'
curl -X PUT http://localhost:5043/api/books/1 \
  -H 'Content-Type: application/json' \
  -d '{"bookName":"Clean Code","author":"Robert C. Martin","pageCount":464,"price":32.99,"pageMaterial":0,"photoUrl":"https://example.com/clean"}'
curl -X DELETE http://localhost:5043/api/books/1
```

`GET /api/books/{id}` includes `self`, `update`, and `delete` links. It includes a `checkout`
link only while the book is available.

### Members

```bash
curl 'http://localhost:5043/api/members?page=1&pageSize=20'
curl http://localhost:5043/api/members/1
curl -X POST http://localhost:5043/api/members \
  -H 'Content-Type: application/json' \
  -d '{"memberName":"Carol Jones","email":"carol@example.com"}'
curl -X PUT http://localhost:5043/api/members/1 \
  -H 'Content-Type: application/json' \
  -d '{"memberName":"Carol Jones","email":"carol@example.com"}'
curl -X DELETE http://localhost:5043/api/members/1
```

### Checkouts

```bash
curl -X POST http://localhost:5043/api/checkouts \
  -H 'Content-Type: application/json' \
  -d '{"bookId":1,"memberId":1}'
curl http://localhost:5043/api/checkouts/1
curl -X PUT http://localhost:5043/api/checkouts/1/return
curl http://localhost:5043/api/checkouts/overdue
```

## Business rules

- A member may have at most `MaxBooksPerMember` active checkouts; the default is 3.
- A standard checkout lasts `MaxCheckoutDays`; the default is 14 days.
- The due-date policy can be `Standard` or `Extended`, configured through `LibrarySettings`.
- A book cannot have more than one active checkout.
- A book cannot be deleted if it has checkout history.
- A member cannot be deleted if it has checkout history.
- Book availability is derived from active checkouts.
- An overdue checkout has no return date and a due date before the current UTC time.
- Return responses include `DaysOverdue`.
- ISBNs and member email addresses are unique.

## Known limitations

- The member checkout-limit check is a read-then-insert operation, so simultaneous requests for
  the same member can temporarily exceed the configured limit. The database unique index protects
  the one-active-checkout-per-book rule, but SQLite has no equivalent aggregate constraint for a
  member's total active checkouts.
- SQLite serializes concurrent writers and is intended here for local development and learning,
  not high-throughput production workloads.
- Authentication and authorization are intentionally not implemented yet.
