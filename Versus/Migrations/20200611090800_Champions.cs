using Microsoft.EntityFrameworkCore.Migrations;

namespace Versus.Migrations
{
    public partial class Champions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Champions",
                columns: table => new
                {
                    UserName = table.Column<string>(nullable: true),
                    Wins = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Champions");
        }
    }
}
