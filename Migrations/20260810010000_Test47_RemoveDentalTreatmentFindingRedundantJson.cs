using ExcelFilesCompiler;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260810010000_Test47_RemoveDentalTreatmentFindingRedundantJson")]
    public class Test47_RemoveDentalTreatmentFindingRedundantJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AffectedSurfacesJson",
                table: "DentalTreatmentFinding");

            migrationBuilder.DropColumn(
                name: "CdtCodesJson",
                table: "DentalTreatmentFinding");

            migrationBuilder.DropColumn(
                name: "CdtCodesNotes",
                table: "DentalTreatmentFinding");

            migrationBuilder.DropColumn(
                name: "DescriptionDetails",
                table: "DentalTreatmentFinding");

            migrationBuilder.DropColumn(
                name: "Classification",
                table: "DentalTreatmentFinding");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AffectedSurfacesJson",
                table: "DentalTreatmentFinding",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CdtCodesJson",
                table: "DentalTreatmentFinding",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CdtCodesNotes",
                table: "DentalTreatmentFinding",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionDetails",
                table: "DentalTreatmentFinding",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Classification",
                table: "DentalTreatmentFinding",
                type: "text",
                nullable: true);
        }
    }
}
