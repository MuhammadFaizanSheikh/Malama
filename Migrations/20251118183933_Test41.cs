using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test41 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FluReasonExcusedComments",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepAReasonExcusedComments",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepBReasonExcusedComments",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MMRReasonExcusedComments",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TetTdpReasonExcusedComments",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VaricellaReasonExcusedComments",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FluReasonExcusedComments",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepAReasonExcusedComments",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepBReasonExcusedComments",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "MMRReasonExcusedComments",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "TetTdpReasonExcusedComments",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "VaricellaReasonExcusedComments",
                table: "ImmunizationStation");
        }
    }
}
