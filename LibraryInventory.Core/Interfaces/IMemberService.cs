using LibraryInventory.Core.Entities;
using LibraryInventory.Core.Common;

namespace LibraryInventory.Core.Interfaces;

public interface IMemberService
{
    Task<Result<Member>> CreateAsync(CreateMemberCommand command, CancellationToken ct);
    Task<Result<Member>> GetByIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<Member>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<Result<Member>> UpdateAsync(int id, UpdateMemberCommand command, CancellationToken ct);
    Task<OperationResult> DeleteAsync(int id, CancellationToken ct);
}
