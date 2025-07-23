using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OmintakProduction.Migrations
{
    /// <inheritdoc />
    public partial class UniqueTeamUserSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Team",
                columns: new[] { "TeamId", "TeamName" },
                values: new object[,]
                {
                    { 1, "Dreamspace" },
                    { 2, "404Found" },
                    { 3, "Genty" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserId", "CreatedDate", "Email", "FirstName", "IsApproved", "LastName", "NeedsWelcome", "Password", "ProjectId", "RoleId", "TeamId", "TeamId1", "UserName", "isActive" },
                values: new object[,]
                {
                    { 7, new DateOnly(2025, 6, 26), "dreamer1@omnitak.com", null, false, null, true, "Dream@123", null, 2, 1, null, "Dreamer1", true },
                    { 8, new DateOnly(2025, 6, 26), "dreamer2@omnitak.com", null, false, null, true, "Dream@123", null, 2, 1, null, "Dreamer2", true },
                    { 9, new DateOnly(2025, 6, 26), "found1@omnitak.com", null, false, null, true, "Found@123", null, 2, 2, null, "Found1", true },
                    { 10, new DateOnly(2025, 6, 26), "found2@omnitak.com", null, false, null, true, "Found@123", null, 2, 2, null, "Found2", true },
                    { 11, new DateOnly(2025, 6, 26), "genty1@omnitak.com", null, false, null, true, "Genty@123", null, 2, 3, null, "Genty1", true },
                    { 12, new DateOnly(2025, 6, 26), "genty2@omnitak.com", null, false, null, true, "Genty@123", null, 2, 3, null, "Genty2", true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Team",
                keyColumn: "TeamId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Team",
                keyColumn: "TeamId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Team",
                keyColumn: "TeamId",
                keyValue: 3);
        }
    }
}
