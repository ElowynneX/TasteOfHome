using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasteOfHome.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminSettingsOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CheckedInAt",
                table: "EventReservations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckedInByEmail",
                table: "EventReservations",
                type: "TEXT",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckedIn",
                table: "EventReservations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TicketCode",
                table: "EventReservations",
                type: "TEXT",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TicketIssuedAt",
                table: "EventReservations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdminSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EnableRestaurantReservations = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableEventBookings = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableHiddenGemSubmissions = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequireHiddenGemApproval = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowHiddenGemsOnHomepage = table.Column<bool>(type: "INTEGER", nullable: false),
                    MaxGuestsPerReservation = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 25, nullable: true),
                    EmailNotificationsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    SmsNotificationsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    EventAnnouncementsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    DefaultGuestCount = table.Column<int>(type: "INTEGER", nullable: false),
                    DietaryPreference = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SeatingPreference = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    MarketingEmailsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_Email",
                table: "UserSettings",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminSettings");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropColumn(
                name: "CheckedInAt",
                table: "EventReservations");

            migrationBuilder.DropColumn(
                name: "CheckedInByEmail",
                table: "EventReservations");

            migrationBuilder.DropColumn(
                name: "IsCheckedIn",
                table: "EventReservations");

            migrationBuilder.DropColumn(
                name: "TicketCode",
                table: "EventReservations");

            migrationBuilder.DropColumn(
                name: "TicketIssuedAt",
                table: "EventReservations");
        }
    }
}
