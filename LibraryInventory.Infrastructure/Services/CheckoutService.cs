using LibraryInventory.Core.Common;
using LibraryInventory.Core.Entities;
using LibraryInventory.Core.Interfaces;
using LibraryInventory.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LibraryInventory.Infrastructure.Services;

public class CheckoutService(AppDbContext db) : ICheckoutService
{
    public async Task<Result<Checkout>> CreateAsync(
        CreateCheckoutCommand command,
        CancellationToken ct)
    {
        var memberExists = await db.Members.AnyAsync(m => m.Id == command.MemberId, ct);
        if (!memberExists)
            return Result<Checkout>.Failure($"Member with id {command.MemberId} not found");

        var bookExists = await db.Books.AnyAsync(b => b.Id == command.BookId, ct);
        if (!bookExists)
            return Result<Checkout>.Failure($"Book with id {command.BookId} not found");

        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        var rowsUpdated = await db.Books
            .Where(b => b.Id == command.BookId && b.AvailableCopies > 0)
            .ExecuteUpdateAsync(
                updates => updates.SetProperty(
                    b => b.AvailableCopies,
                    b => b.AvailableCopies - 1),
                ct);

        if (rowsUpdated == 0)
            return Result<Checkout>.Failure("No copies available");

        try
        {
            var checkout = new Checkout(
                command.BookId,
                command.MemberId,
                command.CheckedOutAtUtc,
                command.DueDateUtc);

            db.Checkouts.Add(checkout);
            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return Result<Checkout>.Success(checkout, "Book checked out");
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            await transaction.RollbackAsync(ct);
            return Result<Checkout>.Failure("Member already has this book checked out");
        }
    }

    private static bool IsUniqueViolation(DbUpdateException exception) =>
        exception.InnerException is SqliteException
        {
            SqliteErrorCode: 19
        };
}
