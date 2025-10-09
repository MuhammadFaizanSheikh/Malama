using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationVaccineLotEntry_ImmunizationVaccineInfo_Immuniz~",
                table: "ImmunizationVaccineLotEntry");

            migrationBuilder.DropIndex(
                name: "IX_ImmunizationVaccineLotEntry_ImmunizationVaccineInfoId1",
                table: "ImmunizationVaccineLotEntry");

            migrationBuilder.DropColumn(
                name: "ImmunizationVaccineInfoId1",
                table: "ImmunizationVaccineLotEntry");

            migrationBuilder.AlterColumn<long>(
                name: "ImmunizationVaccineInfoId",
                table: "ImmunizationVaccineLotEntry",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationVaccineLotEntry_ImmunizationVaccineInfoId",
                table: "ImmunizationVaccineLotEntry",
                column: "ImmunizationVaccineInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImmunizationVaccineLotEntry_ImmunizationVaccineInfo_Immuniz~",
                table: "ImmunizationVaccineLotEntry",
                column: "ImmunizationVaccineInfoId",
                principalTable: "ImmunizationVaccineInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationVaccineLotEntry_ImmunizationVaccineInfo_Immuniz~",
                table: "ImmunizationVaccineLotEntry");

            migrationBuilder.DropIndex(
                name: "IX_ImmunizationVaccineLotEntry_ImmunizationVaccineInfoId",
                table: "ImmunizationVaccineLotEntry");

            migrationBuilder.AlterColumn<int>(
                name: "ImmunizationVaccineInfoId",
                table: "ImmunizationVaccineLotEntry",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "ImmunizationVaccineInfoId1",
                table: "ImmunizationVaccineLotEntry",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationVaccineLotEntry_ImmunizationVaccineInfoId1",
                table: "ImmunizationVaccineLotEntry",
                column: "ImmunizationVaccineInfoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ImmunizationVaccineLotEntry_ImmunizationVaccineInfo_Immuniz~",
                table: "ImmunizationVaccineLotEntry",
                column: "ImmunizationVaccineInfoId1",
                principalTable: "ImmunizationVaccineInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
