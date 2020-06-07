using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Versus.Migrations
{
    public partial class nameToUserName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("71026341-bcde-4dec-baa7-4d940d11fdfe"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("bee75d3b-fa86-454a-afca-fa3c485fd825"));

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("0adc837e-50fe-4cee-902f-f46a5b0bdb1b"), "6c65e1a7-1ba8-4bfd-ac39-724b6d103096", "admin", "ADMIN" },
                    { new Guid("0774ae8c-3332-47c6-8928-17115c500577"), "073d961b-3dfe-4caf-8097-eb3b184f22e4", "user", "USER" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("0774ae8c-3332-47c6-8928-17115c500577"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("0adc837e-50fe-4cee-902f-f46a5b0bdb1b"));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("71026341-bcde-4dec-baa7-4d940d11fdfe"), "3fc3325b-d844-4660-b97d-d3dbb7809248", "admin", "ADMIN" },
                    { new Guid("bee75d3b-fa86-454a-afca-fa3c485fd825"), "1757e76b-6736-400a-934b-2159db4423c0", "user", "USER" }
                });
        }
    }
}
