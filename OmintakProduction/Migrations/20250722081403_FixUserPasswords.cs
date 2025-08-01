﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmintakProduction.Migrations
{
    /// <inheritdoc />
    public partial class FixUserPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$fBSTrcOEAqca4FS2enM.We5LxG4HFsp54Rk3mQgXW1CNmccnPmLuq");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$PFw30qehubrtTQbOquLUJOkz6Qbr2Tssdx1OSdIQvEPR/cfJdRXZq");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$dKdxIObeDocGpDzgCMjpNOcNr6GlzKNMZ/g5mL8z39HWOXTuXrFv2");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 4,
                column: "Password",
                value: "$2a$11$YNgrELn4X6n9erLUI/0ei.XILMDwUAGOn.jsT9avBSuIrjLqdmnRC");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 5,
                column: "Password",
                value: "$2a$11$0p80QhdwA3Xx7Efb0WHYCuAMQ8sPwrUR1vWr5tJZDk1pJ5MbIhwSu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "Admin@123");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Password",
                value: "User@123");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Password",
                value: "User@123");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 4,
                column: "Password",
                value: "User@123");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 5,
                column: "Password",
                value: "User@123");
        }
    }
}
