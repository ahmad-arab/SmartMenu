using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDomainManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DomainName",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LandingPageUrl",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DomainName",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "LandingPageUrl",
                table: "Tenants");
        }
    }
}
