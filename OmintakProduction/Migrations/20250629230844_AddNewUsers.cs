using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OmintakProduction.Migrations
{
    /// <inheritdoc />
    public partial class AddNewUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Email",
                value: "Admin1.seededData@omnitak.com");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "Email", "Password", "RoleId", "UserName" },
                values: new object[] { "Admin2.seededData@omnitak.com", "Admin@123", 1, "AdminUser2" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 3,
                columns: new[] { "Email", "Password", "RoleId", "UserName" },
                values: new object[] { "Engineer1.seededData@omnitak.com", "Engineer@123", 2, "EngineerUser1" });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserId", "CreatedDate", "Email", "Password", "RoleId", "UserName", "isActive" },
                values: new object[,]
                {
                    { 4, new DateOnly(2025, 6, 26), "Engineer2.seededData@omnitak.com", "Engineer@123", 2, "EngineerUser2", true },
                    { 5, new DateOnly(2025, 6, 26), "Tester1.seededData@omnitak.com", "Tester@123", 3, "TesterUser1", true },
                    { 6, new DateOnly(2025, 6, 26), "Tester2.seededData@omnitak.com", "Tester@123", 3, "TesterUser2", true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Email",
                value: "Admin@omnitak.com");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "Email", "Password", "RoleId", "UserName" },
                values: new object[] { "Engineer@omnitak.com", "Engineer@123", 2, "EngineerUser" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 3,
                columns: new[] { "Email", "Password", "RoleId", "UserName" },
                values: new object[] { "Tester@omnitak.com", "Tester@123", 3, "TesterUser" });
        }
    }
}
