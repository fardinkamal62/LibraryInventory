# Library Inventory API

ASP.NET Core Web API for books, members, and library circulation. The implementation follows
`IMPLEMENTATION_PLAN.md` and uses Clean Architecture with EF Core, SQLite, ASP.NET Core Identity,
and JWT authentication.

## Project structure

```text
LibraryInventory/
├── LibraryInventory.Api/
│   ├── Controllers/       # HTTP endpoints and status-code mapping
│   ├── Dtos/              # Request and response contracts
│   ├── Extensions/        # DI, middleware, and result helpers
│   ├── Middleware/        # Request logging and exception handling
│   ├── Validators/        # FluentValidation validators
│   └── Program.cs
├── LibraryInventory.Core/
│   ├── Commands/          # Service input models
│   ├── Common/            # Result, paging, roles, and shared types
│   ├── Entities/          # Book, Member, and Checkout
│   ├── Interfaces/        # Service contracts
│   └── Settings/          # LibrarySettings and JwtSettings
├── LibraryInventory.Infrastructure/
│   ├── Data/              # DbContext, seeding, and database helpers
│   ├── Identity/          # ApplicationUser
│   ├── Migrations/        # EF Core migrations
│   └── Services/          # Service implementations and due-date policies
├── LibraryInventory.Tests/ # xUnit unit and integration tests
└── LibraryInventory.sln
```

```text
HTTP request
    |
    v
Api controllers, authentication, and middleware
    |
    v
Core interfaces, commands, and entities
    |
    v
Infrastructure services, Identity, and AppDbContext
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

### JWT signing key (required)

The signing key is never committed. It is read from user secrets in Development or from an
environment variable elsewhere:

```bash
dotnet user-secrets set "Jwt:Key" "$(openssl rand -base64 48)" --project LibraryInventory.Api
```

In production, set `Jwt__Key` (at least 32 characters) in the environment.

### Optional Development librarian

In Development the app seeds a librarian account when these secrets are present:

```bash
dotnet user-secrets set "Seed:LibrarianEmail" "librarian@library.local" --project LibraryInventory.Api
dotnet user-secrets set "Seed:LibrarianPassword" "Librarian#123" --project LibraryInventory.Api
```

### Database and migrations

Apply migrations explicitly with:

```bash
dotnet ef database update \
  --project LibraryInventory.Infrastructure \
  --startup-project LibraryInventory.Api
```

The API also applies pending migrations at startup. In Development it seeds example books,
members, roles, and (when configured) the librarian account. Run it with:

```bash
dotnet run --project LibraryInventory.Api
```

The default connection is `App_Data/library.db`. The examples below assume
`http://localhost:5043`; use the URL printed by `dotnet run` if it differs.

## Authentication and authorization

Two roles exist:

| Role | Permissions |
|---|---|
| Librarian | Everything: book and member management, all checkouts, returns, overdue list |
| Member | Read books, check out books for themselves, read only their own checkouts |

Endpoints return `401` when a token is missing or invalid and `403` when the signed-in user lacks
the required role. Register and log in first:

```bash
# Register a member (returns a JWT and the linked memberId)
curl -X POST http://localhost:5043/api/auth/register \
  -H 'Content-Type: application/json' \
  -d '{"memberName":"Carol Jones","email":"carol@example.com","password":"Member#1234"}'

# Log in
curl -X POST http://localhost:5043/api/auth/login \
  -H 'Content-Type: application/json' \
  -d '{"email":"carol@example.com","password":"Member#1234"}'
```

Both responses look like:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiresAtUtc": "2026-11-08T12:00:00Z",
  "memberId": 3,
  "role": "Member"
}
```

Send protected requests with `-H "Authorization: Bearer <token>"`.

## Status codes

| Code | Meaning |
|---|---|
| 200 | Successful read, update, login, register, return, or overdue query |
| 201 | Successful create, including a `Location` header |
| 204 | Successful delete |
| 400 | Request-shape or input validation failed |
| 401 | Missing, invalid, or expired token; wrong credentials |
| 403 | Authenticated but not allowed (wrong role, another member's checkout) |
| 404 | Referenced resource does not exist |
| 409 | Conflict, such as a duplicate ISBN, duplicate email, active checkout, or delete with checkout history |
| 422 | Semantically invalid domain data or business rule violation, such as an invalid entity value or member checkout limit |
| 500 | Unexpected exception, returned by exception-handling middleware |

## API endpoints

JSON uses the default ASP.NET Core camel-case contract.

| Method | Route | Auth | Success | Common failure statuses |
|---|---|---|---|---|
| POST | `/api/auth/register` | anonymous | 200 | 400, 409, 422 |
| POST | `/api/auth/login` | anonymous | 200 | 400, 401 |
| GET | `/api/books` | anonymous | 200 | - |
| GET | `/api/books/{id}` | anonymous | 200 | 404 |
| POST | `/api/books` | Librarian | 201 | 400, 401, 403, 409, 422 |
| PUT | `/api/books/{id}` | Librarian | 200 | 400, 401, 403, 404, 422 |
| DELETE | `/api/books/{id}` | Librarian | 204 | 401, 403, 404, 409 |
| GET | `/api/members` | Librarian | 200 | 401, 403 |
| GET | `/api/members/{id}` | Librarian | 200 | 401, 403, 404 |
| POST | `/api/members` | Librarian | 201 | 400, 401, 403, 409, 422 |
| PUT | `/api/members/{id}` | Librarian | 200 | 400, 401, 403, 404, 422 |
| DELETE | `/api/members/{id}` | Librarian | 204 | 401, 403, 404, 409 |
| POST | `/api/checkouts` | Librarian or owning Member | 201 | 400, 401, 403, 404, 409, 422 |
| GET | `/api/checkouts/{id}` | Librarian or owning Member | 200 | 401, 403, 404 |
| PUT | `/api/checkouts/{id}/return` | Librarian | 200 | 401, 403, 404, 409 |
| GET | `/api/checkouts/overdue` | Librarian | 200 | 401, 403 |

### Books

```bash
curl 'http://localhost:5043/api/books?search=clean&available=true&page=1&pageSize=20'
curl http://localhost:5043/api/books/1
curl -X POST http://localhost:5043/api/books \
  -H "Authorization: Bearer $TOKEN" \
  -H 'Content-Type: application/json' \
  -d '{"bookName":"Domain-Driven Design","author":"Eric Evans","isbnNumber":"9780321125217","pageCount":560,"price":49.99,"pageMaterial":1,"photoUrl":"https://example.com/ddd"}'
