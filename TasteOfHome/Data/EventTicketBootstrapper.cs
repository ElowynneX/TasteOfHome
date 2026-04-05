using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace TasteOfHome.Data
{
    public static class EventTicketBootstrapper
    {
        public static async Task InitializeAsync(AppDbContext db)
        {
            await EnsureColumnAsync(db, "EventReservations", "TicketCode", "TEXT NULL");
            await EnsureColumnAsync(db, "EventReservations", "TicketIssuedAt", "TEXT NULL");
            await EnsureColumnAsync(db, "EventReservations", "IsCheckedIn", "INTEGER NOT NULL DEFAULT 0");
            await EnsureColumnAsync(db, "EventReservations", "CheckedInAt", "TEXT NULL");
            await EnsureColumnAsync(db, "EventReservations", "CheckedInByEmail", "TEXT NULL");

            await db.Database.ExecuteSqlRawAsync(@"
CREATE UNIQUE INDEX IF NOT EXISTS ""IX_EventReservations_TicketCode""
ON ""EventReservations"" (""TicketCode"");");
        }

        private static async Task EnsureColumnAsync(AppDbContext db, string tableName, string columnName, string sqlType)
        {
            var connection = db.Database.GetDbConnection();
            await connection.OpenAsync();

            try
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = $@"PRAGMA table_info(""{tableName}"");";

                using var reader = await cmd.ExecuteReaderAsync();

                var exists = false;
                while (await reader.ReadAsync())
                {
                    var existingColumnName = reader["name"]?.ToString();
                    if (string.Equals(existingColumnName, columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        exists = true;
                        break;
                    }
                }

                await reader.CloseAsync();

                if (!exists)
                {
                    await db.Database.ExecuteSqlRawAsync($@"ALTER TABLE ""{tableName}"" ADD COLUMN ""{columnName}"" {sqlType};");
                }
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
    }
}