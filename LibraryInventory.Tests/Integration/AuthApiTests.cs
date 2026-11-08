using System.Net;
using System.Net.Http.Json;
using LibraryInventory.Api.Dtos.Auth;

namespace LibraryInventory.Tests.Integration;

public class AuthApiTests
{
    [Fact]
    public async Task Anonymous_request_to_create_book_returns_401()
    {
        using var factory = new LibraryApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/books", ValidBook());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Member_cannot_create_book()
    {
        using var factory = new LibraryApiFactory();
        var librarian = await TestAuth.LibrarianTokenAsync(factory);
        using var librarianClient = TestAuth.AuthorizedClient(factory, librarian);
        _ = await TestAuth.CreateBookAsync(librarianClient);

        var (memberClient, _, _) = await TestAuth.RegisterMemberAsync(factory);
        using var _client = memberClient;

        var response = await memberClient.PostAsJsonAsync("/api/books", ValidBook());

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Librarian_can_create_book_and_gets_location_header()
    {
        using var factory = new LibraryApiFactory();
        var token = await TestAuth.LibrarianTokenAsync(factory);
        using var client = TestAuth.AuthorizedClient(factory, token);

        var response = await client.PostAsJsonAsync("/api/books", ValidBook());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task Login_with_wrong_password_returns_401()
    {
        using var factory = new LibraryApiFactory();
        _ = await TestAuth.LibrarianTokenAsync(factory);
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login",
            new { email = TestAuth.LibrarianEmail, password = "wrong-password" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Member_management_requires_librarian()
    {
        using var factory = new LibraryApiFactory();

        using var anonymous = factory.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonymous.GetAsync("/api/members")).StatusCode);

        var (memberClient, _, _) = await TestAuth.RegisterMemberAsync(factory);
        using var _client = memberClient;
        Assert.Equal(HttpStatusCode.Forbidden, (await memberClient.GetAsync("/api/members")).StatusCode);

        var token = await TestAuth.LibrarianTokenAsync(factory);
        using var librarian = TestAuth.AuthorizedClient(factory, token);
        Assert.Equal(HttpStatusCode.OK, (await librarian.GetAsync("/api/members")).StatusCode);
    }

    [Fact]
    public async Task Registering_the_same_email_twice_returns_409()
    {
        using var factory = new LibraryApiFactory();
        using var client = factory.CreateClient();
        var email = $"member-{Guid.NewGuid():N}@test.local";

        var first = await client.PostAsJsonAsync("/api/auth/register",
            new { memberName = "First", email, password = "Member#1234" });
        var second = await client.PostAsJsonAsync("/api/auth/register",
            new { memberName = "Second", email, password = "Member#1234" });

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Register_returns_member_role_and_member_id()
    {
        using var factory = new LibraryApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register",
            new { memberName = "New Member", email = $"m-{Guid.NewGuid():N}@test.local", password = "Member#1234" });

        var auth = (await response.Content.ReadFromJsonAsync<AuthResponseDto>())!;
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Member", auth.Role);
        Assert.NotNull(auth.MemberId);
        Assert.False(string.IsNullOrWhiteSpace(auth.Token));
    }

    private static object ValidBook()
    {
        return new
        {
            bookName = "Auth Test Book",
            author = "Author",
            isbnNumber = Random.Shared.NextInt64(1_000_000_000_000, 9_999_999_999_999).ToString(),
            pageCount = 100,
            price = 9.99m,
            pageMaterial = 0,
            photoUrl = "https://example.com/book"
        };
    }
}
