using FluentValidation;
using LibraryInventory.Api.Extensions;
using LibraryInventory.Api.Validators.Books;

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
builder.Services.AddAuthFeature(builder.Configuration);
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseApiMiddleware();
await app.InitializeDatabaseAsync();   // Migrate always, Seed only in Development

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program;
