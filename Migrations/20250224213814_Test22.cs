using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFilesCompiler.Migrations
{
    /// <inheritdoc />
    public partial class Test22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicenseActiveDate",
                table: "StaffLicense");

            migrationBuilder.DropColumn(
                name: "LicenseExpiryDate",
                table: "StaffLicense");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "StaffLicense");

            migrationBuilder.DropColumn(
                name: "LicenseState",
                table: "StaffLicense");

            migrationBuilder.DropColumn(
                name: "LicenseType",
                table: "StaffLicense");

            migrationBuilder.CreateTable(
                name: "StaffLicenseDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffLicenseId = table.Column<long>(type: "bigint", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LicenseState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LicenseType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LicenseActiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LicenseExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LicenseInfoDTOId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffLicenseDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffLicenseDetails_StaffLicense_LicenseInfoDTOId",
                        column: x => x.LicenseInfoDTOId,
                        principalTable: "StaffLicense",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaffLicenseDetails_LicenseInfoDTOId",
                table: "StaffLicenseDetails",
                column: "LicenseInfoDTOId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffLicenseDetails");

            migrationBuilder.AddColumn<DateTime>(
                name: "LicenseActiveDate",
                table: "StaffLicense",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LicenseExpiryDate",
                table: "StaffLicense",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "StaffLicense",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LicenseState",
                table: "StaffLicense",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LicenseType",
                table: "StaffLicense",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
