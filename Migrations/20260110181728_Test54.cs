using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test54 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboGrouping",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AboNeeded",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AboReason",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AboRhFactor",
                table: "LabStation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboGrouping",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "AboNeeded",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "AboReason",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "AboRhFactor",
                table: "LabStation");
        }
    }
}
