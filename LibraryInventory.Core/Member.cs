using LibraryInventory.Core.Common;

namespace LibraryInventory.Core;

public class Member(string memberName, string email) : BaseEntity
{
    public int MemberId { get; init; }

    public string MemberName { get; private set; } = memberName;

    public string Email { get; private set; } = email;

    public void UpdateContact(string email, string memberName)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));
        if (string.IsNullOrWhiteSpace(memberName))
            throw new ArgumentException("Member name is required", nameof(memberName));

        Email = email;
        MemberName = memberName;
    }
}
