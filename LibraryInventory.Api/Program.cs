using FluentValidation;
using LibraryInventory.Api.Extensions;
using LibraryInventory.Api.Validators.Books;
using LibraryInventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
Directory.CreateDirectory("App_Data");

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookDtoValidator>();

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddLibrarySettings(builder.Configuration);
builder.Services.AddBookFeature();
builder.Services.AddMemberFeature();
builder.Services.AddCheckoutFeature();
builder.Services.AddLoggerService();
builder.Services.AddOpenApi();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    if (app.Environment.IsDevelopment()) await DbContextSeeder.SeedAsync(db);
}

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseApiMiddleware();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
