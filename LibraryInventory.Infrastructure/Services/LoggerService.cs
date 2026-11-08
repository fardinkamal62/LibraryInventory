using LibraryInventory.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace LibraryInventory.Infrastructure.Services;

public class LoggerService(ILogger<LoggerService> logger) : ILoggerService
{
    public void Info(string message) => logger.LogInformation("{Message}", message);
    public void Error(Exception ex, string message) => logger.LogError(ex, "{Message}", message);
}
