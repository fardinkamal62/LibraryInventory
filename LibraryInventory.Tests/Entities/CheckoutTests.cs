using LibraryInventory.Core.Entities;

namespace LibraryInventory.Tests.Entities;

public class CheckoutTests
{
    private static Checkout Create(DateTime? due = null)
    {
        return new Checkout(1, 1, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            due ?? new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void Constructor_rejects_due_date_not_after_checkout_date()
    {
        Assert.Throws<ArgumentException>(() =>
            new Checkout(1, 1, new DateTime(2026, 1, 1), new DateTime(2026, 1, 1)));
    }

    [Fact]
    public void DaysOverdue_is_zero_before_due_date()
    {
        Assert.Equal(0, Create().DaysOverdue(new DateTime(2026, 1, 10)));
    }

    [Fact]
    public void DaysOverdue_counts_days_past_due_when_not_returned()
    {
        Assert.Equal(3, Create().DaysOverdue(new DateTime(2026, 1, 18)));
    }

    [Fact]
    public void DaysOverdue_counts_up_to_return_date_when_returned_late()
    {
        var checkout = Create();
        checkout.MarkReturned(new DateTime(2026, 1, 20));

        Assert.Equal(5, checkout.DaysOverdue(new DateTime(2026, 2, 1)));
    }

    [Fact]
    public void DaysOverdue_is_zero_when_returned_on_time()
    {
        var checkout = Create();
        checkout.MarkReturned(new DateTime(2026, 1, 14));

        Assert.Equal(0, checkout.DaysOverdue(new DateTime(2026, 1, 20)));
    }

    [Theory]
    [InlineData("2026-01-14", false)]
    [InlineData("2026-01-15", false)]
    [InlineData("2026-01-16", true)]
    public void IsOverdue_only_when_active_and_past_due(string now, bool expected)
    {
        Assert.Equal(expected, Create().IsOverdue(DateTime.Parse(now)));
    }

    [Fact]
    public void MarkReturned_twice_throws()
    {
        var checkout = Create();
        checkout.MarkReturned(new DateTime(2026, 1, 10));

        Assert.Throws<InvalidOperationException>(() => checkout.MarkReturned(new DateTime(2026, 1, 11)));
    }
}
