using Microsoft.EntityFrameworkCore; using Microsoft.Data.Sqlite;
namespace LibraryInventory.Infrastructure.Data;
internal static class DbExceptionExtensions{public static bool IsUniqueViolation(this DbUpdateException ex)=>ex.InnerException is SqliteException{SqliteErrorCode:19,SqliteExtendedErrorCode:2067};}
