using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmintakProduction.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAllUsersExceptSystemAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Soft delete all users except System Admin (UserId = 1)
            migrationBuilder.Sql(@"
                UPDATE [User] 
                SET IsDeleted = 1, 
                    DeletedAt = GETUTCDATE(), 
                    DeletedByUserId = 1,
                    isActive = 0,
                    IsApproved = 0
                WHERE UserId != 1 AND RoleId != 1
            ");

            // Delete seeded user data
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
            // Restore soft-deleted users
            migrationBuilder.Sql(@"
                UPDATE [User] 
                SET IsDeleted = 0, 
                    DeletedAt = NULL, 
                    DeletedByUserId = NULL,
                    isActive = 1,
                    IsApproved = 1
                WHERE UserId != 1 AND DeletedByUserId = 1
            ");

            // Restore seeded data
            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserId", "CreatedDate", "DeletedAt", "DeletedByUserId", "Email", "FirstName", "IsApproved", "IsDeleted", "LastName", "NeedsWelcome", "Password", "ProjectId", "RoleId", "TeamId", "TeamId1", "UserName", "isActive" },
                values: new object[,]
                {
                    { 2, new DateOnly(2025, 1, 2), null, null, "dumisaninxumalo5gmail.com", "Dumisani", true, false, "Nxumalo", true, "$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi", null, 8, 1, null, "Duma", true },
                    { 3, new DateOnly(2025, 1, 3), null, null, "tledwaba@dynamicdna.co.za", "Thabang", true, false, "Ledwaba", true, "$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi", null, 7, null, null, "Chief", true }
                });
        }
    }
}