using LibraryInventory.Core.Common;

namespace LibraryInventory.Core.Entities;


public class Member(string memberName, string memberEmail, Uri photoUrl) : BaseEntity
{
    public int MemberId { get; init; }

    public string MemberName { get; private set; } = memberName;

    public string memberEmail { get; private set; } = memberEmail;

    public Uri PhotoUrl { get; private set; } = photoUrl;

    public void UpdateDetails(string memberName, string memberEmail, Uri photoUrl)
    {
        if (string.IsNullOrWhiteSpace(memberName))
            throw new ArgumentException("Member's name required", nameof(memberName));
        if (string.IsNullOrWhiteSpace(memberEmail))
            throw new ArgumentException("Member's email required", nameof(memberEmail));
        MemberName = memberName;
        this.memberEmail = memberEmail;
        PhotoUrl = photoUrl;
    }
}
