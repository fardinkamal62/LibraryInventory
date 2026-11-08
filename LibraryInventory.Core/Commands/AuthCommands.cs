namespace LibraryInventory.Core.Commands;

public record RegisterCommand(string MemberName, string Email, string Password);

public record LoginCommand(string Email, string Password);

public record AuthResult(string Token, DateTime ExpiresAtUtc, int? MemberId, string Role);
