using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OmintakProduction.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    AssignedToUserId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false),
                    CompletedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Ticket",
                columns: new[] { "Id", "AssignedToUserId", "CompletedAt", "CreatedAt", "Description", "DueDate", "ProjectId", "Status", "Title" },
                values: new object[,]
                {
                    { 1, null, new DateOnly(2025, 6, 26), new DateOnly(2025, 6, 26), "This is a sample ticket description.", new DateOnly(2025, 6, 26), null, "ToDo", "Sample Ticket" },
                    { 2, null, new DateOnly(2025, 6, 26), new DateOnly(2025, 6, 26), "This is a sample ticket description.", new DateOnly(2025, 6, 26), null, "In_Progress", "Sample Ticket" },
                    { 3, null, new DateOnly(2025, 6, 26), new DateOnly(2025, 6, 26), "This is a sample ticket description.", new DateOnly(2025, 6, 26), null, "In_Review", "Sample Ticket" },
                    { 4, null, new DateOnly(2025, 6, 26), new DateOnly(2025, 6, 26), "This is a sample ticket description.", new DateOnly(2025, 6, 26), null, "Done", "Sample Ticket" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ticket");
        }
    }
}
