using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuCommandstaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "MenuStaffs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "MenuCommandStaffs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuCommandId = table.Column<int>(type: "int", nullable: false),
                    MenuStaffId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCommandStaffs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuCommandStaffs_MenuCommands_MenuCommandId",
                        column: x => x.MenuCommandId,
                        principalTable: "MenuCommands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuCommandStaffs_MenuStaffs_MenuStaffId",
                        column: x => x.MenuStaffId,
                        principalTable: "MenuStaffs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuCommandStaffs_MenuCommandId",
                table: "MenuCommandStaffs",
                column: "MenuCommandId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCommandStaffs_MenuStaffId",
                table: "MenuCommandStaffs",
                column: "MenuStaffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuCommandStaffs");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "MenuStaffs");
        }
    }
}
