using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmintakProduction.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskAssignedUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_User_AssignedToUserId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssignedToUserId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Ticket",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "Ticket",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RoleName",
                table: "Role",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "TaskAssignedUsers",
                columns: table => new
                {
                    AssignedUsersUserId = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskAssignedUsers", x => new { x.AssignedUsersUserId, x.TaskId });
                    table.ForeignKey(
                        name: "FK_TaskAssignedUsers_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskAssignedUsers_User_AssignedUsersUserId",
                        column: x => x.AssignedUsersUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Ticket",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Priority", "TaskId" },
                values: new object[] { 1, null });

            migrationBuilder.UpdateData(
                table: "Ticket",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Priority", "TaskId" },
                values: new object[] { 1, null });

            migrationBuilder.UpdateData(
                table: "Ticket",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Priority", "TaskId" },
                values: new object[] { 1, null });

            migrationBuilder.UpdateData(
                table: "Ticket",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Priority", "TaskId" },
                values: new object[] { 1, null });

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_TaskId",
                table: "Ticket",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignedUsers_TaskId",
                table: "TaskAssignedUsers",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Tasks_TaskId",
                table: "Ticket",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Tasks_TaskId",
                table: "Ticket");

            migrationBuilder.DropTable(
                name: "TaskAssignedUsers");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_TaskId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "Ticket");

            migrationBuilder.AddColumn<int>(
                name: "AssignedToUserId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RoleName",
                table: "Role",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedToUserId",
                table: "Tasks",
                column: "AssignedToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_User_AssignedToUserId",
                table: "Tasks",
                column: "AssignedToUserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
