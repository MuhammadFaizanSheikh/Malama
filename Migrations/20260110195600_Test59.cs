using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test59 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AboGivenDateTime",
                table: "LabStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "G6pdGivenDateTime",
                table: "LabStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HivGivenDateTime",
                table: "LabStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LipidPanelGivenDateTime",
                table: "LabStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PregnancyTestGivenDateTime",
                table: "LabStation",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboGivenDateTime",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "G6pdGivenDateTime",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "HivGivenDateTime",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "LipidPanelGivenDateTime",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "PregnancyTestGivenDateTime",
                table: "LabStation");
        }
    }
}
