using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasteOfHome.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminHostedEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PricePerPerson",
                table: "CulturalEvents",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CulturalEvents",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DressCode",
                table: "CulturalEvents",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntertainmentDetails",
                table: "CulturalEvents",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FoodDetails",
                table: "CulturalEvents",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Highlights",
                table: "CulturalEvents",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HostedBy",
                table: "CulturalEvents",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CulturalEvents");

            migrationBuilder.DropColumn(
                name: "DressCode",
                table: "CulturalEvents");

            migrationBuilder.DropColumn(
                name: "EntertainmentDetails",
                table: "CulturalEvents");

            migrationBuilder.DropColumn(
                name: "FoodDetails",
                table: "CulturalEvents");

            migrationBuilder.DropColumn(
                name: "Highlights",
                table: "CulturalEvents");

            migrationBuilder.DropColumn(
                name: "HostedBy",
                table: "CulturalEvents");

            migrationBuilder.AlterColumn<decimal>(
                name: "PricePerPerson",
                table: "CulturalEvents",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
