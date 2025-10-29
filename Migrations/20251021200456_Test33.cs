using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test33 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HepABodyPart",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HepAGivenDateTime",
                table: "ImmunizationStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepASite",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepAStaffName",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepAType",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MMRBodyPart",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MMRGivenDateTime",
                table: "ImmunizationStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MMRSite",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MMRStaffName",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MMRType",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TetTdpBodyPart",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TetTdpGivenDateTime",
                table: "ImmunizationStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TetTdpSite",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TetTdpStaffName",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TetTdpType",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VaricellaBodyPart",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VaricellaGivenDateTime",
                table: "ImmunizationStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VaricellaSite",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VaricellaStaffName",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VaricellaType",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HepABodyPart",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepAGivenDateTime",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepASite",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepAStaffName",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepAType",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "MMRBodyPart",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "MMRGivenDateTime",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "MMRSite",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "MMRStaffName",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "MMRType",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "TetTdpBodyPart",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "TetTdpGivenDateTime",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "TetTdpSite",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "TetTdpStaffName",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "TetTdpType",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "VaricellaBodyPart",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "VaricellaGivenDateTime",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "VaricellaSite",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "VaricellaStaffName",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "VaricellaType",
                table: "ImmunizationStation");
        }
    }
}
