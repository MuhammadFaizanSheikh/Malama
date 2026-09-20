using ExcelFilesCompiler;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260920180000_Test63_DentalFindingIsTreatmentPossibleNullable")]
    public class Test63_DentalFindingIsTreatmentPossibleNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsTreatmentPossible",
                table: "DentalFinding",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"UPDATE ""DentalFinding""
                  SET ""IsTreatmentPossible"" = TRUE
                  WHERE ""IsTreatmentPossible"" IS NULL;");

            migrationBuilder.AlterColumn<bool>(
                name: "IsTreatmentPossible",
                table: "DentalFinding",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);
        }
    }
}
