using Microsoft.EntityFrameworkCore;
using LibraryInventory.Core.Common;
using LibraryInventory.Core.Entities;
using LibraryInventory.Core.Interfaces;
using LibraryInventory.Infrastructure.Data;

namespace LibraryInventory.Infrastructure.Services;

public class MemberService(AppDbContext db) : IMemberService
{
    public async Task<Result<Member>> CreateAsync(CreateMemberCommand command, CancellationToken ct)
    {
        try
        {
            var exists = await db.Members.AnyAsync(m => m.MemberEmail == command.Email, ct);
            if (exists)
                return Result<Member>.Failure($"A member with email {command.Email} already exists");

            var member = new Member(command.MemberName, command.Email);
            db.Members.Add(member);
            await db.SaveChangesAsync(ct);
            return Result<Member>.Success(member, "Member registered");
        }
        catch (ArgumentException ex)
        {
            return Result<Member>.Failure(ex.Message);
        }
    }

    public async Task<IReadOnlyList<Member>> GetAllAsync(int page, int pageSize, CancellationToken ct) =>
        await db.Members.AsNoTracking().Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

    public async Task<Result<Member>> GetByIdAsync(int id, CancellationToken ct)
    {
        var member = await db.Members.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, ct);
        return member is null
            ? Result<Member>.Failure($"Member with id {id} not found")
            : Result<Member>.Success(member);
    }

    public async Task<Result<Member>> UpdateAsync(int id, UpdateMemberCommand command, CancellationToken ct)
    {
        try
        {
            var member = await db.Members.FirstOrDefaultAsync(m => m.Id == id, ct);
            if (member is null) return await Task.FromResult(Result<Member>.Failure($"Member with id {id} not found"));

            // Business rule: email must be unique
            var emailExists = await db.Members.AnyAsync(m => m.MemberEmail == command.Email && m.Id != id, ct);
            if (emailExists)
                return await Task.FromResult(
                    Result<Member>.Failure($"A member with email {command.Email} already exists"));

            member.UpdateDetails(command.MemberName, command.Email, command.PhotoUrl, command.MemberAddress);
            await db.SaveChangesAsync(ct);
            return await Task.FromResult(Result<Member>.Success(member, "Member updated"));
        }
        catch (ArgumentException ex)
        {
            return await Task.FromResult(Result<Member>.Failure(ex.Message));
        }
    }

    public async Task<OperationResult> DeleteAsync(int id, CancellationToken ct)
    {
        try
        {
            var member = await db.Members.FirstOrDefaultAsync(m => m.Id == id, ct);
            if (member is null) return await Task.FromResult(OperationResult.Failure($"Member with id {id} not found"));

            // Business rule: cannot delete member with active checkouts
            var hasActive = await db.Checkouts.AnyAsync(c => c.MemberId == id && c.ReturnedAt == null, ct);
            if (hasActive) return await Task.FromResult(OperationResult.Failure("Cannot delete member with active checkouts"));

            db.Members.Remove(member);
            await db.SaveChangesAsync(ct);
            return await Task.FromResult(OperationResult.Success("Member deleted"));
        }
        catch (Exception ex)
        {
            return await Task.FromResult(OperationResult.Failure($"Error deleting member: {ex.Message}"));
        }
    }
}
