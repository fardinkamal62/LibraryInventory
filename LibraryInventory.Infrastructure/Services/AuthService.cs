using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LibraryInventory.Core.Commands;
using LibraryInventory.Core.Common;
using LibraryInventory.Core.Entities;
using LibraryInventory.Core.Interfaces;
using LibraryInventory.Core.Settings;
using LibraryInventory.Infrastructure.Data;
using LibraryInventory.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LibraryInventory.Infrastructure.Services;

public class AuthService(
    AppDbContext db,
    UserManager<ApplicationUser> users,
    IOptions<JwtSettings> jwt,
    TimeProvider time) : IAuthService
{
    public async Task<Result<AuthResult>> RegisterAsync(RegisterCommand c, CancellationToken ct)
    {
        var email = c.Email.Trim().ToLowerInvariant();
        if (await db.Members.AnyAsync(m => m.Email == email, ct))
            return Result<AuthResult>.Conflict($"A member with email {email} already exists");

        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        Member member;
        try
        {
            member = new Member(c.MemberName, email);
        }
        catch (ArgumentException ex)
        {
            return Result<AuthResult>.Validation(ex.Message);
        }

        db.Members.Add(member);
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            return Result<AuthResult>.Conflict($"A member with email {email} already exists");
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            MemberId = member.MemberId
        };
        var created = await users.CreateAsync(user, c.Password);
        if (!created.Succeeded)
            return Result<AuthResult>.Validation(string.Join("; ", created.Errors.Select(e => e.Description)));

        var addedToRole = await users.AddToRoleAsync(user, Roles.Member);
        if (!addedToRole.Succeeded)
            return Result<AuthResult>.Validation(string.Join("; ", addedToRole.Errors.Select(e => e.Description)));

        await transaction.CommitAsync(ct);
        return Result<AuthResult>.Success(CreateToken(user, Roles.Member), "Registered");
    }

    public async Task<Result<AuthResult>> LoginAsync(LoginCommand c, CancellationToken ct)
    {
        var email = c.Email.Trim().ToLowerInvariant();
        var user = await users.FindByEmailAsync(email);
        if (user is null || !await users.CheckPasswordAsync(user, c.Password))
            return Result<AuthResult>.Unauthorized("Invalid email or password");

        var role = (await users.GetRolesAsync(user)).FirstOrDefault() ?? Roles.Member;
        return Result<AuthResult>.Success(CreateToken(user, role));
    }

    private AuthResult CreateToken(ApplicationUser user, string role)
    {
        var now = time.GetUtcNow().UtcDateTime;
        var expires = now.AddMinutes(jwt.Value.ExpiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Role, role)
        };
        if (user.MemberId is { } memberId) claims.Add(new Claim("memberId", memberId.ToString()));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Value.Key));
        var token = new JwtSecurityToken(
            issuer: jwt.Value.Issuer,
            audience: jwt.Value.Audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new AuthResult(new JwtSecurityTokenHandler().WriteToken(token), expires, user.MemberId, role);
    }
}
