using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ExcelFilesCompiler;

#nullable disable

namespace Malama.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261006200000_Test68_RemoveDentalTreatmentDentalExamId")]
    public class Test68_RemoveDentalTreatmentDentalExamId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DentalTreatment_DentalExam_DentalExamId",
                table: "DentalTreatment");

            migrationBuilder.DropIndex(
                name: "IX_DentalTreatment_DentalExamId",
                table: "DentalTreatment");

            migrationBuilder.DropColumn(
                name: "DentalExamId",
                table: "DentalTreatment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DentalExamId",
                table: "DentalTreatment",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DentalTreatment_DentalExamId",
                table: "DentalTreatment",
                column: "DentalExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_DentalTreatment_DentalExam_DentalExamId",
                table: "DentalTreatment",
                column: "DentalExamId",
                principalTable: "DentalExam",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
