namespace LibraryInventory.Core.Common;
public abstract class BaseEntity
{
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
