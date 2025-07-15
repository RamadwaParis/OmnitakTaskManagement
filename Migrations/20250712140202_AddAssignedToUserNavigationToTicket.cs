using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmintakProduction.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedToUserNavigationToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Ticket_AssignedToUserId",
                table: "Ticket",
                column: "AssignedToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_User_AssignedToUserId",
                table: "Ticket",
                column: "AssignedToUserId",
                principalTable: "User",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_User_AssignedToUserId",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_AssignedToUserId",
                table: "Ticket");
        }
    }
}
