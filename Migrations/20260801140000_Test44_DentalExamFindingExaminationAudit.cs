using ExcelFilesCompiler;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260801140000_Test44_DentalExamFindingExaminationAudit")]
    public class Test44_DentalExamFindingExaminationAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExaminationAddedBy",
                table: "DentalExamFinding",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExaminationAddedOn",
                table: "DentalExamFinding",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExaminationUpdatedBy",
                table: "DentalExamFinding",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExaminationUpdatedOn",
                table: "DentalExamFinding",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExaminationAddedBy",
                table: "DentalExamFinding");

            migrationBuilder.DropColumn(
                name: "ExaminationAddedOn",
                table: "DentalExamFinding");

            migrationBuilder.DropColumn(
                name: "ExaminationUpdatedBy",
                table: "DentalExamFinding");

            migrationBuilder.DropColumn(
                name: "ExaminationUpdatedOn",
                table: "DentalExamFinding");
        }
    }
}
