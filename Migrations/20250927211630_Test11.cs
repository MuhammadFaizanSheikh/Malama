using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImmunizationStation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileDataId = table.Column<long>(type: "bigint", nullable: false),
                    IsSickToday = table.Column<string>(type: "text", nullable: false),
                    IsSickTodayReason = table.Column<string>(type: "text", nullable: true),
                    HasAllergiesToMedicationFoodVaccineOrLatex = table.Column<string>(type: "text", nullable: false),
                    HasAllergiesReason = table.Column<string>(type: "text", nullable: true),
                    HadSeriousReactionAfterVaccination = table.Column<string>(type: "text", nullable: false),
                    SeriousReactionReason = table.Column<string>(type: "text", nullable: true),
                    HasLongTermHealthProblem = table.Column<string>(type: "text", nullable: false),
                    LongTermHealthProblemReason = table.Column<string>(type: "text", nullable: true),
                    HasCancerOrImmuneSystemProblem = table.Column<string>(type: "text", nullable: false),
                    CancerOrImmuneSystemReason = table.Column<string>(type: "text", nullable: true),
                    TookImmuneSuppressingMedicationRecently = table.Column<string>(type: "text", nullable: false),
                    ImmuneSuppressionReason = table.Column<string>(type: "text", nullable: true),
                    HadSeizureOrNervousSystemProblem = table.Column<string>(type: "text", nullable: false),
                    SeizureReason = table.Column<string>(type: "text", nullable: true),
                    HadBloodTransfusionOrAntiviralInPastYear = table.Column<string>(type: "text", nullable: false),
                    BloodTransfusionReason = table.Column<string>(type: "text", nullable: true),
                    IsPregnantOrCouldBePregnant = table.Column<string>(type: "text", nullable: false),
                    PregnancyCheckboxSelected = table.Column<bool>(type: "boolean", nullable: true),
                    ReceivedVaccineInPast4Weeks = table.Column<string>(type: "text", nullable: false),
                    ReceivedVaccineReason = table.Column<string>(type: "text", nullable: true),
                    HepBNeeded = table.Column<string>(type: "text", nullable: false),
                    HepBReason = table.Column<string>(type: "text", nullable: true),
                    HepBManufacturer = table.Column<string>(type: "text", nullable: true),
                    HepBLotNo = table.Column<string>(type: "text", nullable: true),
                    HepBExpirationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FluNeeded = table.Column<string>(type: "text", nullable: true),
                    FluReason = table.Column<string>(type: "text", nullable: true),
                    FluManufacturer = table.Column<string>(type: "text", nullable: true),
                    FluLotNo = table.Column<string>(type: "text", nullable: true),
                    FluExpirationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    MMRNeeded = table.Column<string>(type: "text", nullable: true),
                    MMRReason = table.Column<string>(type: "text", nullable: true),
                    MMRManufacturer = table.Column<string>(type: "text", nullable: true),
                    MMRLotNo = table.Column<string>(type: "text", nullable: true),
                    MMRExpirationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HepANeeded = table.Column<string>(type: "text", nullable: true),
                    HepAReason = table.Column<string>(type: "text", nullable: true),
                    HepAManufacturer = table.Column<string>(type: "text", nullable: true),
                    HepALotNo = table.Column<string>(type: "text", nullable: true),
                    HepAExpirationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TetTdpNeeded = table.Column<string>(type: "text", nullable: true),
                    TetTdpReason = table.Column<string>(type: "text", nullable: true),
                    TetTdpManufacturer = table.Column<string>(type: "text", nullable: true),
                    TetTdpLotNo = table.Column<string>(type: "text", nullable: true),
                    TetTdpExpirationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    VaricellaNeeded = table.Column<string>(type: "text", nullable: true),
                    VaricellaReason = table.Column<string>(type: "text", nullable: true),
                    VaricellaManufacturer = table.Column<string>(type: "text", nullable: true),
                    VaricellaLotNo = table.Column<string>(type: "text", nullable: true),
                    VaricellaExpirationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmunizationStation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImmunizationStation_FileData_FileDataId",
                        column: x => x.FileDataId,
                        principalTable: "FileData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_FileDataId",
                table: "ImmunizationStation",
                column: "FileDataId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImmunizationStation");
        }
    }
}
