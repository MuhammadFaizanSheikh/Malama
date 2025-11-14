using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test40 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboReason",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "AboStatus",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "AudiologistServiceCompleted",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "ClassDentalExam",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "DentalExamStatus",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "DentalTreatmentReason",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "DentalTreatmentReceived",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "DentalXrayStatus",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "DnaReason",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "DnaStatus",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "EkgNeededReason",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "EkgNeededStatus",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "FinalDentalClass",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "FinalTreatmentClass3Reason",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "FluNeededStatus",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "FluReason",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "G6pdCheckoutStatus",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "G6pdReason",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "HearingStatus",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "HepANeededStatus",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "HepAReason",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "HepBNeededStatus",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "HepBReason",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "HivBarcode",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "HivReason",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "HivStatus",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "LipidReason",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "LipidStatus",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "MmrReason",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "MmrStatus",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "OptometristServiceCompleted",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "PanoramicXray",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "PhaFollowUp",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "PhaStatus",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "SickleReason",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "SickleStatus",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "TetTdpNeededStatus",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "TetTdpReason",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "VaricellaNeededStatus",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "VaricellaReason",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "VisionStatus",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "VitalsStatus",
                table: "FileData");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboReason",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AboStatus",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AudiologistServiceCompleted",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassDentalExam",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DentalExamStatus",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DentalTreatmentReason",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DentalTreatmentReceived",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DentalXrayStatus",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DnaReason",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DnaStatus",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EkgNeededReason",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EkgNeededStatus",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinalDentalClass",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinalTreatmentClass3Reason",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FluNeededStatus",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FluReason",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "G6pdCheckoutStatus",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "G6pdReason",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HearingStatus",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepANeededStatus",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepAReason",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepBNeededStatus",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepBReason",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HivBarcode",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HivReason",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HivStatus",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LipidReason",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LipidStatus",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MmrReason",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MmrStatus",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OptometristServiceCompleted",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PanoramicXray",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhaFollowUp",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhaStatus",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SickleReason",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SickleStatus",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TetTdpNeededStatus",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TetTdpReason",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VaricellaNeededStatus",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VaricellaReason",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VisionStatus",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VitalsStatus",
                table: "FileData",
                type: "text",
                nullable: true);
        }
    }
}
