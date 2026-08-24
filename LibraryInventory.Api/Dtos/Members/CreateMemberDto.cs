namespace LibraryInventory.Dtos.Members;

/// <summary>
/// Used by POST /api/members
/// </summary>
public record CreateMemberDto(
    string MemberName,
    string Email
);
