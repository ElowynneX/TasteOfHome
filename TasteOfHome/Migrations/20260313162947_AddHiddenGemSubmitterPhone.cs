using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasteOfHome.Migrations
{
    /// <inheritdoc />
    public partial class AddHiddenGemSubmitterPhone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NotificationSent",
                table: "HiddenGems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SubmitterPhoneNumber",
                table: "HiddenGems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationSent",
                table: "HiddenGems");

            migrationBuilder.DropColumn(
                name: "SubmitterPhoneNumber",
                table: "HiddenGems");
        }
    }
}
