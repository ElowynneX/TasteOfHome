using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasteOfHome.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantCultureFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CulturalStory",
                table: "Restaurants",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CulturalTraditions",
                table: "Restaurants",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignatureDishes",
                table: "Restaurants",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "SignatureDishesCsv",
                table: "Restaurants",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CulturalStory",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "CulturalTraditions",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "SignatureDishes",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "SignatureDishesCsv",
                table: "Restaurants");
        }
    }
}
