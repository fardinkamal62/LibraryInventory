namespace LibraryInventory.Core.Common;

public abstract class BaseEntity
{
    public int Id { get; protected set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
