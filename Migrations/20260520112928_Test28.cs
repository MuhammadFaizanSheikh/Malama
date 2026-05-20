using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test28 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FluStatus",
                table: "PostEventImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepAStatus",
                table: "PostEventImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepBStatus",
                table: "PostEventImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MmrStatus",
                table: "PostEventImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TetTdpStatus",
                table: "PostEventImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VaricellaStatus",
                table: "PostEventImmunizationStation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FluStatus",
                table: "PostEventImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepAStatus",
                table: "PostEventImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepBStatus",
                table: "PostEventImmunizationStation");

            migrationBuilder.DropColumn(
                name: "MmrStatus",
                table: "PostEventImmunizationStation");

            migrationBuilder.DropColumn(
                name: "TetTdpStatus",
                table: "PostEventImmunizationStation");

            migrationBuilder.DropColumn(
                name: "VaricellaStatus",
                table: "PostEventImmunizationStation");
        }
    }
}
