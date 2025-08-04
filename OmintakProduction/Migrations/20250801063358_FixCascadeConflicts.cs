using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OmintakProduction.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeConflicts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistory_Tasks_TaskId",
                table: "TaskHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistory_Tasks_TaskId1",
                table: "TaskHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistory_User_UserId1",
                table: "TaskHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_User_AssignedToUserId",
                table: "Ticket");

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

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Ticket",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "Ticket",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Ticket",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Ticket",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamLeadId",
                table: "Team",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Tasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoldReason",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HoldRequestedByUserId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HoldStartDate",
                table: "Tasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnHold",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "UserId1",
                table: "TaskHistory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TaskId1",
                table: "TaskHistory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "RoleName",
                value: "Developer");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "RoleName",
                value: "Tester");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "RoleName",
                value: "Stakeholder");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "RoleName",
                value: "TeamLead");

            migrationBuilder.UpdateData(
                table: "Team",
                keyColumn: "TeamId",
                keyValue: 1,
                column: "TeamLeadId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Team",
                keyColumn: "TeamId",
                keyValue: 2,
                column: "TeamLeadId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Team",
                keyColumn: "TeamId",
                keyValue: 3,
                column: "TeamLeadId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Ticket",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DeletedAt", "DeletedByUserId", "IsDeleted", "UserId" },
                values: new object[] { null, null, false, null });

            migrationBuilder.UpdateData(
                table: "Ticket",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DeletedAt", "DeletedByUserId", "IsDeleted", "UserId" },
                values: new object[] { null, null, false, null });

            migrationBuilder.UpdateData(
                table: "Ticket",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DeletedAt", "DeletedByUserId", "IsDeleted", "UserId" },
                values: new object[] { null, null, false, null });

            migrationBuilder.UpdateData(
                table: "Ticket",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DeletedAt", "DeletedByUserId", "IsDeleted", "UserId" },
                values: new object[] { null, null, false, null });

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleId",
                table: "User",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_DeletedByUserId",
                table: "Ticket",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_UserId",
                table: "Ticket",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Team_TeamLeadId",
                table: "Team",
                column: "TeamLeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DeletedByUserId",
                table: "Tasks",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_HoldRequestedByUserId",
                table: "Tasks",
                column: "HoldRequestedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistory_Tasks_TaskId",
                table: "TaskHistory",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_User_DeletedByUserId",
                table: "Tasks",
                column: "DeletedByUserId",
                principalTable: "User",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_User_HoldRequestedByUserId",
                table: "Tasks",
                column: "HoldRequestedByUserId",
                principalTable: "User",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Team_User_TeamLeadId",
                table: "Team",
                column: "TeamLeadId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_User_AssignedToUserId",
                table: "Ticket",
                column: "AssignedToUserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_User_DeletedByUserId",
                table: "Ticket",
                column: "DeletedByUserId",
                principalTable: "User",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_User_UserId",
                table: "Ticket",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Role_RoleId",
                table: "User",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistory_Tasks_TaskId",
                table: "TaskHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistory_Tasks_TaskId1",
                table: "TaskHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistory_User_UserId1",
                table: "TaskHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_User_DeletedByUserId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_User_HoldRequestedByUserId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Team_User_TeamLeadId",
                table: "Team");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_User_AssignedToUserId",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_User_DeletedByUserId",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_User_UserId",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Role_RoleId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_RoleId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_DeletedByUserId",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_UserId",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Team_TeamLeadId",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_DeletedByUserId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_HoldRequestedByUserId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "TeamLeadId",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "HoldReason",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "HoldRequestedByUserId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "HoldStartDate",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "IsOnHold",
                table: "Tasks");

            migrationBuilder.AlterColumn<int>(
                name: "UserId1",
                table: "TaskHistory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TaskId1",
                table: "TaskHistory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "RoleName",
                value: "Engineer");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "RoleName",
                value: "Software Tester");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "RoleName",
                value: "ProjectLead");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "RoleName",
                value: "Developer");

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { 6, "Tester" },
                    { 7, "Stakeholder" },
                    { 8, "TeamLead" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistory_Tasks_TaskId",
                table: "TaskHistory",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistory_Tasks_TaskId1",
                table: "TaskHistory",
                column: "TaskId1",
                principalTable: "Tasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistory_User_UserId1",
                table: "TaskHistory",
                column: "UserId1",
                principalTable: "User",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_User_AssignedToUserId",
                table: "Ticket",
                column: "AssignedToUserId",
                principalTable: "User",
                principalColumn: "UserId");
        }
    }
}
