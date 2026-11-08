using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using LibraryInventory.Api.Dtos.Auth;
using LibraryInventory.Api.Dtos.Books;
using LibraryInventory.Core.Common;
using LibraryInventory.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryInventory.Tests.Integration;

internal static class TestAuth
{
    public const string LibrarianEmail = "librarian@test.local";
    public const string LibrarianPassword = "Librarian#123";

    public static async Task<string> LibrarianTokenAsync(LibraryApiFactory factory)
    {
        await EnsureLibrarianAsync(factory);

        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login",
            new { email = LibrarianEmail, password = LibrarianPassword });
        response.EnsureSuccessStatusCode();

        var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        return auth!.Token;
    }

    private static async Task EnsureLibrarianAsync(LibraryApiFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (await users.FindByEmailAsync(LibrarianEmail) is not null) return;

        var user = new ApplicationUser
        {
            UserName = LibrarianEmail,
            Email = LibrarianEmail,
            EmailConfirmed = true
        };
        var created = await users.CreateAsync(user, LibrarianPassword);
        Assert.True(created.Succeeded, string.Join("; ", created.Errors.Select(e => e.Description)));

        var role = await users.AddToRoleAsync(user, Roles.Librarian);
        Assert.True(role.Succeeded, string.Join("; ", role.Errors.Select(e => e.Description)));
    }

    public static HttpClient AuthorizedClient(LibraryApiFactory factory, string token)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public static async Task<(HttpClient Client, int MemberId, string Token)> RegisterMemberAsync(
        LibraryApiFactory factory, string? email = null)
    {
        email ??= $"member-{Guid.NewGuid():N}@test.local";

        using var anonymous = factory.CreateClient();
        var response = await anonymous.PostAsJsonAsync("/api/auth/register",
            new { memberName = "Test Member", email, password = "Member#1234" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var auth = (await response.Content.ReadFromJsonAsync<AuthResponseDto>())!;
        return (AuthorizedClient(factory, auth.Token), auth.MemberId!.Value, auth.Token);
    }

    public static async Task<int> CreateBookAsync(HttpClient librarianClient, string? isbn = null)
    {
        isbn ??= Random.Shared.NextInt64(1_000_000_000_000, 9_999_999_999_999).ToString();

        var response = await librarianClient.PostAsJsonAsync("/api/books", new
        {
            bookName = "Test Book",
            author = "Test Author",
            isbnNumber = isbn,
            pageCount = 100,
            price = 10.5m,
            pageMaterial = 0,
            photoUrl = "https://example.com/book"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var book = (await response.Content.ReadFromJsonAsync<BookResponseDto>())!;
        return book.BookId;
    }
}
