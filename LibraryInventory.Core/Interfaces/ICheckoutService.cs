using LibraryInventory.Core.Commands; using LibraryInventory.Core.Common; using LibraryInventory.Core.Entities;
namespace LibraryInventory.Core.Interfaces;
public interface ICheckoutService { Task<Result<Checkout>> CheckoutAsync(CreateCheckoutCommand c,CancellationToken ct); Task<Result<Checkout>> GetByIdAsync(int id,CancellationToken ct); Task<Result<Checkout>> ReturnAsync(int id,CancellationToken ct); Task<IReadOnlyList<Checkout>> GetOverdueAsync(CancellationToken ct); }
