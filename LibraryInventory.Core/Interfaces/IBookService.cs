using LibraryInventory.Core.Commands; using LibraryInventory.Core.Common;
namespace LibraryInventory.Core.Interfaces;
public interface IBookService { Task<Result<BookWithAvailability>> CreateAsync(CreateBookCommand c,CancellationToken ct); Task<Result<BookWithAvailability>> GetByIdAsync(int id,CancellationToken ct); Task<PagedResult<BookWithAvailability>> GetAllAsync(BookQuery q,CancellationToken ct); Task<Result<BookWithAvailability>> UpdateAsync(int id,UpdateBookCommand c,CancellationToken ct); Task<OperationResult> DeleteAsync(int id,CancellationToken ct); }
