using ExcelFilesCompiler;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260805200000_Test45_DentalTreatment")]
    public class Test45_DentalTreatment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DentalTreatment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceMembersChildId = table.Column<long>(type: "bigint", nullable: false),
                    DentalExamId = table.Column<long>(type: "bigint", nullable: false),
                    SmFinalClassification = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalTreatment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalTreatment_DentalExam_DentalExamId",
                        column: x => x.DentalExamId,
                        principalTable: "DentalExam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DentalTreatment_ServiceMembersChild_ServiceMembersChildId",
                        column: x => x.ServiceMembersChildId,
                        principalTable: "ServiceMembersChild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DentalTreatmentAnesthesia",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DentalTreatmentId = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<string>(type: "text", nullable: true),
                    CarpulesByTypeJson = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalTreatmentAnesthesia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalTreatmentAnesthesia_DentalTreatment_DentalTreatmentId",
                        column: x => x.DentalTreatmentId,
                        principalTable: "DentalTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DentalTreatmentFinding",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DentalTreatmentId = table.Column<long>(type: "bigint", nullable: false),
                    DentalExamFindingId = table.Column<long>(type: "bigint", nullable: false),
                    TreatmentCompleted = table.Column<string>(type: "text", nullable: true),
                    PostServiceTreatmentJson = table.Column<string>(type: "text", nullable: true),
                    TreatmentCdtCodesJson = table.Column<string>(type: "text", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    ProceduredDrc = table.Column<string>(type: "text", nullable: true),
                    DentistProfessional = table.Column<string>(type: "text", nullable: true),
                    TreatmentStatus = table.Column<string>(type: "text", nullable: true),
                    TreatmentDateTime = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalTreatmentFinding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalTreatmentFinding_DentalExamFinding_DentalExamFindingId",
                        column: x => x.DentalExamFindingId,
                        principalTable: "DentalExamFinding",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DentalTreatmentFinding_DentalTreatment_DentalTreatmentId",
                        column: x => x.DentalTreatmentId,
                        principalTable: "DentalTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DentalTreatmentOverallNote",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DentalTreatmentId = table.Column<long>(type: "bigint", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    Dentist = table.Column<string>(type: "text", nullable: true),
                    NoteDateTime = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalTreatmentOverallNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalTreatmentOverallNote_DentalTreatment_DentalTreatmentId",
                        column: x => x.DentalTreatmentId,
                        principalTable: "DentalTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DentalTreatmentPrescription",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DentalTreatmentId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Product = table.Column<string>(type: "text", nullable: true),
                    StartDate = table.Column<string>(type: "text", nullable: true),
                    EndDate = table.Column<string>(type: "text", nullable: true),
                    Dosage = table.Column<string>(type: "text", nullable: true),
                    Duration = table.Column<string>(type: "text", nullable: true),
                    Frequency = table.Column<string>(type: "text", nullable: true),
                    PrescribedAmount = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    PrescribedBy = table.Column<string>(type: "text", nullable: true),
                    PrescribedOn = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalTreatmentPrescription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalTreatmentPrescription_DentalTreatment_DentalTreatmentId",
                        column: x => x.DentalTreatmentId,
                        principalTable: "DentalTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DentalTreatmentSelectedTooth",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DentalTreatmentId = table.Column<long>(type: "bigint", nullable: false),
                    ToothNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalTreatmentSelectedTooth", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalTreatmentSelectedTooth_DentalTreatment_DentalTreatmentId",
                        column: x => x.DentalTreatmentId,
                        principalTable: "DentalTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DentalTreatment_DentalExamId",
                table: "DentalTreatment",
                column: "DentalExamId");

            migrationBuilder.CreateIndex(
                name: "IX_DentalTreatment_ServiceMembersChildId",
                table: "DentalTreatment",
                column: "ServiceMembersChildId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DentalTreatmentAnesthesia_DentalTreatmentId",
                table: "DentalTreatmentAnesthesia",
                column: "DentalTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DentalTreatmentFinding_DentalExamFindingId",
                table: "DentalTreatmentFinding",
                column: "DentalExamFindingId");

            migrationBuilder.CreateIndex(
                name: "IX_DentalTreatmentFinding_DentalTreatmentId_DentalExamFindingId",
                table: "DentalTreatmentFinding",
                columns: new[] { "DentalTreatmentId", "DentalExamFindingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DentalTreatmentOverallNote_DentalTreatmentId",
                table: "DentalTreatmentOverallNote",
                column: "DentalTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DentalTreatmentPrescription_DentalTreatmentId",
                table: "DentalTreatmentPrescription",
                column: "DentalTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DentalTreatmentSelectedTooth_DentalTreatmentId_ToothNumber",
                table: "DentalTreatmentSelectedTooth",
                columns: new[] { "DentalTreatmentId", "ToothNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "DentalTreatmentAnesthesia");
            migrationBuilder.DropTable(name: "DentalTreatmentFinding");
            migrationBuilder.DropTable(name: "DentalTreatmentOverallNote");
            migrationBuilder.DropTable(name: "DentalTreatmentPrescription");
            migrationBuilder.DropTable(name: "DentalTreatmentSelectedTooth");
            migrationBuilder.DropTable(name: "DentalTreatment");
        }
    }
}
