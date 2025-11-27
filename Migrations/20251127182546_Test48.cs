using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test48 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffAttributeDetails_StaffQualification_StaffQualification~",
                table: "StaffAttributeDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffLicenseDetails_StaffQualification_StaffQualificationId",
                table: "StaffLicenseDetails");

            migrationBuilder.DropColumn(
                name: "StaffLicenseId",
                table: "StaffLicenseDetails");

            migrationBuilder.DropColumn(
                name: "StaffLicenseId",
                table: "StaffAttributeDetails");

            migrationBuilder.AlterColumn<long>(
                name: "StaffQualificationId",
                table: "StaffLicenseDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "StaffQualificationId",
                table: "StaffAttributeDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffAttributeDetails_StaffQualification_StaffQualification~",
                table: "StaffAttributeDetails",
                column: "StaffQualificationId",
                principalTable: "StaffQualification",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffLicenseDetails_StaffQualification_StaffQualificationId",
                table: "StaffLicenseDetails",
                column: "StaffQualificationId",
                principalTable: "StaffQualification",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffAttributeDetails_StaffQualification_StaffQualification~",
                table: "StaffAttributeDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffLicenseDetails_StaffQualification_StaffQualificationId",
                table: "StaffLicenseDetails");

            migrationBuilder.AlterColumn<long>(
                name: "StaffQualificationId",
                table: "StaffLicenseDetails",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "StaffLicenseId",
                table: "StaffLicenseDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "StaffQualificationId",
                table: "StaffAttributeDetails",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "StaffLicenseId",
                table: "StaffAttributeDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffAttributeDetails_StaffQualification_StaffQualification~",
                table: "StaffAttributeDetails",
                column: "StaffQualificationId",
                principalTable: "StaffQualification",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffLicenseDetails_StaffQualification_StaffQualificationId",
                table: "StaffLicenseDetails",
                column: "StaffQualificationId",
                principalTable: "StaffQualification",
                principalColumn: "Id");
        }
    }
}
