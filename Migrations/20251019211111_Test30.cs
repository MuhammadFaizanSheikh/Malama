using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test30 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImmunizationVaccineInfo_Container_ContainerId",
                table: "ImmunizationVaccineInfo");

            migrationBuilder.DropIndex(
                name: "IX_ImmunizationVaccineInfo_ContainerId",
                table: "ImmunizationVaccineInfo");

            migrationBuilder.DropColumn(
                name: "ContainerId",
                table: "ImmunizationVaccineInfo");
        }
    }
}
