using ExcelFilesCompiler;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260826010000_Test53_DentalFindingExternalAssignmentFields")]
    public class Test53_DentalFindingExternalAssignmentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalExaminerName",
                table: "DentalFinding",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExternalExamDateTime",
                table: "DentalFinding",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalDentistRemarks",
                table: "DentalFinding",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalExaminerName",
                table: "DentalFinding");

            migrationBuilder.DropColumn(
                name: "ExternalExamDateTime",
                table: "DentalFinding");

            migrationBuilder.DropColumn(
                name: "ExternalDentistRemarks",
                table: "DentalFinding");
        }
    }
}
