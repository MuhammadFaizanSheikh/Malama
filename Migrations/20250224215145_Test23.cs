using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFilesCompiler.Migrations
{
    /// <inheritdoc />
    public partial class Test23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffLicenseDetails_StaffLicense_LicenseInfoDTOId",
                table: "StaffLicenseDetails");

            migrationBuilder.DropIndex(
                name: "IX_StaffLicenseDetails_LicenseInfoDTOId",
                table: "StaffLicenseDetails");

            migrationBuilder.DropColumn(
                name: "LicenseInfoDTOId",
                table: "StaffLicenseDetails");

            migrationBuilder.CreateIndex(
                name: "IX_StaffLicenseDetails_StaffLicenseId",
                table: "StaffLicenseDetails",
                column: "StaffLicenseId");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffLicenseDetails_StaffLicense_StaffLicenseId",
                table: "StaffLicenseDetails",
                column: "StaffLicenseId",
                principalTable: "StaffLicense",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffLicenseDetails_StaffLicense_StaffLicenseId",
                table: "StaffLicenseDetails");

            migrationBuilder.DropIndex(
                name: "IX_StaffLicenseDetails_StaffLicenseId",
                table: "StaffLicenseDetails");

            migrationBuilder.AddColumn<long>(
                name: "LicenseInfoDTOId",
                table: "StaffLicenseDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StaffLicenseDetails_LicenseInfoDTOId",
                table: "StaffLicenseDetails",
                column: "LicenseInfoDTOId");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffLicenseDetails_StaffLicense_LicenseInfoDTOId",
                table: "StaffLicenseDetails",
                column: "LicenseInfoDTOId",
                principalTable: "StaffLicense",
                principalColumn: "Id");
        }
    }
}
