using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test34 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FluBodyPartOther",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepABodyPartOther",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepBBodyPartOther",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MMRBodyPartOther",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TetTdpBodyPartOther",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VaricellaBodyPartOther",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FluBodyPartOther",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepABodyPartOther",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepBBodyPartOther",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "MMRBodyPartOther",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "TetTdpBodyPartOther",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "VaricellaBodyPartOther",
                table: "ImmunizationStation");
        }
    }
}
