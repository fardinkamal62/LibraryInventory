using LibraryInventory.Core.Commands;
using LibraryInventory.Core.Common;
using LibraryInventory.Core.Entities;
using LibraryInventory.Core.Interfaces;
using LibraryInventory.Core.Settings;
using LibraryInventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LibraryInventory.Infrastructure.Services;

public class CheckoutService(
    AppDbContext db,
    IDueDateCalculator dueDate,
    IOptions<LibrarySettings> settings,
    TimeProvider time) : ICheckoutService
{
    public async Task<Result<Checkout>> CheckoutAsync(CreateCheckoutCommand c, CancellationToken ct)
    {
        if (!await db.Books.AnyAsync(x => x.BookId == c.BookId, ct))
            return Result<Checkout>.NotFound($"Book {c.BookId} not found");
        if (!await db.Members.AnyAsync(x => x.MemberId == c.MemberId, ct))
            return Result<Checkout>.NotFound($"Member {c.MemberId} not found");
        if (await db.Checkouts.AnyAsync(x => x.BookId == c.BookId && x.ReturnedAtUtc == null, ct))
            return Result<Checkout>.Conflict("Book is already checked out");
        var active = await db.Checkouts.CountAsync(x => x.MemberId == c.MemberId && x.ReturnedAtUtc == null, ct);
        if (active >= settings.Value.MaxBooksPerMember)
            return Result<Checkout>.RuleViolation(
                $"Member already has {active} books checked out (limit {settings.Value.MaxBooksPerMember})");
        var now = time.GetUtcNow().UtcDateTime;
        var checkout = new Checkout(c.BookId, c.MemberId, now, dueDate.Calculate(now));
        db.Checkouts.Add(checkout);
        try
        {
            await db.SaveChangesAsync(ct);
            return Result<Checkout>.Success(checkout, "Book checked out");
        }
        catch (DbUpdateException e)when (e.IsUniqueViolation())
        {
            return Result<Checkout>.Conflict("Book is already checked out");
        }
    }

    public async Task<Result<Checkout>> GetByIdAsync(int id, CancellationToken ct)
    {
        var c = await db.Checkouts.AsNoTracking().FirstOrDefaultAsync(x => x.CheckoutId == id, ct);
        return c is null ? Result<Checkout>.NotFound($"Checkout {id} not found") : Result<Checkout>.Success(c);
    }

    public async Task<Result<Checkout>> ReturnAsync(int id, CancellationToken ct)
    {
        var c = await db.Checkouts.FirstOrDefaultAsync(x => x.CheckoutId == id, ct);
        if (c is null) return Result<Checkout>.NotFound($"Checkout {id} not found");
        if (!c.IsActive) return Result<Checkout>.Conflict("Book was already returned");
        c.MarkReturned(time.GetUtcNow().UtcDateTime);
        await db.SaveChangesAsync(ct);
        return Result<Checkout>.Success(c, "Book returned");
    }

    public async Task<IReadOnlyList<Checkout>> GetOverdueAsync(CancellationToken ct)
    {
        var now = time.GetUtcNow().UtcDateTime;
        return await db.Checkouts.AsNoTracking().Where(x => x.ReturnedAtUtc == null && x.DueDateUtc < now)
            .OrderBy(x => x.DueDateUtc).ToListAsync(ct);
    }
}

public class StandardDueDateCalculator(IOptions<LibrarySettings> s) : IDueDateCalculator
{
    public DateTime Calculate(DateTime t)
    {
        return t.AddDays(s.Value.MaxCheckoutDays);
    }
}

public class ExtendedDueDateCalculator(IOptions<LibrarySettings> s) : IDueDateCalculator
{
    public DateTime Calculate(DateTime t)
    {
        return t.AddDays(s.Value.MaxCheckoutDays * s.Value.ExtendedLoanMultiplier);
    }
}