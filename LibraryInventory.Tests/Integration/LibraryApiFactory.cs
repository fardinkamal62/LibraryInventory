using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace LibraryInventory.Tests.Integration;

public sealed class LibraryApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"library-tests-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = $"Data Source={_dbPath};Default Timeout=30",
                ["Jwt:Issuer"] = "LibraryInventory.Tests",
                ["Jwt:Audience"] = "LibraryInventory.Tests",
                ["Jwt:Key"] = "integration-test-signing-key-0123456789abcdef",
                ["Jwt:ExpiryMinutes"] = "60",
                ["LibrarySettings:MaxCheckoutDays"] = "14",
                ["LibrarySettings:MaxBooksPerMember"] = "3",
                ["LibrarySettings:DueDatePolicy"] = "Standard",
                ["LibrarySettings:ExtendedLoanMultiplier"] = "2"
            });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        SqliteConnection.ClearAllPools();
        try
        {
            File.Delete(_dbPath);
        }
        catch (IOException)
        {
            // The test process will clean up its temp directory eventually.
        }
    }
}
