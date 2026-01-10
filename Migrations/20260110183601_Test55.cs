using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test55 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Glucose",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HdlCholesterol",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LdlCholesterol",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LipidPanelNeeded",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LipidPanelRapidTesting",
                table: "LabStation",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LipidPanelReason",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NonHdlCholesterol",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TotalCholesterol",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TotalCholesterolHdlRatio",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Triglycerides",
                table: "LabStation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Glucose",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "HdlCholesterol",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "LdlCholesterol",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "LipidPanelNeeded",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "LipidPanelRapidTesting",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "LipidPanelReason",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "NonHdlCholesterol",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "TotalCholesterol",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "TotalCholesterolHdlRatio",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "Triglycerides",
                table: "LabStation");
        }
    }
}
