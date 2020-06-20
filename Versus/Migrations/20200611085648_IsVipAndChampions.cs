using Microsoft.EntityFrameworkCore.Migrations;

namespace Versus.Migrations
{
    public partial class IsVipAndChampions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVip",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVip",
                table: "AspNetUsers");
        }
    }
}
