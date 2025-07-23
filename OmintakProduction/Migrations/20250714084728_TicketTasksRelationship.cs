using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmintakProduction.Migrations
{
    /// <inheritdoc />
    public partial class TicketTasksRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Tasks_TaskId",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_TaskId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "Ticket");

            migrationBuilder.AddColumn<int>(
                name: "TicketId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TicketId",
                table: "Tasks",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Ticket_TicketId",
                table: "Tasks",
                column: "TicketId",
                principalTable: "Ticket",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Ticket_TicketId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TicketId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "Ticket",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Ticket",
                keyColumn: "Id",
                keyValue: 1,
                column: "TaskId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Ticket",
                keyColumn: "Id",
                keyValue: 2,
                column: "TaskId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Ticket",
                keyColumn: "Id",
                keyValue: 3,
                column: "TaskId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Ticket",
                keyColumn: "Id",
                keyValue: 4,
                column: "TaskId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_TaskId",
                table: "Ticket",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Tasks_TaskId",
                table: "Ticket",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }
    }
}
