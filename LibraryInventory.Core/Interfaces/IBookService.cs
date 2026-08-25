using LibraryInventory.Core.Entities;
using LibraryInventory.Core.Common;

namespace LibraryInventory.Core.Interfaces;

public interface IBookService
{
    Task<Result<Book>> CreateAsync(CreateBookCommand command, CancellationToken ct);
    Task<Result<Book>> GetByIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<Book>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<Result<Book>> UpdateAsync(int id, UpdateBookCommand command, CancellationToken ct);
    Task<OperationResult> DeleteAsync(int id, CancellationToken ct);
}