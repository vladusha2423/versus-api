using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Versus.Migrations
{
    public partial class UserSocket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSockets",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    SocketId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSockets", x => new { x.UserId, x.SocketId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSockets");
        }
    }
}
