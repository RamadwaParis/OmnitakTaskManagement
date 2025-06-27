using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OmintakProduction.Migrations
{
    /// <inheritdoc />
    public partial class SeedUserData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserId", "CreatedDate", "Email", "Password", "RoleId", "Status", "UserName" },
                values: new object[,]
                {
                    { 2, new DateOnly(2025, 6, 26), "Engineer@omnitak.com", "Engineer@123", "2", "Active", "EngineerUser" },
                    { 3, new DateOnly(2025, 6, 26), "Tester@omnitak.com", "Tester@123", "3", "Active", "TesterUser" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 3);
        }
    }
}
