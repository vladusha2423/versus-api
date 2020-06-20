using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Versus.Migrations
{
    public partial class VersusEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Versus",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    InitiatorId = table.Column<Guid>(nullable: false),
                    InitiatorName = table.Column<string>(nullable: true),
                    OpponentId = table.Column<Guid>(nullable: false),
                    OpponentName = table.Column<string>(nullable: true),
                    Exercise = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    InitiatorIterations = table.Column<int>(nullable: false),
                    OpponentIterations = table.Column<int>(nullable: false),
                    WinnerName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Versus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VersusUsers",
                columns: table => new
                {
                    VersusId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VersusUsers", x => new { x.UserId, x.VersusId });
                    table.ForeignKey(
                        name: "FK_VersusUsers_Versus_VersusId",
                        column: x => x.VersusId,
                        principalTable: "Versus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VersusUsers_VersusId",
                table: "VersusUsers",
                column: "VersusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VersusUsers");

            migrationBuilder.DropTable(
                name: "Versus");
        }
    }
}
