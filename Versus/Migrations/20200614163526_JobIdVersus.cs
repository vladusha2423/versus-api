using Microsoft.EntityFrameworkCore.Migrations;

namespace Versus.Migrations
{
    public partial class JobIdVersus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JobId",
                table: "Versus",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobId",
                table: "Versus");
        }
    }
}
