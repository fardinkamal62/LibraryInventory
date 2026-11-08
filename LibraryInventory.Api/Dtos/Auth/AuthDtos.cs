namespace LibraryInventory.Api.Dtos.Auth;

public record RegisterDto(string MemberName, string Email, string Password);

public record LoginDto(string Email, string Password);

public record AuthResponseDto(string Token, DateTime ExpiresAtUtc, int? MemberId, string Role);
