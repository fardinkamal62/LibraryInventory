using LibraryInventory.Api.Middleware;
using LibraryInventory.Infrastructure.Data;
using LibraryInventory.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryInventory.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApiMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestLoggingMiddleware>();    // outermost
        app.UseMiddleware<ExceptionHandlingMiddleware>(); // inside it
        return app;
    }

    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var db = services.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        await IdentitySeeder.EnsureRolesAsync(services.GetRequiredService<RoleManager<IdentityRole>>());

        if (!app.Environment.IsDevelopment()) return;

        await DbContextSeeder.SeedAsync(db);

        var configuration = services.GetRequiredService<IConfiguration>();
        await IdentitySeeder.SeedLibrarianAsync(
            services.GetRequiredService<UserManager<ApplicationUser>>(),
            configuration["Seed:LibrarianEmail"],
            configuration["Seed:LibrarianPassword"]);
    }
}
