using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OmintakProduction.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 6);

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

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "User",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "RoleId", "RoleName", "UserId" },
                values: new object[,]
                {
                    { 4, "ProjectLead", 4 },
                    { 5, "Developer", 5 },
                    { 6, "Tester", 6 },
                    { 7, "Stakeholder", 7 },
                    { 8, "TeamLead", 8 }
                });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "CreatedDate", "DeletedAt", "DeletedByUserId", "Email", "FirstName", "IsApproved", "IsDeleted", "LastName", "UserName" },
                values: new object[] { new DateOnly(2025, 1, 1), null, null, "uthandocibi@gmail.com", "Thando", true, false, "Cibi", "Tee" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "CreatedDate", "DeletedAt", "DeletedByUserId", "Email", "FirstName", "IsApproved", "IsDeleted", "LastName", "Password", "RoleId", "TeamId", "UserName" },
                values: new object[] { new DateOnly(2025, 1, 2), null, null, "dumisaninxumalo5gmail.com", "Dumisani", true, false, "Nxumalo", "User@123", 8, 1, "Duma" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 3,
                columns: new[] { "CreatedDate", "DeletedAt", "DeletedByUserId", "Email", "FirstName", "IsApproved", "IsDeleted", "LastName", "Password", "RoleId", "UserName" },
                values: new object[] { new DateOnly(2025, 1, 3), null, null, "tledwaba@dynamicdna.co.za", "Thabang", true, false, "Ledwaba", "User@123", 7, "Chief" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 4,
                columns: new[] { "CreatedDate", "DeletedAt", "DeletedByUserId", "Email", "FirstName", "IsApproved", "IsDeleted", "LastName", "Password", "RoleId", "TeamId", "UserName" },
                values: new object[] { new DateOnly(2025, 1, 4), null, null, "ramadwaparis@gmail.com", "Paris", true, false, "Ramadwa", "User@123", 6, 2, "Paris" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 5,
                columns: new[] { "CreatedDate", "DeletedAt", "DeletedByUserId", "Email", "FirstName", "IsApproved", "IsDeleted", "LastName", "Password", "RoleId", "TeamId", "UserName" },
                values: new object[] { new DateOnly(2025, 1, 5), null, null, "ayakazilungile20@gmail.com", "Zilungile", true, false, "Nquku", "User@123", 5, 3, "Zee" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 8);

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "User");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "User");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "CreatedDate", "Email", "FirstName", "IsApproved", "LastName", "UserName" },
                values: new object[] { new DateOnly(2025, 6, 26), "Admin1.seededData@omnitak.com", null, false, null, "AdminUser" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "CreatedDate", "Email", "FirstName", "IsApproved", "LastName", "Password", "RoleId", "TeamId", "UserName" },
                values: new object[] { new DateOnly(2025, 6, 26), "Admin2.seededData@omnitak.com", null, false, null, "Admin@123", 1, null, "AdminUser2" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 3,
                columns: new[] { "CreatedDate", "Email", "FirstName", "IsApproved", "LastName", "Password", "RoleId", "UserName" },
                values: new object[] { new DateOnly(2025, 6, 26), "Engineer1.seededData@omnitak.com", null, false, null, "Engineer@123", 2, "EngineerUser1" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 4,
                columns: new[] { "CreatedDate", "Email", "FirstName", "IsApproved", "LastName", "Password", "RoleId", "TeamId", "UserName" },
                values: new object[] { new DateOnly(2025, 6, 26), "Engineer2.seededData@omnitak.com", null, false, null, "Engineer@123", 2, null, "EngineerUser2" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 5,
                columns: new[] { "CreatedDate", "Email", "FirstName", "IsApproved", "LastName", "Password", "RoleId", "TeamId", "UserName" },
                values: new object[] { new DateOnly(2025, 6, 26), "Tester1.seededData@omnitak.com", null, false, null, "Tester@123", 3, null, "TesterUser1" });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserId", "CreatedDate", "Email", "FirstName", "IsApproved", "LastName", "NeedsWelcome", "Password", "ProjectId", "RoleId", "TeamId", "TeamId1", "UserName", "isActive" },
                values: new object[,]
                {
                    { 6, new DateOnly(2025, 6, 26), "Tester2.seededData@omnitak.com", null, false, null, true, "Tester@123", null, 3, null, null, "TesterUser2", true },
                    { 7, new DateOnly(2025, 6, 26), "dreamer1@omnitak.com", null, false, null, true, "Dream@123", null, 2, 1, null, "Dreamer1", true },
                    { 8, new DateOnly(2025, 6, 26), "dreamer2@omnitak.com", null, false, null, true, "Dream@123", null, 2, 1, null, "Dreamer2", true },
                    { 9, new DateOnly(2025, 6, 26), "found1@omnitak.com", null, false, null, true, "Found@123", null, 2, 2, null, "Found1", true },
                    { 10, new DateOnly(2025, 6, 26), "found2@omnitak.com", null, false, null, true, "Found@123", null, 2, 2, null, "Found2", true },
                    { 11, new DateOnly(2025, 6, 26), "genty1@omnitak.com", null, false, null, true, "Genty@123", null, 2, 3, null, "Genty1", true },
                    { 12, new DateOnly(2025, 6, 26), "genty2@omnitak.com", null, false, null, true, "Genty@123", null, 2, 3, null, "Genty2", true }
                });
        }
    }
}
