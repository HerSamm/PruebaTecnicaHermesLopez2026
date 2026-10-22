using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace PruebaTecnicaHermes.Api.Application.Common;

public static class DbExceptionExtensions
{
    private const int SqliteConstraintErrorCode = 19;

   
    public static bool IsUniqueConstraintViolation(this DbUpdateException ex) =>
        ex.InnerException is SqliteException { SqliteErrorCode: SqliteConstraintErrorCode } sqliteEx
        && sqliteEx.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase);
}
