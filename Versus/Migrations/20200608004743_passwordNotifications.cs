using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Versus.Migrations
{
    public partial class passwordNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("0774ae8c-3332-47c6-8928-17115c500577"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("0adc837e-50fe-4cee-902f-f46a5b0bdb1b"));

            migrationBuilder.AddColumn<bool>(
                name: "IsNotifications",
                table: "Settings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("e9dd5cd9-6520-48be-9126-f574c342b983"), "7eb7cdba-8b3b-419a-b414-87412a6f1eaf", "admin", "ADMIN" },
                    { new Guid("26fef8bc-3e6d-4a2f-bb99-a9b7bf5bdc6b"), "be898b2e-9adb-41bc-97d3-a7ffdf0342d8", "user", "USER" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("26fef8bc-3e6d-4a2f-bb99-a9b7bf5bdc6b"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("e9dd5cd9-6520-48be-9126-f574c342b983"));

            migrationBuilder.DropColumn(
                name: "IsNotifications",
                table: "Settings");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("0adc837e-50fe-4cee-902f-f46a5b0bdb1b"), "6c65e1a7-1ba8-4bfd-ac39-724b6d103096", "admin", "ADMIN" },
                    { new Guid("0774ae8c-3332-47c6-8928-17115c500577"), "073d961b-3dfe-4caf-8097-eb3b184f22e4", "user", "USER" }
                });
        }
    }
}
