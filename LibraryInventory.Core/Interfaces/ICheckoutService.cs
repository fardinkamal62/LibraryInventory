using LibraryInventory.Core.Common;
using LibraryInventory.Core.Entities;

namespace LibraryInventory.Core.Interfaces;

public interface ICheckoutService
{
    Task<Result<Checkout>> CreateAsync(CreateCheckoutCommand command, CancellationToken ct);
}
