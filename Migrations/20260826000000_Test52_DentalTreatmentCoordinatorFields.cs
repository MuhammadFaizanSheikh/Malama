using ExcelFilesCompiler;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260826000000_Test52_DentalTreatmentCoordinatorFields")]
    public class Test52_DentalTreatmentCoordinatorFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TreatmentCoordinatorUserId",
                table: "DentalTreatment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TreatmentCoordinatorDateTime",
                table: "DentalTreatment",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TreatmentCoordinatorComments",
                table: "DentalTreatment",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TreatmentCoordinatorUserId",
                table: "DentalTreatment");

            migrationBuilder.DropColumn(
                name: "TreatmentCoordinatorDateTime",
                table: "DentalTreatment");

            migrationBuilder.DropColumn(
                name: "TreatmentCoordinatorComments",
                table: "DentalTreatment");
        }
    }
}
