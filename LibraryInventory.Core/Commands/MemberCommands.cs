namespace LibraryInventory.Core.Commands;

public record CreateMemberCommand(string MemberName, string Email);

public record UpdateMemberCommand(string MemberName, string Email);