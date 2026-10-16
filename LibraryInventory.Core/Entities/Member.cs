using LibraryInventory.Core.Common;
namespace LibraryInventory.Core.Entities;
public class Member : BaseEntity
{
    private Member(){}
    public Member(string memberName,string email)=>UpdateDetails(memberName,email);
    public int MemberId{get;private set;} public string MemberName{get;private set;}=null!; public string Email{get;private set;}=null!;
    public void UpdateDetails(string name,string email){if(string.IsNullOrWhiteSpace(name))throw new ArgumentException("Name required",nameof(name));if(string.IsNullOrWhiteSpace(email))throw new ArgumentException("Email required",nameof(email));MemberName=name.Trim();Email=email.Trim().ToLowerInvariant();}
}
