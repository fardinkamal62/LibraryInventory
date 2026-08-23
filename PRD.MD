# Library Inventory API

### Core Requirements

**Resources:** Books, Members, Checkouts

**Endpoints:**

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/api/books` | List all books (with optional `?search=` and `?available=true` filters) |
| GET | `/api/books/{id}` | Get single book with current checkout status |
| POST | `/api/books` | Add new book |
| PUT | `/api/books/{id}` | Update book details |
| DELETE | `/api/books/{id}` | Remove book (only if not currently checked out) |
| GET | `/api/members` | List all members |
| POST | `/api/members` | Register new member |
| POST | `/api/checkouts` | Checkout a book to a member |
| PUT | `/api/checkouts/{id}/return` | Return a book |
| GET | `/api/checkouts/overdue` | List all overdue checkouts |

### What This Forces You to Learn

**SQL & ASP.NET Integration:**
- Design 3 tables: `Books`, `Members`, `Checkouts` with foreign keys
- Write raw SQL or use Entity Framework — your choice. If EF, use migrations
- Handle concurrency: two members can't checkout the same book simultaneously

**Configuration:**
- Connection string in `appsettings.json`
- `appsettings.Development.json` with different DB (SQLite for dev, SQL Server for production-like testing)
- A custom config section for `LibrarySettings` (e.g., `MaxCheckoutDays`, `MaxBooksPerMember`)
- Read config via `IOptions<T>` pattern

**Dependency Injection:**
- `IBookService`, `IMemberService`, `ICheckoutService` as scoped services
- A `IDueDateCalculator` service injected into checkout logic
- At least one singleton service (e.g., `ILoggerService` wrapping `ILogger`)
- Register all services in `Program.cs` using extension methods per feature

**Middleware:**
- Custom logging middleware that logs request path, timestamp, and duration
- Exception handling middleware that returns structured error responses (never raw stack traces)
- Optional: rate-limiting middleware (simple in-memory counter)

**REST Principles:**
- Proper status codes (201 on create, 409 on conflict, 422 on business rule violations)
- Resource-based URLs, not action-based (no `/api/checkoutBook`)
- HATEOAS-lite: return book checkout links in GET `/api/books/{id}` response
- Validation on inputs (book must have ISBN, member must have email)

### Project Structure

```
LibraryInventory/
├── LibraryInventory.Api/           # ASP.NET Core Web API
│   ├── Controllers/
│   ├── Middleware/
│   ├── Extensions/
│   └── Program.cs
├── LibraryInventory.Core/          # Domain models, interfaces
│   ├── Entities/
│   ├── Interfaces/
│   └── Settings/
├── LibraryInventory.Infrastructure/ # Data access, EF, external services
│   ├── Data/
│   ├── Repositories/
│   └── Services/
└── LibraryInventory.sln
```

### Progression Order

1. Models + EF setup + migration + seed data. Get `/api/books` returning real data from SQL.
2. All CRUD endpoints for Books and Members. No checkout logic yet.
3. Checkout + Return logic with business rules. This is where you'll hit concurrency problems.
4. Configuration + `IOptions`. Move hardcoded values to settings.
5. Custom middleware. Replace exception handling and add request logging.
6. Refactor controllers into services with DI. Controllers should be thin.
7. Polish. Status codes, validation, error responses, README.

### Business Rules to Implement

- A member can only have 3 books checked out at once (`MaxBooksPerMember` from config)
- Standard checkout is 14 days (`MaxCheckoutDays` from config)
- A book cannot be deleted if currently checked out (return 409)
- When listing books, mark each as `"available": true/false` based on active checkouts
- Overdue checkouts are those past due date with no return date
- Returning a book calculates and includes `DaysOverdue` in the response

### What Not to Do

- Don't add authentication, yet.
- Don't use AutoMapper or MediatR. Raw DI.
