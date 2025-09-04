using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMenutheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DraftThemeJson",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublishedThemeJson",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThemeKey",
                table: "Menus",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DraftThemeJson",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "PublishedThemeJson",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "ThemeKey",
                table: "Menus");
        }
    }
}
