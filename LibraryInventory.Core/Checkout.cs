using LibraryInventory.Core.Common;

namespace LibraryInventory.Core;

public class Checkout : BaseEntity
{
    public int CheckOutId { get; init; }

    public DateTimeOffset CheckedOutAt { get; init; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? ReturnedAt { get; private set; }

    // Foreign key
    public int MemberId { get; private set; }

    // Navigation properties
    public Member Member { get; private set; } = null!;

    public ICollection<CheckoutItem> Items { get; private set; } = [];

    public ICollection<Book> Books => Items.Select(i => i.Book).ToList();

    // EF Core needs a parameterless constructor
    private Checkout() { }

    public Checkout(int memberId, IEnumerable<Book> books)
    {
        if (books is null || !books.Any())
            throw new ArgumentException("At least one book must be checked out", nameof(books));

        MemberId = memberId;
        Items = books.Select(b => new CheckoutItem(b.BookId, b)).ToList();
    }

    // Domain behaviour
    public void MarkReturned()
    {
        if (ReturnedAt is not null)
            throw new InvalidOperationException("This checkout has already been returned");
        ReturnedAt = DateTimeOffset.UtcNow;
    }

    public bool IsActive => ReturnedAt is null;
}
