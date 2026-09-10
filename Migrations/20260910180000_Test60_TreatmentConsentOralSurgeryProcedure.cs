using ExcelFilesCompiler;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260910180000_Test60_TreatmentConsentOralSurgeryProcedure")]
    public class Test60_TreatmentConsentOralSurgeryProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OralSurgeryProcedureText",
                table: "TreatmentConsent",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OralSurgeryProcedureText",
                table: "TreatmentConsent");
        }
    }
}
