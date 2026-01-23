using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test61 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FluLotNo",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "FluManufacturer",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepALotNo",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepAManufacturer",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepBLotNo",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepBManufacturer",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "MMRLotNo",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "MMRManufacturer",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "TetTdpLotNo",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "TetTdpManufacturer",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "VaricellaLotNo",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "VaricellaManufacturer",
                table: "ImmunizationStation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FluLotNo",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FluManufacturer",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepALotNo",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepAManufacturer",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepBLotNo",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepBManufacturer",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MMRLotNo",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MMRManufacturer",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TetTdpLotNo",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TetTdpManufacturer",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VaricellaLotNo",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VaricellaManufacturer",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);
        }
    }
}
