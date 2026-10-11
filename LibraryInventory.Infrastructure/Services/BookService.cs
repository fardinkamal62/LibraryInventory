using LibraryInventory.Core.Entities;
using LibraryInventory.Core.Common;
using LibraryInventory.Core.Interfaces;
using LibraryInventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryInventory.Infrastructure.Services;

public class BookService(AppDbContext db) : IBookService
{
    public async Task<Result<Book>> CreateAsync(CreateBookCommand command, CancellationToken ct)
    {
        try
        {
            // Business rule: ISBN must be unique
            var isbnExists = await db.Books.AnyAsync(b => b.IsbnNumber == command.IsbnNumber, ct);
            if (isbnExists)
                return Result<Book>.Failure($"A book with ISBN {command.IsbnNumber} already exists");

            var book = new Book(
                command.BookName,
                command.Author,
                command.IsbnNumber,
                command.PageCount,
                command.Price,
                command.PageMaterial,
                command.PhotoUrl,
                command.TotalCopies
            );

            db.Books.Add(book);
            await db.SaveChangesAsync(ct);

            return Result<Book>.Success(book, "Book created");
        }
        catch (ArgumentException ex)
        {
            return Result<Book>.Failure(ex.Message);
        }
    }

    public async Task<Result<Book>> GetByIdAsync(int id, CancellationToken ct)
    {
        var book = await db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, ct);
        if (book is null)
            return Result<Book>.Failure($"Book with id {id} not found");
        return Result<Book>.Success(book);
    }

    public async Task<IReadOnlyList<Book>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        return await db.Books
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<Result<Book>> UpdateAsync(int id, UpdateBookCommand command, CancellationToken ct)
    {
        try
        {
            var book = await db.Books.FirstOrDefaultAsync(b => b.Id == id, ct);
            if (book is null)
                return Result<Book>.Failure($"Book with id {id} not found");

            var isbnExists = await db.Books.AnyAsync(
                b => b.IsbnNumber == command.IsbnNumber && b.Id != id, ct);
            if (isbnExists)
                return Result<Book>.Failure($"A book with ISBN {command.IsbnNumber} already exists");

            var onLoan = book.TotalCopies - book.AvailableCopies;
            if (command.TotalCopies < onLoan)
                return Result<Book>.Failure($"Cannot set total copies to {command.TotalCopies} because {onLoan} copies are currently on loan");

            var newAvailableCopies = command.TotalCopies - onLoan;
            book.UpdateDetails(command.BookName, command.PageMaterial, command.PhotoUrl, command.PageCount, command.Price, command.TotalCopies, newAvailableCopies);
            book.ChangePrice(command.Price);

            await db.SaveChangesAsync(ct);
            return Result<Book>.Success(book, "Book updated");
        }
        catch (ArgumentException ex)
        {
            return Result<Book>.Failure(ex.Message);
        }
    }

    public async Task<OperationResult> DeleteAsync(int id, CancellationToken ct)
    {
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == id, ct);
        if (book is null)
            return OperationResult.Failure($"Book with id {id} not found");

        if (book.AvailableCopies != book.TotalCopies)
            return OperationResult.Failure("Cannot delete a book with active checkouts");

        db.Books.Remove(book);
        await db.SaveChangesAsync(ct);
        return OperationResult.Success("Book deleted");
    }
}
