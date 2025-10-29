using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test32 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FluBodyPart",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FluGivenDateTime",
                table: "ImmunizationStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FluSite",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FluStaffName",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FluType",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FluBodyPart",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "FluGivenDateTime",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "FluSite",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "FluStaffName",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "FluType",
                table: "ImmunizationStation");
        }
    }
}
