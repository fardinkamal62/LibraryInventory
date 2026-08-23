using LibraryInventory.Core.Common;

namespace LibraryInventory.Core;

public class CheckoutItem(int bookId, Book book) : BaseEntity
{
    public int CheckoutItemId { get; init; }

    public int CheckoutId { get; private set; }

    public int BookId { get; private set; } = bookId;

    public Checkout Checkout { get; private set; } = null!;

    public Book Book { get; private set; } = book;
}

