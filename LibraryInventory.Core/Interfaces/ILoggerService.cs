namespace LibraryInventory.Core.Interfaces;

public interface ILoggerService
{
    void Info(string message);
    void Error(Exception ex, string message);
}
