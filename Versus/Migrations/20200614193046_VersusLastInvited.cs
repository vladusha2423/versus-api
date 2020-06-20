using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Versus.Migrations
{
    public partial class VersusLastInvited : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobId",
                table: "Versus");

            migrationBuilder.AddColumn<Guid>(
                name: "LastInvitedId",
                table: "Versus",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastInvitedId",
                table: "Versus");

            migrationBuilder.AddColumn<string>(
                name: "JobId",
                table: "Versus",
                type: "text",
                nullable: true);
        }
    }
}
