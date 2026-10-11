using LibraryInventory.Core.Common;

namespace LibraryInventory.Core.Entities;


public class Member(string memberName, string memberEmail) : BaseEntity
{
    public string MemberName { get; private set; } = memberName;

    public string MemberEmail { get; private set; } = memberEmail;

    public Uri PhotoUrl { get; private set; } = new Uri("https://example.com/default-photo.jpg");

    public string MemberAddress { get; private set; } = string.Empty;

    public void UpdateDetails(string memberName, string memberEmail, Uri photoUrl, string memberAddress)
    {
        if (string.IsNullOrWhiteSpace(memberName))
            throw new ArgumentException("Member's name required", nameof(memberName));
        if (string.IsNullOrWhiteSpace(memberEmail))
            throw new ArgumentException("Member's email required", nameof(memberEmail));
        if (photoUrl == null)
            throw new ArgumentNullException(nameof(photoUrl), "Member's photo URL required");
        if (string.IsNullOrWhiteSpace(memberAddress))
            throw new ArgumentException("Member's address required", nameof(memberAddress));
        MemberName = memberName;
        MemberEmail = memberEmail;
        PhotoUrl = photoUrl;
        MemberAddress = memberAddress;
    }
}
