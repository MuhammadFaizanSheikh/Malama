using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test57 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HivBarcodeCarebill",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HivNeeded",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HivReason",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PregnancyTestNeeded",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PregnancyTestReason",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PregnancyTestResult",
                table: "LabStation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HivBarcodeCarebill",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "HivNeeded",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "HivReason",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "PregnancyTestNeeded",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "PregnancyTestReason",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "PregnancyTestResult",
                table: "LabStation");
        }
    }
}
