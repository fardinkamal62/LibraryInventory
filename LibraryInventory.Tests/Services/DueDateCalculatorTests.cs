using LibraryInventory.Core.Settings;
using LibraryInventory.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace LibraryInventory.Tests.Services;

public class DueDateCalculatorTests
{
    private static readonly DateTime CheckedOut = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Standard_calculator_adds_max_checkout_days()
    {
        var settings = Options.Create(new LibrarySettings { MaxCheckoutDays = 14 });
        var calculator = new StandardDueDateCalculator(settings);

        Assert.Equal(CheckedOut.AddDays(14), calculator.Calculate(CheckedOut));
    }

    [Fact]
    public void Extended_calculator_multiplies_max_checkout_days()
    {
        var settings = Options.Create(new LibrarySettings { MaxCheckoutDays = 14, ExtendedLoanMultiplier = 2 });
        var calculator = new ExtendedDueDateCalculator(settings);

        Assert.Equal(CheckedOut.AddDays(28), calculator.Calculate(CheckedOut));
    }

    [Fact]
    public void Calculators_honour_configured_values()
    {
        var settings = Options.Create(new LibrarySettings { MaxCheckoutDays = 7, ExtendedLoanMultiplier = 3 });

        Assert.Equal(CheckedOut.AddDays(7), new StandardDueDateCalculator(settings).Calculate(CheckedOut));
        Assert.Equal(CheckedOut.AddDays(21), new ExtendedDueDateCalculator(settings).Calculate(CheckedOut));
    }
}
