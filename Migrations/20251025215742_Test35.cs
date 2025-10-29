using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test35 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationVaccineInfo_Container_ContainerId",
                table: "ImmunizationVaccineInfo");

            migrationBuilder.DropIndex(
                name: "IX_ImmunizationVaccineInfo_ContainerId",
                table: "ImmunizationVaccineInfo");

            migrationBuilder.DropColumn(
                name: "Dose",
                table: "ImmunizationVaccineLotEntry");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "ImmunizationVaccineLotEntry");

            migrationBuilder.DropColumn(
                name: "ContainerId",
                table: "ImmunizationVaccineInfo");

            migrationBuilder.AddColumn<long>(
                name: "ContainerId",
                table: "ImmunizationVaccineLotEntry",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "Dose",
                table: "ImmunizationVaccineInfo",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "ImmunizationVaccineInfo",
                type: "character varying(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationVaccineLotEntry_ContainerId",
                table: "ImmunizationVaccineLotEntry",
                column: "ContainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImmunizationVaccineLotEntry_Container_ContainerId",
                table: "ImmunizationVaccineLotEntry",
                column: "ContainerId",
                principalTable: "Container",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationVaccineLotEntry_Container_ContainerId",
                table: "ImmunizationVaccineLotEntry");

            migrationBuilder.DropIndex(
                name: "IX_ImmunizationVaccineLotEntry_ContainerId",
                table: "ImmunizationVaccineLotEntry");

            migrationBuilder.DropColumn(
                name: "ContainerId",
                table: "ImmunizationVaccineLotEntry");

            migrationBuilder.DropColumn(
                name: "Dose",
                table: "ImmunizationVaccineInfo");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "ImmunizationVaccineInfo");

            migrationBuilder.AddColumn<int>(
                name: "Dose",
                table: "ImmunizationVaccineLotEntry",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "ImmunizationVaccineLotEntry",
                type: "character varying(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "ContainerId",
                table: "ImmunizationVaccineInfo",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationVaccineInfo_ContainerId",
                table: "ImmunizationVaccineInfo",
                column: "ContainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImmunizationVaccineInfo_Container_ContainerId",
                table: "ImmunizationVaccineInfo",
                column: "ContainerId",
                principalTable: "Container",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
