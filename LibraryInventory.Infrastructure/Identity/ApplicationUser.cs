using Microsoft.AspNetCore.Identity;

namespace LibraryInventory.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public int? MemberId { get; set; }
}
