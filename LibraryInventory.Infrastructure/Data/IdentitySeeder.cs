using LibraryInventory.Core.Common;
using LibraryInventory.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace LibraryInventory.Infrastructure.Data;

public static class IdentitySeeder
{
    public static async Task EnsureRolesAsync(RoleManager<IdentityRole> roles)
    {
        foreach (var role in new[] { Roles.Librarian, Roles.Member })
        {
            if (!await roles.RoleExistsAsync(role))
                await roles.CreateAsync(new IdentityRole(role));
        }
    }

    public static async Task SeedLibrarianAsync(UserManager<ApplicationUser> users, string? email, string? password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;

        var normalized = email.Trim().ToLowerInvariant();
        if (await users.FindByEmailAsync(normalized) is not null) return;

        var user = new ApplicationUser { UserName = normalized, Email = normalized, EmailConfirmed = true };
        var created = await users.CreateAsync(user, password);
        if (created.Succeeded) await users.AddToRoleAsync(user, Roles.Librarian);
    }
}
