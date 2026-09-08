using ExcelFilesCompiler;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Malama.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260908180000_Test58_TreatmentConsent")]
    public class Test58_TreatmentConsent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TreatmentConsent",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceMembersChildId = table.Column<long>(type: "bigint", nullable: false),
                    IncludeQuestionnaire = table.Column<bool>(type: "boolean", nullable: false),
                    IncludeOralSurgeryForm = table.Column<bool>(type: "boolean", nullable: false),
                    IncludeDentalTreatmentConsent = table.Column<bool>(type: "boolean", nullable: false),
                    OralSurgeryDentistEventStaffIdsJson = table.Column<string>(type: "text", nullable: true),
                    DentalTreatmentDentistEventStaffIdsJson = table.Column<string>(type: "text", nullable: true),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentConsent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentConsent_ServiceMembersChild_ServiceMembersChildId",
                        column: x => x.ServiceMembersChildId,
                        principalTable: "ServiceMembersChild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentConsent_ServiceMembersChildId",
                table: "TreatmentConsent",
                column: "ServiceMembersChildId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "TreatmentConsent");
        }
    }
}
