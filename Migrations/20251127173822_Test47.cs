using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test47 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffAttributeDetails_StaffLicense_StaffLicenseId",
                table: "StaffAttributeDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffLicenseDetails_StaffLicense_StaffLicenseId",
                table: "StaffLicenseDetails");

            migrationBuilder.DropTable(
                name: "StaffLicense");

            migrationBuilder.DropIndex(
                name: "IX_StaffLicenseDetails_StaffLicenseId",
                table: "StaffLicenseDetails");

            migrationBuilder.DropIndex(
                name: "IX_StaffAttributeDetails_StaffLicenseId",
                table: "StaffAttributeDetails");

            migrationBuilder.AddColumn<long>(
                name: "StaffQualificationId",
                table: "StaffLicenseDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StaffQualificationId",
                table: "StaffAttributeDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StaffQualification",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventStaffId = table.Column<long>(type: "bigint", nullable: false),
                    QualificationName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffQualification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffQualification_EventStaff_EventStaffId",
                        column: x => x.EventStaffId,
                        principalTable: "EventStaff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaffLicenseDetails_StaffQualificationId",
                table: "StaffLicenseDetails",
                column: "StaffQualificationId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffAttributeDetails_StaffQualificationId",
                table: "StaffAttributeDetails",
                column: "StaffQualificationId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffQualification_EventStaffId",
                table: "StaffQualification",
                column: "EventStaffId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffAttributeDetails_StaffQualification_StaffQualification~",
                table: "StaffAttributeDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffLicenseDetails_StaffQualification_StaffQualificationId",
                table: "StaffLicenseDetails");

            migrationBuilder.DropTable(
                name: "StaffQualification");

            migrationBuilder.DropIndex(
                name: "IX_StaffLicenseDetails_StaffQualificationId",
                table: "StaffLicenseDetails");

            migrationBuilder.DropIndex(
                name: "IX_StaffAttributeDetails_StaffQualificationId",
                table: "StaffAttributeDetails");

            migrationBuilder.DropColumn(
                name: "StaffQualificationId",
                table: "StaffLicenseDetails");

            migrationBuilder.DropColumn(
                name: "StaffQualificationId",
                table: "StaffAttributeDetails");

            migrationBuilder.CreateTable(
                name: "StaffLicense",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventStaffId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffLicense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffLicense_EventStaff_EventStaffId",
                        column: x => x.EventStaffId,
                        principalTable: "EventStaff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaffLicenseDetails_StaffLicenseId",
                table: "StaffLicenseDetails",
                column: "StaffLicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffAttributeDetails_StaffLicenseId",
                table: "StaffAttributeDetails",
                column: "StaffLicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffLicense_EventStaffId",
                table: "StaffLicense",
                column: "EventStaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffAttributeDetails_StaffLicense_StaffLicenseId",
                table: "StaffAttributeDetails",
                column: "StaffLicenseId",
                principalTable: "StaffLicense",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffLicenseDetails_StaffLicense_StaffLicenseId",
                table: "StaffLicenseDetails",
                column: "StaffLicenseId",
                principalTable: "StaffLicense",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
