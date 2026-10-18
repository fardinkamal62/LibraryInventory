namespace LibraryInventory.Api.Dtos.Members;

public record MemberResponseDto(int MemberId, string MemberName, string Email);

public record UpdateMemberDto(string MemberName, string Email);