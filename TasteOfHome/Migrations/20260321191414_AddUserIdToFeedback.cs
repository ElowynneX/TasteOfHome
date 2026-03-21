using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasteOfHome.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationSent",
                table: "HiddenGems");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Feedback",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Feedback");

            migrationBuilder.AddColumn<bool>(
                name: "NotificationSent",
                table: "HiddenGems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
