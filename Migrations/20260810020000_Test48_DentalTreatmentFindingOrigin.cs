using ExcelFilesCompiler;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260810020000_Test48_DentalTreatmentFindingOrigin")]
    public class Test48_DentalTreatmentFindingOrigin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Origin",
                table: "DentalTreatmentFinding",
                type: "text",
                nullable: false,
                defaultValue: "Exam");

            migrationBuilder.Sql("""
                UPDATE "DentalTreatmentFinding"
                SET "Origin" = CASE
                    WHEN "DentalExamFindingId" IS NULL THEN 'Treatment'
                    ELSE 'Exam'
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Origin",
                table: "DentalTreatmentFinding");
        }
    }
}
