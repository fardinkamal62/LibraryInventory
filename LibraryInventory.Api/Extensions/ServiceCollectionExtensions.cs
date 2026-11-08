using System.Text;
using LibraryInventory.Core.Interfaces;
using LibraryInventory.Core.Settings;
using LibraryInventory.Infrastructure.Data;
using LibraryInventory.Infrastructure.Identity;
using LibraryInventory.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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

    public static IServiceCollection AddAuthFeature(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection("Jwt"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddIdentityCore<ApplicationUser>(o =>
            {
                o.User.RequireUniqueEmail = true;
                o.Password.RequiredLength = 8;
                o.Password.RequireNonAlphanumeric = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.AddScoped<IAuthService, AuthService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        // Configured as a post-configure so the settings are resolved after the host configuration
        // is final (integration tests replace the JWT settings at startup).
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtSettings>>((options, jwtAccessor) =>
            {
                var jwt = jwtAccessor.Value;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization();
        return services;
    }
}
