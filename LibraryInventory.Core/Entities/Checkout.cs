using LibraryInventory.Core.Common;

namespace LibraryInventory.Core.Entities;

public class Checkout : BaseEntity
{
    public int BookId { get; private set; }
    public Book Book { get; private set; } = null!;

    public int MemberId { get; private set; }
    public Member Member { get; private set; } = null!;

    public DateTime CheckedOutAtUtc { get; private set; }
    public DateTime DueDateUtc { get; private set; }
    public DateTime? ReturnedAtUtc { get; private set; }

    private Checkout() { }

    public Checkout(int bookId, int memberId, DateTime checkedOutAtUtc, DateTime dueDateUtc)
    {
        ValidateUtc(checkedOutAtUtc, nameof(checkedOutAtUtc));
        ValidateUtc(dueDateUtc, nameof(dueDateUtc));

        if (dueDateUtc <= checkedOutAtUtc)
            throw new ArgumentException("Due date must be after checkout date", nameof(dueDateUtc));

        BookId = bookId;
        MemberId = memberId;
        CheckedOutAtUtc = checkedOutAtUtc;
        DueDateUtc = dueDateUtc;
    }

    public bool IsActive => ReturnedAtUtc is null;
    public bool IsOverdue => IsActive && DateTime.UtcNow > DueDateUtc;
    public int DaysOverdue => IsOverdue
        ? (int)Math.Ceiling((DateTime.UtcNow - DueDateUtc).TotalDays)
        : 0;

    public void MarkReturned(DateTime nowUtc)
    {
        ValidateUtc(nowUtc, nameof(nowUtc));

        if (ReturnedAtUtc is not null)
            throw new InvalidOperationException("This checkout has already been returned");

        ReturnedAtUtc = nowUtc;
    }

    private static void ValidateUtc(DateTime value, string paramName)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Value must be UTC", paramName);
    }
}
