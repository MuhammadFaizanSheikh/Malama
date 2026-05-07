using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DnaGivenDateTime",
                table: "LabStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DnaNeeded",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DnaReason",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DnaSerialNo",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SickleCellGivenDateTime",
                table: "LabStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SickleCellNeeded",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SickleCellReason",
                table: "LabStation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DnaGivenDateTime",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "DnaNeeded",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "DnaReason",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "DnaSerialNo",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "SickleCellGivenDateTime",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "SickleCellNeeded",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "SickleCellReason",
                table: "LabStation");
        }
    }
}