curl -X PUT http://localhost:5043/api/books/1 \
  -H "Authorization: Bearer $TOKEN" \
  -H 'Content-Type: application/json' \
  -d '{"bookName":"Clean Code","author":"Robert C. Martin","pageCount":464,"price":32.99,"pageMaterial":0,"photoUrl":"https://example.com/clean"}'
curl -X DELETE http://localhost:5043/api/books/1 -H "Authorization: Bearer $TOKEN"
```

`GET /api/books/{id}` includes `self`, `update`, and `delete` links. It includes a `checkout`
link only while the book is available.

### Members

```bash
curl 'http://localhost:5043/api/members?page=1&pageSize=20' -H "Authorization: Bearer $TOKEN"
curl http://localhost:5043/api/members/1 -H "Authorization: Bearer $TOKEN"
curl -X POST http://localhost:5043/api/members \
  -H "Authorization: Bearer $TOKEN" \
  -H 'Content-Type: application/json' \
  -d '{"memberName":"Carol Jones","email":"carol@example.com"}'
curl -X PUT http://localhost:5043/api/members/1 \
  -H "Authorization: Bearer $TOKEN" \
  -H 'Content-Type: application/json' \
  -d '{"memberName":"Carol Jones","email":"carol@example.com"}'
curl -X DELETE http://localhost:5043/api/members/1 -H "Authorization: Bearer $TOKEN"
```

### Checkouts

```bash
curl -X POST http://localhost:5043/api/checkouts \
  -H "Authorization: Bearer $TOKEN" \
  -H 'Content-Type: application/json' \
  -d '{"bookId":1,"memberId":1}'
curl http://localhost:5043/api/checkouts/1 -H "Authorization: Bearer $TOKEN"
curl -X PUT http://localhost:5043/api/checkouts/1/return -H "Authorization: Bearer $TOKEN"
curl http://localhost:5043/api/checkouts/overdue -H "Authorization: Bearer $TOKEN"
```

## Business rules

- A member may have at most `MaxBooksPerMember` active checkouts; the default is 3.
- A standard checkout lasts `MaxCheckoutDays`; the default is 14 days.
- The due-date policy can be `Standard` or `Extended`, configured through `LibrarySettings`.
- A book cannot have more than one active checkout, enforced by a filtered unique index.
- A book cannot be deleted if it has checkout history.
- A member cannot be deleted if it has checkout history.
- Book availability is derived from active checkouts.
- An overdue checkout has no return date and a due date before the current UTC time.
- Return responses include `DaysOverdue`.
- ISBNs and member email addresses are unique.
- A member account is linked to its `Member` row through a `memberId` claim in the JWT.

## Tests

```bash
dotnet test
```

The suite contains unit tests (entity rules, due-date strategies, `CheckoutService` against
SQLite in-memory) and integration tests using `WebApplicationFactory` with a temporary database:

- authentication and role checks (401/403/201),
- checkout/return flow and availability changes,
- member checkout limit (422),
- ten parallel checkouts of one book yielding exactly one success and nine conflicts.

## Known limitations

- The member checkout-limit check is a read-then-insert operation, so simultaneous requests for
  the same member can temporarily exceed the configured limit. The database unique index protects
  the one-active-checkout-per-book rule, but SQLite has no equivalent aggregate constraint for a
  member's total active checkouts.
- SQLite serializes concurrent writers and is intended here for local development and learning,
  not high-throughput production workloads.
- JWT signing keys are stored in user secrets or environment variables only; there is no refresh
  token, revocation list, or token rotation.
- The optional seeded librarian is Development-only and is skipped unless both seed secrets exist.

## Implementation status

| Phase | What | Status |
|---|---|---|
| 0 | Folder restructure | Done |
| 1 | EF setup, migration, seed | Done |
| 2 | Books and Members CRUD | Done |
| 2.5 | Cleanup: entities, schema, Result type, services | Done |
| 2.6 | Member endpoints, search, availability, paging | Done |
| 3 | Checkout and return, concurrency-safe | Done |
| 4 | Configuration with IOptions | Done |
| 5 | Custom middleware | Done |
| 6 | DI cleanup | Done |
| 7 | Polish: status codes, HATEOAS, README | Done |
| 8 | Tests, including the concurrency test | Done |
| 9 | Docker and SQL Server | Not started |
| 10 | ASP.NET Core Identity | Done |
| 11 | Redis and Worker Service | Not started |
