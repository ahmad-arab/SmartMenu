using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.Data.Migrations
{
    /// <inheritdoc />
    public partial class addingThemes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThemeKey",
                table: "Menus");

            migrationBuilder.RenameColumn(
                name: "PublishedThemeJson",
                table: "Menus",
                newName: "MenuThemeJson");

            migrationBuilder.RenameColumn(
                name: "DraftThemeJson",
                table: "Menus",
                newName: "LableThemeJson");

            migrationBuilder.AddColumn<string>(
                name: "CategoryCardThemeJson",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryCardThemeKey",
                table: "Menus",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemCardThemeJson",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemCardThemeKey",
                table: "Menus",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LableThemeKey",
                table: "Menus",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MenuThemeKey",
                table: "Menus",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryCardThemeJson",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "CategoryCardThemeKey",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "ItemCardThemeJson",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "ItemCardThemeKey",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "LableThemeKey",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "MenuThemeKey",
                table: "Menus");

            migrationBuilder.RenameColumn(
                name: "MenuThemeJson",
                table: "Menus",
                newName: "PublishedThemeJson");

            migrationBuilder.RenameColumn(
                name: "LableThemeJson",
                table: "Menus",
                newName: "DraftThemeJson");

            migrationBuilder.AddColumn<string>(
                name: "ThemeKey",
                table: "Menus",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);
        }
    }
}
