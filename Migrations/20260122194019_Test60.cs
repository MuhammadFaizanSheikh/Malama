using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test60 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FluVaccineInfoId",
                table: "ImmunizationStation",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FluVaccineLotEntryId",
                table: "ImmunizationStation",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "HepAVaccineInfoId",
                table: "ImmunizationStation",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "HepAVaccineLotEntryId",
                table: "ImmunizationStation",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "HepBVaccineInfoId",
                table: "ImmunizationStation",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "HepBVaccineLotEntryId",
                table: "ImmunizationStation",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MMRVaccineInfoId",
                table: "ImmunizationStation",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MMRVaccineLotEntryId",
                table: "ImmunizationStation",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TetTdpVaccineInfoId",
                table: "ImmunizationStation",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TetTdpVaccineLotEntryId",
                table: "ImmunizationStation",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "VaricellaVaccineInfoId",
                table: "ImmunizationStation",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "VaricellaVaccineLotEntryId",
                table: "ImmunizationStation",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_FluVaccineInfoId",
                table: "ImmunizationStation",
                column: "FluVaccineInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_FluVaccineLotEntryId",
                table: "ImmunizationStation",
                column: "FluVaccineLotEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_HepAVaccineInfoId",
                table: "ImmunizationStation",
                column: "HepAVaccineInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_HepAVaccineLotEntryId",
                table: "ImmunizationStation",
                column: "HepAVaccineLotEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_HepBVaccineInfoId",
                table: "ImmunizationStation",
                column: "HepBVaccineInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_HepBVaccineLotEntryId",
                table: "ImmunizationStation",
                column: "HepBVaccineLotEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_MMRVaccineInfoId",
                table: "ImmunizationStation",
                column: "MMRVaccineInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_MMRVaccineLotEntryId",
                table: "ImmunizationStation",
                column: "MMRVaccineLotEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_TetTdpVaccineInfoId",
                table: "ImmunizationStation",
                column: "TetTdpVaccineInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_TetTdpVaccineLotEntryId",
                table: "ImmunizationStation",
                column: "TetTdpVaccineLotEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_VaricellaVaccineInfoId",
                table: "ImmunizationStation",
                column: "VaricellaVaccineInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_VaricellaVaccineLotEntryId",
                table: "ImmunizationStation",
                column: "VaricellaVaccineLotEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineInfo_FluVaccineInfoId",
                table: "ImmunizationStation",
                column: "FluVaccineInfoId",
                principalTable: "ImmunizationVaccineInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineInfo_HepAVaccineInfo~",
                table: "ImmunizationStation",
                column: "HepAVaccineInfoId",
                principalTable: "ImmunizationVaccineInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineInfo_HepBVaccineInfo~",
                table: "ImmunizationStation",
                column: "HepBVaccineInfoId",
                principalTable: "ImmunizationVaccineInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineInfo_MMRVaccineInfoId",
                table: "ImmunizationStation",
                column: "MMRVaccineInfoId",
                principalTable: "ImmunizationVaccineInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineInfo_TetTdpVaccineIn~",
                table: "ImmunizationStation",
                column: "TetTdpVaccineInfoId",
                principalTable: "ImmunizationVaccineInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineInfo_VaricellaVaccin~",
                table: "ImmunizationStation",
                column: "VaricellaVaccineInfoId",
                principalTable: "ImmunizationVaccineInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineLotEntry_FluVaccineL~",
                table: "ImmunizationStation",
                column: "FluVaccineLotEntryId",
                principalTable: "ImmunizationVaccineLotEntry",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineLotEntry_HepAVaccine~",
                table: "ImmunizationStation",
                column: "HepAVaccineLotEntryId",
                principalTable: "ImmunizationVaccineLotEntry",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineLotEntry_HepBVaccine~",
                table: "ImmunizationStation",
                column: "HepBVaccineLotEntryId",
                principalTable: "ImmunizationVaccineLotEntry",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineLotEntry_MMRVaccineL~",
                table: "ImmunizationStation",
                column: "MMRVaccineLotEntryId",
                principalTable: "ImmunizationVaccineLotEntry",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineLotEntry_TetTdpVacci~",
                table: "ImmunizationStation",
                column: "TetTdpVaccineLotEntryId",
                principalTable: "ImmunizationVaccineLotEntry",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineLotEntry_VaricellaVa~",
                table: "ImmunizationStation",
                column: "VaricellaVaccineLotEntryId",
                principalTable: "ImmunizationVaccineLotEntry",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineInfo_FluVaccineInfoId",
                table: "ImmunizationStation");

            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineInfo_HepAVaccineInfo~",
                table: "ImmunizationStation");

            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineInfo_HepBVaccineInfo~",
                table: "ImmunizationStation");

            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineInfo_MMRVaccineInfoId",
                table: "ImmunizationStation");

            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineInfo_TetTdpVaccineIn~",
                table: "ImmunizationStation");

            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineInfo_VaricellaVaccin~",
                table: "ImmunizationStation");

            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineLotEntry_FluVaccineL~",
                table: "ImmunizationStation");

            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineLotEntry_HepAVaccine~",
                table: "ImmunizationStation");

            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineLotEntry_HepBVaccine~",
                table: "ImmunizationStation");

            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineLotEntry_MMRVaccineL~",
                table: "ImmunizationStation");

            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineLotEntry_TetTdpVacci~",
                table: "ImmunizationStation");

            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationStation_ImmunizationVaccineLotEntry_VaricellaVa~",
                table: "ImmunizationStation");

            migrationBuilder.DropIndex(
                name: "IX_ImmunizationStation_FluVaccineInfoId",
                table: "ImmunizationStation");

            migrationBuilder.DropIndex(
                name: "IX_ImmunizationStation_FluVaccineLotEntryId",
                table: "ImmunizationStation");

            migrationBuilder.DropIndex(
                name: "IX_ImmunizationStation_HepAVaccineInfoId",
                table: "ImmunizationStation");

            migrationBuilder.DropIndex(
                name: "IX_ImmunizationStation_HepAVaccineLotEntryId",
                table: "ImmunizationStation");

            migrationBuilder.DropIndex(
                name: "IX_ImmunizationStation_HepBVaccineInfoId",
                table: "ImmunizationStation");

            migrationBuilder.DropIndex(
                name: "IX_ImmunizationStation_HepBVaccineLotEntryId",
                table: "ImmunizationStation");

            migrationBuilder.DropIndex(
                name: "IX_ImmunizationStation_MMRVaccineInfoId",
                table: "ImmunizationStation");

            migrationBuilder.DropIndex(
                name: "IX_ImmunizationStation_MMRVaccineLotEntryId",
                table: "ImmunizationStation");

            migrationBuilder.DropIndex(
                name: "IX_ImmunizationStation_TetTdpVaccineInfoId",
                table: "ImmunizationStation");

            migrationBuilder.DropIndex(
                name: "IX_ImmunizationStation_TetTdpVaccineLotEntryId",
                table: "ImmunizationStation");

            migrationBuilder.DropIndex(
                name: "IX_ImmunizationStation_VaricellaVaccineInfoId",
                table: "ImmunizationStation");

            migrationBuilder.DropIndex(
                name: "IX_ImmunizationStation_VaricellaVaccineLotEntryId",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "FluVaccineInfoId",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "FluVaccineLotEntryId",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepAVaccineInfoId",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepAVaccineLotEntryId",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepBVaccineInfoId",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "HepBVaccineLotEntryId",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "MMRVaccineInfoId",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "MMRVaccineLotEntryId",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "TetTdpVaccineInfoId",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "TetTdpVaccineLotEntryId",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "VaricellaVaccineInfoId",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "VaricellaVaccineLotEntryId",
                table: "ImmunizationStation");
        }
    }
}
