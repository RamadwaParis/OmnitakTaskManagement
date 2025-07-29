using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OmintakProduction.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAllUsersExceptSystemAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 5);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserId", "CreatedDate", "DeletedAt", "DeletedByUserId", "Email", "FirstName", "IsApproved", "IsDeleted", "LastName", "NeedsWelcome", "Password", "ProjectId", "RoleId", "TeamId", "TeamId1", "UserName", "isActive" },
                values: new object[,]
                {
                    { 2, new DateOnly(2025, 1, 2), null, null, "dumisaninxumalo5gmail.com", "Dumisani", true, false, "Nxumalo", true, "$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi", null, 8, 1, null, "Duma", true },
                    { 3, new DateOnly(2025, 1, 3), null, null, "tledwaba@dynamicdna.co.za", "Thabang", true, false, "Ledwaba", true, "$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi", null, 7, null, null, "Chief", true },
                    { 4, new DateOnly(2025, 1, 4), null, null, "ramadwaparis@gmail.com", "Paris", true, false, "Ramadwa", true, "$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi", null, 6, 2, null, "Paris", true },
                    { 5, new DateOnly(2025, 1, 5), null, null, "ayakazilungile20@gmail.com", "Zilungile", true, false, "Nquku", true, "$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi", null, 5, 3, null, "Zee", true }
                });
        }
    }
}
