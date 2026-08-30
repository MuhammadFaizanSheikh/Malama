using ExcelFilesCompiler;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260828000000_Test54_RenameDentalXRayStationToDentalXRay")]
    public class Test54_RenameDentalXRayStationToDentalXRay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DentalXRayPaImage_DentalXRayStation_DentalXRayStationId",
                table: "DentalXRayPaImage");

            migrationBuilder.DropIndex(
                name: "IX_DentalXRayPaImage_DentalXRayStationId",
                table: "DentalXRayPaImage");

            migrationBuilder.RenameColumn(
                name: "DentalXRayStationId",
                table: "DentalXRayPaImage",
                newName: "DentalXRayId");

            migrationBuilder.RenameTable(
                name: "DentalXRayStation",
                newName: "DentalXRay");

            migrationBuilder.Sql(
                """ALTER TABLE "DentalXRay" RENAME CONSTRAINT "PK_DentalXRayStation" TO "PK_DentalXRay";""");

            migrationBuilder.Sql(
                """ALTER TABLE "DentalXRay" RENAME CONSTRAINT "FK_DentalXRayStation_ServiceMembersChild_ServiceMembersChildId" TO "FK_DentalXRay_ServiceMembersChild_ServiceMembersChildId";""");

            migrationBuilder.Sql(
                """ALTER INDEX "IX_DentalXRayStation_ServiceMembersChildId" RENAME TO "IX_DentalXRay_ServiceMembersChildId";""");

            migrationBuilder.CreateIndex(
                name: "IX_DentalXRayPaImage_DentalXRayId",
                table: "DentalXRayPaImage",
                column: "DentalXRayId");

            migrationBuilder.AddForeignKey(
                name: "FK_DentalXRayPaImage_DentalXRay_DentalXRayId",
                table: "DentalXRayPaImage",
                column: "DentalXRayId",
                principalTable: "DentalXRay",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DentalXRayPaImage_DentalXRay_DentalXRayId",
                table: "DentalXRayPaImage");

            migrationBuilder.DropIndex(
                name: "IX_DentalXRayPaImage_DentalXRayId",
                table: "DentalXRayPaImage");

            migrationBuilder.RenameTable(
                name: "DentalXRay",
                newName: "DentalXRayStation");

            migrationBuilder.RenameColumn(
                name: "DentalXRayId",
                table: "DentalXRayPaImage",
                newName: "DentalXRayStationId");

            migrationBuilder.Sql(
                """ALTER TABLE "DentalXRayStation" RENAME CONSTRAINT "PK_DentalXRay" TO "PK_DentalXRayStation";""");

            migrationBuilder.Sql(
                """ALTER TABLE "DentalXRayStation" RENAME CONSTRAINT "FK_DentalXRay_ServiceMembersChild_ServiceMembersChildId" TO "FK_DentalXRayStation_ServiceMembersChild_ServiceMembersChildId";""");

            migrationBuilder.Sql(
                """ALTER INDEX "IX_DentalXRay_ServiceMembersChildId" RENAME TO "IX_DentalXRayStation_ServiceMembersChildId";""");

            migrationBuilder.CreateIndex(
                name: "IX_DentalXRayPaImage_DentalXRayStationId",
                table: "DentalXRayPaImage",
                column: "DentalXRayStationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DentalXRayPaImage_DentalXRayStation_DentalXRayStationId",
                table: "DentalXRayPaImage",
                column: "DentalXRayStationId",
                principalTable: "DentalXRayStation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
