using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LibraryInventory.Infrastructure.Data;

internal static class DbExceptionExtensions
{
    public static bool IsUniqueViolation(this DbUpdateException ex)
    {
        return ex.InnerException is SqliteException { SqliteErrorCode: 19, SqliteExtendedErrorCode: 2067 };
    }
}