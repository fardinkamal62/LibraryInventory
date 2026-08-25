using LibraryInventory.Core.Entities;

namespace LibraryInventory.Core.Interfaces;

/// <summary>
/// Core-friendly request payloads for book operations. Decoupled from Api DTOs
/// so the service contract doesn't depend on the HTTP layer.
/// </summary>
public record CreateMemberCommand(
    string MemberName,
    string Email
);

public record UpdateMemberCommand(
    int MemberId,
    string MemberName,
    string Email,
    Uri PhotoUrl,
    string MemberAddress
);
