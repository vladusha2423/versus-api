using Microsoft.EntityFrameworkCore.Migrations;

namespace Versus.Migrations
{
    public partial class Rate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Champions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rate",
                table: "Champions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "Champions");
        }
    }
}
