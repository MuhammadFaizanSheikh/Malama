using ExcelFilesCompiler;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260921210000_Test65_TreatmentCoordinatorSave")]
    public class Test65_TreatmentCoordinatorSave : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientKey",
                table: "DentalFinding",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TreatmentCoordinatorEventStaffId",
                table: "DentalTreatment",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentsJson",
                table: "DentalTreatment",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TreatmentCoordinatorAppointment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DentalTreatmentId = table.Column<long>(type: "bigint", nullable: false),
                    EventStaffId = table.Column<long>(type: "bigint", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AppointmentStartTime = table.Column<string>(type: "text", nullable: false),
                    AppointmentDuration = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentCoordinatorAppointment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentCoordinatorAppointment_DentalTreatment_DentalTreatmentId",
                        column: x => x.DentalTreatmentId,
                        principalTable: "DentalTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentCoordinatorAppointmentFinding",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppointmentId = table.Column<long>(type: "bigint", nullable: false),
                    DentalFindingId = table.Column<long>(type: "bigint", nullable: false),
                    FindingClientKey = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentCoordinatorAppointmentFinding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentCoordinatorAppointmentFinding_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "TreatmentCoordinatorAppointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreatmentCoordinatorAppointmentFinding_DentalFinding_DentalFindingId",
                        column: x => x.DentalFindingId,
                        principalTable: "DentalFinding",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentCoordinatorAppointment_DentalTreatmentId",
                table: "TreatmentCoordinatorAppointment",
                column: "DentalTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentCoordinatorAppointmentFinding_AppointmentId_DentalFindingId",
                table: "TreatmentCoordinatorAppointmentFinding",
                columns: new[] { "AppointmentId", "DentalFindingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentCoordinatorAppointmentFinding_DentalFindingId",
                table: "TreatmentCoordinatorAppointmentFinding",
                column: "DentalFindingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "TreatmentCoordinatorAppointmentFinding");
            migrationBuilder.DropTable(name: "TreatmentCoordinatorAppointment");

            migrationBuilder.DropColumn(name: "DocumentsJson", table: "DentalTreatment");
            migrationBuilder.DropColumn(name: "TreatmentCoordinatorEventStaffId", table: "DentalTreatment");
            migrationBuilder.DropColumn(name: "ClientKey", table: "DentalFinding");
        }
    }
}
