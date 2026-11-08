using LibraryInventory.Core.Commands;
using LibraryInventory.Core.Common;
using LibraryInventory.Core.Entities;
using LibraryInventory.Core.Settings;
using LibraryInventory.Infrastructure.Data;
using LibraryInventory.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;

namespace LibraryInventory.Tests.Services;

public sealed class CheckoutServiceTests : IDisposable
{
    private static readonly DateTimeOffset Start = new(2026, 1, 10, 10, 0, 0, TimeSpan.Zero);

    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;
    private readonly FakeTimeProvider _time = new(Start);
    private readonly LibrarySettings _settings = new();

    public CheckoutServiceTests()
    {
        // SQLite in-memory keeps the connection open; EF's InMemory provider cannot enforce the
        // filtered unique index that guards the one-active-checkout-per-book rule.
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;
        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();
    }

    private CheckoutService CreateService()
    {
        var options = Options.Create(_settings);
        return new CheckoutService(_db, new StandardDueDateCalculator(options), options, _time);
    }

    private Book AddBook(string isbn)
    {
        var book = new Book($"Book {isbn}", "Author", isbn, 100, 10m, PageTypeEnum.Paperback,
            new Uri($"https://example.com/{isbn}"));
        _db.Books.Add(book);
        _db.SaveChanges();
        return book;
    }

    private Member AddMember(string email)
    {
        var member = new Member("Member", email);
        _db.Members.Add(member);
        _db.SaveChanges();
        return member;
    }

    [Fact]
    public async Task Checkout_missing_book_returns_not_found()
    {
        var member = AddMember("a@test.com");

        var result = await CreateService().CheckoutAsync(new CreateCheckoutCommand(999, member.MemberId), default);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error);
    }

    [Fact]
    public async Task Checkout_missing_member_returns_not_found()
    {
        var book = AddBook("111");

        var result = await CreateService().CheckoutAsync(new CreateCheckoutCommand(book.BookId, 999), default);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error);
    }

    [Fact]
    public async Task Checkout_succeeds_and_sets_due_date_from_calculator()
    {
        var book = AddBook("111");
        var member = AddMember("a@test.com");

        var result = await CreateService().CheckoutAsync(new CreateCheckoutCommand(book.BookId, member.MemberId), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(Start.UtcDateTime, result.Data!.CheckedOutAtUtc);
        Assert.Equal(Start.UtcDateTime.AddDays(_settings.MaxCheckoutDays), result.Data.DueDateUtc);
        Assert.True(result.Data.IsActive);
    }

    [Fact]
    public async Task Checkout_of_already_checked_out_book_returns_conflict()
    {
        var book = AddBook("111");
        var first = AddMember("a@test.com");
        var second = AddMember("b@test.com");
        var service = CreateService();

        await service.CheckoutAsync(new CreateCheckoutCommand(book.BookId, first.MemberId), default);
        var result = await service.CheckoutAsync(new CreateCheckoutCommand(book.BookId, second.MemberId), default);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.Error);
    }

    [Fact]
    public async Task Checkout_above_member_limit_returns_rule_violation()
    {
        var member = AddMember("a@test.com");
        var service = CreateService();

        for (var i = 0; i < _settings.MaxBooksPerMember; i++)
        {
            var book = AddBook($"11{i}");
            var ok = await service.CheckoutAsync(new CreateCheckoutCommand(book.BookId, member.MemberId), default);
            Assert.True(ok.IsSuccess);
        }

        var extraBook = AddBook("999");
        var result = await service.CheckoutAsync(new CreateCheckoutCommand(extraBook.BookId, member.MemberId), default);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.RuleViolation, result.Error);
    }

    [Fact]
    public async Task Return_marks_checkout_returned()
    {
        var book = AddBook("111");
        var member = AddMember("a@test.com");
        var service = CreateService();
        var checkout = await service.CheckoutAsync(new CreateCheckoutCommand(book.BookId, member.MemberId), default);
        _time.Advance(TimeSpan.FromDays(2));

        var result = await service.ReturnAsync(checkout.Data!.CheckoutId, default);

        Assert.True(result.IsSuccess);
        Assert.False(result.Data!.IsActive);
        Assert.Equal(Start.UtcDateTime.AddDays(2), result.Data.ReturnedAtUtc);
    }

    [Fact]
    public async Task Returning_twice_returns_conflict()
    {
        var book = AddBook("111");
        var member = AddMember("a@test.com");
        var service = CreateService();
        var checkout = await service.CheckoutAsync(new CreateCheckoutCommand(book.BookId, member.MemberId), default);
        await service.ReturnAsync(checkout.Data!.CheckoutId, default);

        var result = await service.ReturnAsync(checkout.Data.CheckoutId, default);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.Error);
    }

    [Fact]
    public async Task Returning_missing_checkout_returns_not_found()
    {
        var result = await CreateService().ReturnAsync(999, default);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error);
    }

    [Fact]
    public async Task GetOverdue_returns_only_active_past_due_checkouts()
    {
        var overdueBook = AddBook("111");
        var onTimeBook = AddBook("222");
        var member = AddMember("a@test.com");
        var secondMember = AddMember("b@test.com");
        var service = CreateService();

        await service.CheckoutAsync(new CreateCheckoutCommand(overdueBook.BookId, member.MemberId), default);
        _time.Advance(TimeSpan.FromDays(20));
        await service.CheckoutAsync(new CreateCheckoutCommand(onTimeBook.BookId, secondMember.MemberId), default);

        var overdue = await service.GetOverdueAsync(default);

        Assert.Single(overdue);
        Assert.Equal(overdueBook.BookId, overdue[0].BookId);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}
