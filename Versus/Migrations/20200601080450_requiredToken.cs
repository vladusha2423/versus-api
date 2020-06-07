using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Versus.Migrations
{
    public partial class requiredToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "AspNetUsers",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("71026341-bcde-4dec-baa7-4d940d11fdfe"), "3fc3325b-d844-4660-b97d-d3dbb7809248", "admin", "ADMIN" },
                    { new Guid("bee75d3b-fa86-454a-afca-fa3c485fd825"), "1757e76b-6736-400a-934b-2159db4423c0", "user", "USER" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("71026341-bcde-4dec-baa7-4d940d11fdfe"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("bee75d3b-fa86-454a-afca-fa3c485fd825"));

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
