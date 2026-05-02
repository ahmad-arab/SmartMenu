using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixHeroSubtitleText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryIndexTitleText",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "HeroSubtitleText",
                table: "Menus");

            migrationBuilder.CreateTable(
                name: "MenuCategoryIndexTitleTexts",
                columns: table => new
                {
                    MenuId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCategoryIndexTitleTexts", x => new { x.MenuId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_MenuCategoryIndexTitleTexts_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuCategoryIndexTitleTexts_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MenuHeroSubtitleTexts",
                columns: table => new
                {
                    MenuId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuHeroSubtitleTexts", x => new { x.MenuId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_MenuHeroSubtitleTexts_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuHeroSubtitleTexts_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuCategoryIndexTitleTexts_LanguageId",
                table: "MenuCategoryIndexTitleTexts",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuHeroSubtitleTexts_LanguageId",
                table: "MenuHeroSubtitleTexts",
                column: "LanguageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuCategoryIndexTitleTexts");

            migrationBuilder.DropTable(
                name: "MenuHeroSubtitleTexts");

            migrationBuilder.AddColumn<string>(
                name: "CategoryIndexTitleText",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeroSubtitleText",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
