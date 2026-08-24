using LibraryInventory.Api.Dtos.Books;
using LibraryInventory.Core.Entities;
using LibraryInventory.Core.Common;

namespace LibraryInventory.Api.Services;

public interface IBookService
{
    Task<Result<Book>> CreateAsync(CreateBookDto dto, CancellationToken ct);
    Task<Result<Book>> GetByIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<Book>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<Result<Book>> UpdateAsync(int id, UpdateBookDto dto, CancellationToken ct);
    Task<OperationResult> DeleteAsync(int id, CancellationToken ct);
}
