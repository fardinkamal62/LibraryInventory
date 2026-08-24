using LibraryInventory.Api.Dtos.Books;
using LibraryInventory.Core.Entities;
using LibraryInventory.Core.Common;
using LibraryInventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryInventory.Api.Services;

public class BookService(AppDbContext db) : IBookService
{
    public async Task<Result<Book>> CreateAsync(CreateBookDto dto, CancellationToken ct)
    {
        try
        {
            // Business rule: ISBN must be unique
            var isbnExists = await db.Books.AnyAsync(b => b.IsbnNumber == dto.IsbnNumber, ct);
            if (isbnExists)
                return Result<Book>.Failure($"A book with ISBN {dto.IsbnNumber} already exists");

            var book = new Book(
                dto.BookName,
                dto.Author,
                dto.IsbnNumber,
                dto.PageCount,
                dto.Price,
                dto.PageMaterial,
                dto.PhotoUrl
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
        var book = await db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.BookId == id, ct);
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

    public async Task<Result<Book>> UpdateAsync(int id, UpdateBookDto dto, CancellationToken ct)
    {
        try
        {
            var book = await db.Books.FirstOrDefaultAsync(b => b.BookId == id, ct);
            if (book is null)
                return Result<Book>.Failure($"Book with id {id} not found");

            var isbnExists = await db.Books.AnyAsync(
                b => b.IsbnNumber == dto.IsbnNumber && b.BookId != id, ct);
            if (isbnExists)
                return Result<Book>.Failure($"A book with ISBN {dto.IsbnNumber} already exists");

            book.UpdateDetails(dto.IsbnNumber, dto.BookName, dto.PageMaterial, dto.PhotoUrl, dto.PageCount);
            book.ChangePrice(dto.Price);

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
        var book = await db.Books.FirstOrDefaultAsync(b => b.BookId == id, ct);
        if (book is null)
            return OperationResult.Failure($"Book with id {id} not found");

        var hasActiveCheckouts = await db.CheckoutItems
            .AnyAsync(ci => ci.BookId == id && ci.Checkout.ReturnedAt == null, ct);
        if (hasActiveCheckouts)
            return OperationResult.Failure("Cannot delete a book that is currently checked out");

        db.Books.Remove(book);
        await db.SaveChangesAsync(ct);
        return OperationResult.Success("Book deleted");
    }
}
