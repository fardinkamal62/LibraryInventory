using LibraryInventory.Core.Interfaces;
using LibraryInventory.Core.Services;
using LibraryInventory.Core.Settings;
using LibraryInventory.Infrastructure.Data;
using LibraryInventory.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LibraryInventory.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(o =>
            o.UseSqlite(configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("LibraryInventory.Infrastructure")));
        return services;
    }

    public static IServiceCollection AddLibrarySettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<LibrarySettings>()
            .Bind(configuration.GetSection("LibrarySettings"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        return services;
    }

    public static IServiceCollection AddBookFeature(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        return services;
    }

    public static IServiceCollection AddMemberFeature(this IServiceCollection services)
    {
        services.AddScoped<IMemberService, MemberService>();
        return services;
    }

    public static IServiceCollection AddCheckoutFeature(this IServiceCollection services)
    {
        services.AddScoped<ICheckoutService, CheckoutService>();
        services.AddSingleton<IDueDateCalculator>(sp =>
            sp.GetRequiredService<IOptions<LibrarySettings>>().Value.DueDatePolicy == "Extended"
                ? ActivatorUtilities.CreateInstance<ExtendedDueDateCalculator>(sp)
                : ActivatorUtilities.CreateInstance<StandardDueDateCalculator>(sp));
        return services;
    }

    public static IServiceCollection AddLoggerService(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerService, LoggerService>();
        return services;
    }
}
