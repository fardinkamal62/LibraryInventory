namespace LibraryInventory.Core.Interfaces;

public record CreateCheckoutCommand(
    int BookId,
    int MemberId,
    DateTime CheckedOutAtUtc,
    DateTime DueDateUtc
);
