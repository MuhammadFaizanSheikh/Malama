using ExcelFilesCompiler;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260921190000_Test64_DentalFindingTreatmentNotPossibleDetails")]
    public class Test64_DentalFindingTreatmentNotPossibleDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TreatmentNotPossibleCommandName",
                table: "DentalFinding",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TreatmentPlanNextAppointmentDate",
                table: "DentalFinding",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TreatmentNotPossibleCommandName",
                table: "DentalFinding");

            migrationBuilder.DropColumn(
                name: "TreatmentPlanNextAppointmentDate",
                table: "DentalFinding");
        }
    }
}
