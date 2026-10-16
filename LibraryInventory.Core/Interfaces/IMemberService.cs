using LibraryInventory.Core.Commands; using LibraryInventory.Core.Common; using LibraryInventory.Core.Entities;
namespace LibraryInventory.Core.Interfaces;
public interface IMemberService { Task<Result<Member>> CreateAsync(CreateMemberCommand c,CancellationToken ct); Task<Result<Member>> GetByIdAsync(int id,CancellationToken ct); Task<PagedResult<Member>> GetAllAsync(int page,int pageSize,CancellationToken ct); Task<Result<Member>> UpdateAsync(int id,UpdateMemberCommand c,CancellationToken ct); Task<OperationResult> DeleteAsync(int id,CancellationToken ct); }
