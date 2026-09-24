using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ExcelFilesCompiler;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Malama.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260923233000_Test67_DentalTreatmentCoordinator")]
    public class Test67_DentalTreatmentCoordinator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "TreatmentCoordinatorAppointmentFinding");
            migrationBuilder.DropTable(name: "TreatmentCoordinatorAppointment");

            migrationBuilder.DropColumn(name: "DocumentsJson", table: "DentalTreatment");
            migrationBuilder.DropColumn(name: "TreatmentCoordinatorComments", table: "DentalTreatment");
            migrationBuilder.DropColumn(name: "TreatmentCoordinatorDateTime", table: "DentalTreatment");
            migrationBuilder.DropColumn(name: "TreatmentCoordinatorEventStaffId", table: "DentalTreatment");
            migrationBuilder.DropColumn(name: "TreatmentCoordinatorUserId", table: "DentalTreatment");

            migrationBuilder.CreateTable(
                name: "DentalTreatmentCoordinator",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceMembersChildId = table.Column<long>(type: "bigint", nullable: false),
                    IsTreatmentRequired = table.Column<bool>(type: "boolean", nullable: false),
                    DocumentsJson = table.Column<string>(type: "text", nullable: true),
                    TreatmentCoordinatorUserId = table.Column<string>(type: "text", nullable: true),
                    TreatmentCoordinatorEventStaffId = table.Column<long>(type: "bigint", nullable: true),
                    TreatmentCoordinatorDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TreatmentCoordinatorComments = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalTreatmentCoordinator", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalTreatmentCoordinator_ServiceMembersChild_ServiceMembersChildId",
                        column: x => x.ServiceMembersChildId,
                        principalTable: "ServiceMembersChild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DentalAppointment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DentalTreatmentCoordinatorId = table.Column<long>(type: "bigint", nullable: false),
                    EventStaffId = table.Column<long>(type: "bigint", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AppointmentStartTime = table.Column<string>(type: "text", nullable: false),
                    AppointmentDuration = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalAppointment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalAppointment_DentalTreatmentCoordinator_DentalTreatmentCoordinatorId",
                        column: x => x.DentalTreatmentCoordinatorId,
                        principalTable: "DentalTreatmentCoordinator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DentalAppointmentFinding",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppointmentId = table.Column<long>(type: "bigint", nullable: false),
                    DentalFindingId = table.Column<long>(type: "bigint", nullable: false),
                    FindingClientKey = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalAppointmentFinding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalAppointmentFinding_DentalAppointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "DentalAppointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DentalAppointmentFinding_DentalFinding_DentalFindingId",
                        column: x => x.DentalFindingId,
                        principalTable: "DentalFinding",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DentalTreatmentCoordinator_ServiceMembersChildId",
                table: "DentalTreatmentCoordinator",
                column: "ServiceMembersChildId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DentalAppointment_DentalTreatmentCoordinatorId",
                table: "DentalAppointment",
                column: "DentalTreatmentCoordinatorId");

            migrationBuilder.CreateIndex(
                name: "IX_DentalAppointmentFinding_AppointmentId_DentalFindingId",
                table: "DentalAppointmentFinding",
                columns: new[] { "AppointmentId", "DentalFindingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DentalAppointmentFinding_DentalFindingId",
                table: "DentalAppointmentFinding",
                column: "DentalFindingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "DentalAppointmentFinding");
            migrationBuilder.DropTable(name: "DentalAppointment");
            migrationBuilder.DropTable(name: "DentalTreatmentCoordinator");

            migrationBuilder.AddColumn<string>(name: "DocumentsJson", table: "DentalTreatment", type: "text", nullable: true);
            migrationBuilder.AddColumn<string>(name: "TreatmentCoordinatorComments", table: "DentalTreatment", type: "text", nullable: true);
            migrationBuilder.AddColumn<DateTime>(name: "TreatmentCoordinatorDateTime", table: "DentalTreatment", type: "timestamp without time zone", nullable: true);
            migrationBuilder.AddColumn<long>(name: "TreatmentCoordinatorEventStaffId", table: "DentalTreatment", type: "bigint", nullable: true);
            migrationBuilder.AddColumn<string>(name: "TreatmentCoordinatorUserId", table: "DentalTreatment", type: "text", nullable: true);

            migrationBuilder.CreateTable(
                name: "TreatmentCoordinatorAppointment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
    }
}
