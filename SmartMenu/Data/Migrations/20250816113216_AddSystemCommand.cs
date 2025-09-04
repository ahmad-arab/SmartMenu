using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemCommand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuCommands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Icon = table.Column<int>(type: "int", nullable: false),
                    HasCustomerMessage = table.Column<bool>(type: "bit", nullable: false),
                    SystemMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MenuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCommands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuCommands_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuCommandTexts",
                columns: table => new
                {
                    MenuCommandId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCommandTexts", x => new { x.MenuCommandId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_MenuCommandTexts_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuCommandTexts_MenuCommands_MenuCommandId",
                        column: x => x.MenuCommandId,
                        principalTable: "MenuCommands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuCommands_MenuId",
                table: "MenuCommands",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCommandTexts_LanguageId",
                table: "MenuCommandTexts",
                column: "LanguageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuCommandTexts");

            migrationBuilder.DropTable(
                name: "MenuCommands");
        }
    }
}
