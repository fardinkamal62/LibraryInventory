using LibraryInventory.Core.Common;

namespace LibraryInventory.Core.Entities;

public class Checkout : BaseEntity
{
    private Checkout()
    {
    }

    public Checkout(int bookId, int memberId, DateTime checkedOutAtUtc, DateTime dueDateUtc)
    {
        if (dueDateUtc <= checkedOutAtUtc)
            throw new ArgumentException("Due date must be after checkout date", nameof(dueDateUtc));
        BookId = bookId;
        MemberId = memberId;
        CheckedOutAtUtc = checkedOutAtUtc;
        DueDateUtc = dueDateUtc;
    }

    public int CheckoutId { get; private set; }
    public int BookId { get; private set; }
    public Book Book { get; private set; } = null!;
    public int MemberId { get; private set; }
    public Member Member { get; private set; } = null!;
    public DateTime CheckedOutAtUtc { get; private set; }
    public DateTime DueDateUtc { get; }
    public DateTime? ReturnedAtUtc { get; private set; }
    public bool IsActive => ReturnedAtUtc is null;

    public bool IsOverdue(DateTime nowUtc)
    {
        return IsActive && DueDateUtc < nowUtc;
    }

    public int DaysOverdue(DateTime nowUtc)
    {
        var end = ReturnedAtUtc ?? nowUtc;
        return end > DueDateUtc ? (int)Math.Floor((end - DueDateUtc).TotalDays) : 0;
    }

    public void MarkReturned(DateTime nowUtc)
    {
        if (ReturnedAtUtc is not null) throw new InvalidOperationException("Already returned");
        ReturnedAtUtc = nowUtc;
    }
}