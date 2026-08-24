using LibraryInventory.Core.Common;

namespace LibraryInventory.Core.Entities;

public class CheckoutItem : BaseEntity
{
    public CheckoutItem()
    {
    }

    public CheckoutItem(int bookId)
    {
        BookId = bookId;
    }

    public int CheckoutItemId { get; init; }

    public int CheckoutId { get; private set; }

    public int BookId { get; private set; }

    public Checkout Checkout { get; private set; } = null!;

    public Book Book { get; set; } = null!;
}
