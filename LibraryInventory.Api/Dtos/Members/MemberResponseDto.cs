namespace LibraryInventory.Dtos.Members;

public record MemberResponseDto(
    int MemberId,
    string MemberName,
    Uri PhotoUrl
);
