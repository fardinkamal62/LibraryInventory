using LibraryInventory.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryInventory.Infrastructure.Data;

public static class DbContextSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        if (!await db.Books.AnyAsync(ct))
        {
            db.Books.AddRange(
                new Book("Clean Code", "Robert C. Martin", "9780132350884", 464, 32.99m, PageTypeEnum.Paperback,
                    new Uri("https://example.com/clean")),
                new Book("The Pragmatic Programmer", "Andrew Hunt, David Thomas", "9780201616224", 352, 29.99m,
                    PageTypeEnum.Paperback, new Uri("https://example.com/pragmatic")));
            await db.SaveChangesAsync(ct);
        }

        if (!await db.Members.AnyAsync(ct))
        {
            db.Members.AddRange(new Member("Alice Johnson", "alice@example.com"),
                new Member("Bob Smith", "bob@example.com"));
            await db.SaveChangesAsync(ct);
        }
    }
}