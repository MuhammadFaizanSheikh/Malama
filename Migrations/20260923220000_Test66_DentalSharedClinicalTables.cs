using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ExcelFilesCompiler;

#nullable disable

namespace Malama.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260923220000_Test66_DentalSharedClinicalTables")]
    public class Test66_DentalSharedClinicalTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop dependents that reference DentalFinding.DentalExamId / DentalExamSelectedTooth first.
            migrationBuilder.DropTable(name: "DentalExamSelectedTooth");

            migrationBuilder.DropForeignKey(
                name: "FK_DentalFinding_DentalExam_DentalExamId",
                table: "DentalFinding");

            migrationBuilder.DropIndex(
                name: "IX_DentalFinding_DentalExamId",
                table: "DentalFinding");

            migrationBuilder.DropColumn(
                name: "DentalExamId",
                table: "DentalFinding");

            migrationBuilder.AddColumn<long>(
                name: "ServiceMembersChildId",
                table: "DentalFinding",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_DentalFinding_ServiceMembersChildId",
                table: "DentalFinding",
                column: "ServiceMembersChildId");

            migrationBuilder.AddForeignKey(
                name: "FK_DentalFinding_ServiceMembersChild_ServiceMembersChildId",
                table: "DentalFinding",
                column: "ServiceMembersChildId",
                principalTable: "ServiceMembersChild",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.DropColumn(name: "PsrUpperRight", table: "DentalExam");
            migrationBuilder.DropColumn(name: "PsrUpperAnterior", table: "DentalExam");
            migrationBuilder.DropColumn(name: "PsrUpperLeft", table: "DentalExam");
            migrationBuilder.DropColumn(name: "PsrLowerRight", table: "DentalExam");
            migrationBuilder.DropColumn(name: "PsrLowerAnterior", table: "DentalExam");
            migrationBuilder.DropColumn(name: "PsrLowerLeft", table: "DentalExam");
            migrationBuilder.DropColumn(name: "PsrCarrierRisk", table: "DentalExam");
            migrationBuilder.DropColumn(name: "SoftTissuesWnl", table: "DentalExam");
            migrationBuilder.DropColumn(name: "SoftTissuesConditionDetail", table: "DentalExam");
            migrationBuilder.DropColumn(name: "DenClass", table: "DentalExam");
            migrationBuilder.DropColumn(name: "DenClassReasonComments", table: "DentalExam");
            migrationBuilder.DropColumn(name: "PanoXRayAcknowledged", table: "DentalExam");

            migrationBuilder.AlterColumn<long>(
                name: "DentalExamId",
                table: "DentalTreatment",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateTable(
                name: "DentalPsr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceMembersChildId = table.Column<long>(type: "bigint", nullable: false),
                    PsrUpperRight = table.Column<string>(type: "text", nullable: true),
                    PsrUpperAnterior = table.Column<string>(type: "text", nullable: true),
                    PsrUpperLeft = table.Column<string>(type: "text", nullable: true),
                    PsrLowerRight = table.Column<string>(type: "text", nullable: true),
                    PsrLowerAnterior = table.Column<string>(type: "text", nullable: true),
                    PsrLowerLeft = table.Column<string>(type: "text", nullable: true),
                    PsrCarrierRisk = table.Column<string>(type: "text", nullable: true),
                    SoftTissuesWnl = table.Column<string>(type: "text", nullable: true),
                    SoftTissuesConditionDetail = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalPsr", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalPsr_ServiceMembersChild_ServiceMembersChildId",
                        column: x => x.ServiceMembersChildId,
                        principalTable: "ServiceMembersChild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DentalPsrSelectedTooth",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DentalPsrId = table.Column<long>(type: "bigint", nullable: false),
                    ToothNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalPsrSelectedTooth", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalPsrSelectedTooth_DentalPsr_DentalPsrId",
                        column: x => x.DentalPsrId,
                        principalTable: "DentalPsr",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DentalDenClass",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceMembersChildId = table.Column<long>(type: "bigint", nullable: false),
                    DenClass = table.Column<string>(type: "text", nullable: true),
                    DenClassReasonComments = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalDenClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalDenClass_ServiceMembersChild_ServiceMembersChildId",
                        column: x => x.ServiceMembersChildId,
                        principalTable: "ServiceMembersChild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DentalPanoAcknowledgement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceMembersChildId = table.Column<long>(type: "bigint", nullable: false),
                    PanoXRayAcknowledged = table.Column<bool>(type: "boolean", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: true),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalPanoAcknowledgement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalPanoAcknowledgement_ServiceMembersChild_ServiceMembersChildId",
                        column: x => x.ServiceMembersChildId,
                        principalTable: "ServiceMembersChild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DentalPsr_ServiceMembersChildId",
                table: "DentalPsr",
                column: "ServiceMembersChildId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DentalPsrSelectedTooth_DentalPsrId_ToothNumber",
                table: "DentalPsrSelectedTooth",
                columns: new[] { "DentalPsrId", "ToothNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DentalDenClass_ServiceMembersChildId",
                table: "DentalDenClass",
                column: "ServiceMembersChildId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DentalPanoAcknowledgement_ServiceMembersChildId",
                table: "DentalPanoAcknowledgement",
                column: "ServiceMembersChildId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "DentalPsrSelectedTooth");
            migrationBuilder.DropTable(name: "DentalPsr");
            migrationBuilder.DropTable(name: "DentalDenClass");
            migrationBuilder.DropTable(name: "DentalPanoAcknowledgement");

            migrationBuilder.DropForeignKey(
                name: "FK_DentalFinding_ServiceMembersChild_ServiceMembersChildId",
                table: "DentalFinding");

            migrationBuilder.DropIndex(
                name: "IX_DentalFinding_ServiceMembersChildId",
                table: "DentalFinding");

            migrationBuilder.DropColumn(
                name: "ServiceMembersChildId",
                table: "DentalFinding");

            migrationBuilder.AddColumn<long>(
                name: "DentalExamId",
                table: "DentalFinding",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(name: "PsrUpperRight", table: "DentalExam", type: "text", nullable: true);
            migrationBuilder.AddColumn<string>(name: "PsrUpperAnterior", table: "DentalExam", type: "text", nullable: true);
            migrationBuilder.AddColumn<string>(name: "PsrUpperLeft", table: "DentalExam", type: "text", nullable: true);
            migrationBuilder.AddColumn<string>(name: "PsrLowerRight", table: "DentalExam", type: "text", nullable: true);
            migrationBuilder.AddColumn<string>(name: "PsrLowerAnterior", table: "DentalExam", type: "text", nullable: true);
            migrationBuilder.AddColumn<string>(name: "PsrLowerLeft", table: "DentalExam", type: "text", nullable: true);
            migrationBuilder.AddColumn<string>(name: "PsrCarrierRisk", table: "DentalExam", type: "text", nullable: true);
            migrationBuilder.AddColumn<string>(name: "SoftTissuesWnl", table: "DentalExam", type: "text", nullable: true);
            migrationBuilder.AddColumn<string>(name: "SoftTissuesConditionDetail", table: "DentalExam", type: "text", nullable: true);
            migrationBuilder.AddColumn<string>(name: "DenClass", table: "DentalExam", type: "text", nullable: true);
            migrationBuilder.AddColumn<string>(name: "DenClassReasonComments", table: "DentalExam", type: "text", nullable: true);
            migrationBuilder.AddColumn<bool>(name: "PanoXRayAcknowledged", table: "DentalExam", type: "boolean", nullable: false, defaultValue: false);

            migrationBuilder.AlterColumn<long>(
                name: "DentalExamId",
                table: "DentalTreatment",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DentalExamSelectedTooth",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DentalExamId = table.Column<long>(type: "bigint", nullable: false),
                    ToothNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalExamSelectedTooth", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalExamSelectedTooth_DentalExam_DentalExamId",
                        column: x => x.DentalExamId,
                        principalTable: "DentalExam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
