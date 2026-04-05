using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasteOfHome.Migrations
{
    /// <inheritdoc />
    public partial class FixHiddenGemAdminFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "HiddenGems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedByEmail",
                table: "HiddenGems",
                type: "TEXT",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmittedByEmail",
                table: "HiddenGems",
                type: "TEXT",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "HiddenGems",
                type: "TEXT",
                maxLength: 450,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "HiddenGems");

            migrationBuilder.DropColumn(
                name: "ReviewedByEmail",
                table: "HiddenGems");

            migrationBuilder.DropColumn(
                name: "SubmittedByEmail",
                table: "HiddenGems");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "HiddenGems");
        }
    }
}
