using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LibraryInventory.Api.Dtos.Checkouts;

namespace LibraryInventory.Tests.Integration;

public class CheckoutApiTests
{
    [Fact]
    public async Task Checkout_and_return_flow_updates_availability()
    {
        using var factory = new LibraryApiFactory();
        var librarianToken = await TestAuth.LibrarianTokenAsync(factory);
        using var librarian = TestAuth.AuthorizedClient(factory, librarianToken);
        var bookId = await TestAuth.CreateBookAsync(librarian);

        var (memberClient, memberId, _) = await TestAuth.RegisterMemberAsync(factory);
        using var _member = memberClient;

        var checkoutResponse = await memberClient.PostAsJsonAsync("/api/checkouts",
            new { bookId, memberId });
        Assert.Equal(HttpStatusCode.Created, checkoutResponse.StatusCode);

        var checkout = (await checkoutResponse.Content.ReadFromJsonAsync<CheckoutResponseDto>())!;
        Assert.Equal(14, (checkout.DueDateUtc - checkout.CheckedOutAtUtc).TotalDays, 0);
        Assert.False(checkout.IsOverdue);

        var unavailable = await librarian.GetFromJsonAsync<JsonElement>("/api/books?available=false&pageSize=100");
        Assert.Contains(unavailable.GetProperty("items").EnumerateArray(),
            b => b.GetProperty("bookId").GetInt32() == bookId);

        var returnResponse = await librarian.PutAsync($"/api/checkouts/{checkout.CheckoutId}/return", null);
        Assert.Equal(HttpStatusCode.OK, returnResponse.StatusCode);
        var returned = (await returnResponse.Content.ReadFromJsonAsync<CheckoutResponseDto>())!;
        Assert.Equal(0, returned.DaysOverdue);
        Assert.NotNull(returned.ReturnedAtUtc);

        var secondReturn = await librarian.PutAsync($"/api/checkouts/{checkout.CheckoutId}/return", null);
        Assert.Equal(HttpStatusCode.Conflict, secondReturn.StatusCode);
    }

    [Fact]
    public async Task Member_cannot_check_out_for_another_member()
    {
        using var factory = new LibraryApiFactory();
        var librarianToken = await TestAuth.LibrarianTokenAsync(factory);
        using var librarian = TestAuth.AuthorizedClient(factory, librarianToken);
        var bookId = await TestAuth.CreateBookAsync(librarian);

        var (memberClient, memberId, _) = await TestAuth.RegisterMemberAsync(factory);
        using var _member = memberClient;

        var response = await memberClient.PostAsJsonAsync("/api/checkouts",
            new { bookId, memberId = memberId + 1 });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Checkout_of_missing_book_returns_404()
    {
        using var factory = new LibraryApiFactory();
        var token = await TestAuth.LibrarianTokenAsync(factory);
        using var librarian = TestAuth.AuthorizedClient(factory, token);
        var (memberClient, memberId, _) = await TestAuth.RegisterMemberAsync(factory);
        using var _member = memberClient;

        var response = await librarian.PostAsJsonAsync("/api/checkouts", new { bookId = 999, memberId });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Member_checkout_limit_returns_422()
    {
        using var factory = new LibraryApiFactory();
        var librarianToken = await TestAuth.LibrarianTokenAsync(factory);
        using var librarian = TestAuth.AuthorizedClient(factory, librarianToken);

        var (memberClient, memberId, _) = await TestAuth.RegisterMemberAsync(factory);
        using var _member = memberClient;

        for (var i = 0; i < 3; i++)
        {
            var bookId = await TestAuth.CreateBookAsync(librarian);
            var ok = await memberClient.PostAsJsonAsync("/api/checkouts", new { bookId, memberId });
            Assert.Equal(HttpStatusCode.Created, ok.StatusCode);
        }

        var fourthBook = await TestAuth.CreateBookAsync(librarian);
        var response = await memberClient.PostAsJsonAsync("/api/checkouts",
            new { bookId = fourthBook, memberId });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task Ten_parallel_checkouts_of_one_book_yield_exactly_one_success()
    {
        using var factory = new LibraryApiFactory();
        var librarianToken = await TestAuth.LibrarianTokenAsync(factory);
        using var librarian = TestAuth.AuthorizedClient(factory, librarianToken);

        var bookId = await TestAuth.CreateBookAsync(librarian);

        // Ten different members, so the member limit cannot be what stops them.
        var memberIds = new List<int>();
        for (var i = 0; i < 10; i++)
        {
            var (client, memberId, _) = await TestAuth.RegisterMemberAsync(factory);
            client.Dispose();
            memberIds.Add(memberId);
        }

        var responses = await Task.WhenAll(memberIds.Select(id =>
            librarian.PostAsJsonAsync("/api/checkouts", new { bookId, memberId = id })));

        Assert.Equal(1, responses.Count(r => r.StatusCode == HttpStatusCode.Created));
        Assert.Equal(9, responses.Count(r => r.StatusCode == HttpStatusCode.Conflict));
    }
}
