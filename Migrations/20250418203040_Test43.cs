using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFilesCompiler.Migrations
{
    /// <inheritdoc />
    public partial class Test43 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaffAttributeDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffLicenseId = table.Column<long>(type: "bigint", nullable: false),
                    Attribute = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffAttributeDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffAttributeDetails_StaffLicense_StaffLicenseId",
                        column: x => x.StaffLicenseId,
                        principalTable: "StaffLicense",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaffAttributeDetails_StaffLicenseId",
                table: "StaffAttributeDetails",
                column: "StaffLicenseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffAttributeDetails");
        }
    }
}
