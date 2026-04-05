using Microsoft.EntityFrameworkCore;

namespace TasteOfHome.Data
{
    public static class SettingsBootstrapper
    {
        public static async Task InitializeAsync(AppDbContext db)
        {
            await db.Database.ExecuteSqlRawAsync(@"
CREATE TABLE IF NOT EXISTS ""UserSettings"" (
    ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_UserSettings"" PRIMARY KEY AUTOINCREMENT,
    ""Email"" TEXT NOT NULL,
    ""FullName"" TEXT NOT NULL,
    ""PhoneNumber"" TEXT NULL,
    ""EmailNotificationsEnabled"" INTEGER NOT NULL DEFAULT 1,
    ""SmsNotificationsEnabled"" INTEGER NOT NULL DEFAULT 1,
    ""EventAnnouncementsEnabled"" INTEGER NOT NULL DEFAULT 1,
    ""DefaultGuestCount"" INTEGER NOT NULL DEFAULT 2,
    ""DietaryPreference"" TEXT NULL,
    ""SeatingPreference"" TEXT NULL,
    ""MarketingEmailsEnabled"" INTEGER NOT NULL DEFAULT 1,
    ""UpdatedAt"" TEXT NOT NULL
);");

            await db.Database.ExecuteSqlRawAsync(@"
CREATE UNIQUE INDEX IF NOT EXISTS ""IX_UserSettings_Email""
ON ""UserSettings"" (""Email"");");

            await db.Database.ExecuteSqlRawAsync(@"
CREATE TABLE IF NOT EXISTS ""AdminSettings"" (
    ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_AdminSettings"" PRIMARY KEY,
    ""EnableRestaurantReservations"" INTEGER NOT NULL DEFAULT 1,
    ""EnableEventBookings"" INTEGER NOT NULL DEFAULT 1,
    ""EnableHiddenGemSubmissions"" INTEGER NOT NULL DEFAULT 1,
    ""RequireHiddenGemApproval"" INTEGER NOT NULL DEFAULT 1,
    ""ShowHiddenGemsOnHomepage"" INTEGER NOT NULL DEFAULT 0,
    ""MaxGuestsPerReservation"" INTEGER NOT NULL DEFAULT 12,
    ""UpdatedAt"" TEXT NOT NULL
);");

            await db.Database.ExecuteSqlRawAsync(@"
INSERT OR IGNORE INTO ""AdminSettings""
(
    ""Id"",
    ""EnableRestaurantReservations"",
    ""EnableEventBookings"",
    ""EnableHiddenGemSubmissions"",
    ""RequireHiddenGemApproval"",
    ""ShowHiddenGemsOnHomepage"",
    ""MaxGuestsPerReservation"",
    ""UpdatedAt""
)
VALUES
(
    1,
    1,
    1,
    1,
    1,
    0,
    12,
    datetime('now')
);");
        }
    }
}