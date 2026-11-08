using LibraryInventory.Core.Commands;
using LibraryInventory.Core.Common;

namespace LibraryInventory.Core.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResult>> RegisterAsync(RegisterCommand command, CancellationToken ct);
    Task<Result<AuthResult>> LoginAsync(LoginCommand command, CancellationToken ct);
}
