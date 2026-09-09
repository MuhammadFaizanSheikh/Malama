using ExcelFilesCompiler;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260909190000_Test59_DentalFindingTreatmentPossible")]
    public class Test59_DentalFindingTreatmentPossible : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTreatmentPossible",
                table: "DentalFinding",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "TreatmentNotPossibleReason",
                table: "DentalFinding",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTreatmentPossible",
                table: "DentalFinding");

            migrationBuilder.DropColumn(
                name: "TreatmentNotPossibleReason",
                table: "DentalFinding");
        }
    }
}
