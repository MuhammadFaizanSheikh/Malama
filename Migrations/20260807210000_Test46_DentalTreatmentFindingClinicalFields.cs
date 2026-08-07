using ExcelFilesCompiler;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260807210000_Test46_DentalTreatmentFindingClinicalFields")]
    public class Test46_DentalTreatmentFindingClinicalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DentalTreatmentFinding_DentalExamFinding_DentalExamFindingId",
                table: "DentalTreatmentFinding");

            migrationBuilder.AlterColumn<long>(
                name: "DentalExamFindingId",
                table: "DentalTreatmentFinding",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimaryTooth",
                table: "DentalTreatmentFinding",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AffectedTooth",
                table: "DentalTreatmentFinding",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiseaseConditionType",
                table: "DentalTreatmentFinding",
                type: "text",
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_DentalTreatmentFinding_DentalExamFinding_DentalExamFindingId",
                table: "DentalTreatmentFinding",
                column: "DentalExamFindingId",
                principalTable: "DentalExamFinding",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DentalTreatmentFinding_DentalExamFinding_DentalExamFindingId",
                table: "DentalTreatmentFinding");

            migrationBuilder.DropColumn(name: "IsPrimaryTooth", table: "DentalTreatmentFinding");
            migrationBuilder.DropColumn(name: "AffectedTooth", table: "DentalTreatmentFinding");
            migrationBuilder.DropColumn(name: "DiseaseConditionType", table: "DentalTreatmentFinding");
            migrationBuilder.DropColumn(name: "AffectedSurfacesJson", table: "DentalTreatmentFinding");
            migrationBuilder.DropColumn(name: "CdtCodesJson", table: "DentalTreatmentFinding");
            migrationBuilder.DropColumn(name: "CdtCodesNotes", table: "DentalTreatmentFinding");
            migrationBuilder.DropColumn(name: "DescriptionDetails", table: "DentalTreatmentFinding");
            migrationBuilder.DropColumn(name: "Classification", table: "DentalTreatmentFinding");

            migrationBuilder.Sql("""
                DELETE FROM "DentalTreatmentFinding" WHERE "DentalExamFindingId" IS NULL;
                """);

            migrationBuilder.AlterColumn<long>(
                name: "DentalExamFindingId",
                table: "DentalTreatmentFinding",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DentalTreatmentFinding_DentalExamFinding_DentalExamFindingId",
                table: "DentalTreatmentFinding",
                column: "DentalExamFindingId",
                principalTable: "DentalExamFinding",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
