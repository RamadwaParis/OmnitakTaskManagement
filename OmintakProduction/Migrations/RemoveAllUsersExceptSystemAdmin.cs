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

            // Remove seeded users from ModelBuilder.HasData (we'll update the context file)
            // This prevents the seeded users from being recreated
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore deleted users (reverse soft delete)
            migrationBuilder.Sql(@"
                UPDATE [User] 
                SET IsDeleted = 0, 
                    DeletedAt = NULL, 
                    DeletedByUserId = NULL,
                    isActive = 1,
                    IsApproved = 1
                WHERE UserId != 1 AND DeletedByUserId = 1
            ");
        }
    }
}
