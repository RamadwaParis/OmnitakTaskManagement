using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmintakProduction.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelChangesAugust2025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$VrZfFLqrJcK3eZ2y7hYCl.QqG5JjS8Z4xKt3CYi2qE1UhAzF9LkTe");
        }
    }
}
