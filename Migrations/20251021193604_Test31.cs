using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test31 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Dose",
                table: "ImmunizationVaccineLotEntry",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "ImmunizationVaccineLotEntry",
                type: "character varying(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HepBBodyPart",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HepBGivenDateTime",
                table: "ImmunizationStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepBSite",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepBStaffName",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepBType",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dose",
                table: "ImmunizationVaccineLotEntry");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "ImmunizationVaccineLotEntry");

            migrationBuilder.DropColumn(
                name: "HepBBodyPart",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepBGivenDateTime",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepBSite",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepBStaffName",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepBType",
                table: "ImmunizationStation");
        }
    }
}
