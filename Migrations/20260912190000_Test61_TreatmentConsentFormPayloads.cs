using ExcelFilesCompiler;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260912190000_Test61_TreatmentConsentFormPayloads")]
    public class Test61_TreatmentConsentFormPayloads : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OralSurgeryFormsJson",
                table: "TreatmentConsent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DentalTreatmentFormsJson",
                table: "TreatmentConsent",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OralSurgeryFormsJson",
                table: "TreatmentConsent");

            migrationBuilder.DropColumn(
                name: "DentalTreatmentFormsJson",
                table: "TreatmentConsent");
        }
    }
}
