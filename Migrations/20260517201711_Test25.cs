using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FluDataEntered",
                table: "PostEventImmunizationStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FluDataEnteredDateTime",
                table: "PostEventImmunizationStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HepADataEntered",
                table: "PostEventImmunizationStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "HepADataEnteredDateTime",
                table: "PostEventImmunizationStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HepBDataEntered",
                table: "PostEventImmunizationStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "HepBDataEnteredDateTime",
                table: "PostEventImmunizationStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MmrDataEntered",
                table: "PostEventImmunizationStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "MmrDataEnteredDateTime",
                table: "PostEventImmunizationStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TetTdpDataEntered",
                table: "PostEventImmunizationStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TetTdpDataEnteredDateTime",
                table: "PostEventImmunizationStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VaricellaDataEntered",
                table: "PostEventImmunizationStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "VaricellaDataEnteredDateTime",
                table: "PostEventImmunizationStation",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FluDataEntered",
                table: "PostEventImmunizationStation");

            migrationBuilder.DropColumn(
                name: "FluDataEnteredDateTime",
                table: "PostEventImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepADataEntered",
                table: "PostEventImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepADataEnteredDateTime",
                table: "PostEventImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepBDataEntered",
                table: "PostEventImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepBDataEnteredDateTime",
                table: "PostEventImmunizationStation");

            migrationBuilder.DropColumn(
                name: "MmrDataEntered",
                table: "PostEventImmunizationStation");

            migrationBuilder.DropColumn(
                name: "MmrDataEnteredDateTime",
                table: "PostEventImmunizationStation");

            migrationBuilder.DropColumn(
                name: "TetTdpDataEntered",
                table: "PostEventImmunizationStation");

            migrationBuilder.DropColumn(
                name: "TetTdpDataEnteredDateTime",
                table: "PostEventImmunizationStation");

            migrationBuilder.DropColumn(
                name: "VaricellaDataEntered",
                table: "PostEventImmunizationStation");

            migrationBuilder.DropColumn(
                name: "VaricellaDataEnteredDateTime",
                table: "PostEventImmunizationStation");
        }
    }
}
