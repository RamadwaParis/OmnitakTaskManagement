using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmintakProduction.Migrations
{
    /// <inheritdoc />
    public partial class FixTaskHistoryRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistory_Tasks_TaskId1",
                table: "TaskHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistory_User_UserId1",
                table: "TaskHistory");

            migrationBuilder.DropIndex(
                name: "IX_TaskHistory_TaskId1",
                table: "TaskHistory");

            migrationBuilder.DropIndex(
                name: "IX_TaskHistory_UserId1",
                table: "TaskHistory");

            migrationBuilder.DropColumn(
                name: "TaskId1",
                table: "TaskHistory");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "TaskHistory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TaskId1",
                table: "TaskHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "TaskHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistory_TaskId1",
                table: "TaskHistory",
                column: "TaskId1");

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistory_UserId1",
                table: "TaskHistory",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistory_Tasks_TaskId1",
                table: "TaskHistory",
                column: "TaskId1",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistory_User_UserId1",
                table: "TaskHistory",
                column: "UserId1",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
