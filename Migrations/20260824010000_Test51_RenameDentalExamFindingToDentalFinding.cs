using ExcelFilesCompiler;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260824010000_Test51_RenameDentalExamFindingToDentalFinding")]
    public class Test51_RenameDentalExamFindingToDentalFinding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DentalTreatmentFinding_DentalExamFinding_DentalExamFindingId",
                table: "DentalTreatmentFinding");

            migrationBuilder.DropForeignKey(
                name: "FK_DentalExamFinding_DentalExam_DentalExamId",
                table: "DentalExamFinding");

            migrationBuilder.DropIndex(
                name: "IX_DentalTreatmentFinding_DentalExamFindingId",
                table: "DentalTreatmentFinding");

            migrationBuilder.DropIndex(
                name: "IX_DentalTreatmentFinding_DentalTreatmentId_DentalExamFindingId",
                table: "DentalTreatmentFinding");

            migrationBuilder.DropIndex(
                name: "IX_DentalExamFinding_DentalExamId",
                table: "DentalExamFinding");

            migrationBuilder.RenameTable(
                name: "DentalExamFinding",
                newName: "DentalFinding");

            migrationBuilder.RenameColumn(
                name: "DentalExamFindingId",
                table: "DentalTreatmentFinding",
                newName: "DentalFindingId");

            migrationBuilder.Sql(
                """ALTER TABLE "DentalFinding" RENAME CONSTRAINT "PK_DentalExamFinding" TO "PK_DentalFinding";""");

            migrationBuilder.CreateIndex(
                name: "IX_DentalFinding_DentalExamId",
                table: "DentalFinding",
                column: "DentalExamId");

            migrationBuilder.CreateIndex(
                name: "IX_DentalTreatmentFinding_DentalFindingId",
                table: "DentalTreatmentFinding",
                column: "DentalFindingId");

            migrationBuilder.CreateIndex(
                name: "IX_DentalTreatmentFinding_DentalTreatmentId_DentalFindingId",
                table: "DentalTreatmentFinding",
                columns: new[] { "DentalTreatmentId", "DentalFindingId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DentalFinding_DentalExam_DentalExamId",
                table: "DentalFinding",
                column: "DentalExamId",
                principalTable: "DentalExam",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DentalTreatmentFinding_DentalFinding_DentalFindingId",
                table: "DentalTreatmentFinding",
                column: "DentalFindingId",
                principalTable: "DentalFinding",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DentalTreatmentFinding_DentalFinding_DentalFindingId",
                table: "DentalTreatmentFinding");

            migrationBuilder.DropForeignKey(
                name: "FK_DentalFinding_DentalExam_DentalExamId",
                table: "DentalFinding");

            migrationBuilder.DropIndex(
                name: "IX_DentalTreatmentFinding_DentalFindingId",
                table: "DentalTreatmentFinding");

            migrationBuilder.DropIndex(
                name: "IX_DentalTreatmentFinding_DentalTreatmentId_DentalFindingId",
                table: "DentalTreatmentFinding");

            migrationBuilder.DropIndex(
                name: "IX_DentalFinding_DentalExamId",
                table: "DentalFinding");

            migrationBuilder.RenameTable(
                name: "DentalFinding",
                newName: "DentalExamFinding");

            migrationBuilder.RenameColumn(
                name: "DentalFindingId",
                table: "DentalTreatmentFinding",
                newName: "DentalExamFindingId");

            migrationBuilder.Sql(
                """ALTER TABLE "DentalExamFinding" RENAME CONSTRAINT "PK_DentalFinding" TO "PK_DentalExamFinding";""");

            migrationBuilder.CreateIndex(
                name: "IX_DentalExamFinding_DentalExamId",
                table: "DentalExamFinding",
                column: "DentalExamId");

            migrationBuilder.CreateIndex(
                name: "IX_DentalTreatmentFinding_DentalExamFindingId",
                table: "DentalTreatmentFinding",
                column: "DentalExamFindingId");

            migrationBuilder.CreateIndex(
                name: "IX_DentalTreatmentFinding_DentalTreatmentId_DentalExamFindingId",
                table: "DentalTreatmentFinding",
                columns: new[] { "DentalTreatmentId", "DentalExamFindingId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DentalExamFinding_DentalExam_DentalExamId",
                table: "DentalExamFinding",
                column: "DentalExamId",
                principalTable: "DentalExam",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
