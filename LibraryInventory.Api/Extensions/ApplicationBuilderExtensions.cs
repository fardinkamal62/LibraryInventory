using LibraryInventory.Api.Middleware;

namespace LibraryInventory.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApiMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestLoggingMiddleware>();    // outermost
        app.UseMiddleware<ExceptionHandlingMiddleware>(); // inside it
        return app;
    }
}
