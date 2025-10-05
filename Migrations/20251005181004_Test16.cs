using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImmunizationVaccineInfo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Client = table.Column<string>(type: "text", nullable: false),
                    EventId = table.Column<string>(type: "text", nullable: false),
                    EventLocation = table.Column<string>(type: "text", nullable: false),
                    EventDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ImmunizationType = table.Column<string>(type: "text", nullable: false),
                    Vaccine = table.Column<string>(type: "text", nullable: false),
                    Manufacturer = table.Column<string>(type: "text", nullable: false),
                    StartingDoses = table.Column<int>(type: "integer", nullable: false),
                    FinalDoses = table.Column<int>(type: "integer", nullable: true),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmunizationVaccineInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImmunizationVaccineLotEntry",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LotNumber = table.Column<string>(type: "text", nullable: false),
                    Expiration = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ImmunizationVaccineInfoId = table.Column<int>(type: "integer", nullable: false),
                    ImmunizationVaccineInfoId1 = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmunizationVaccineLotEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImmunizationVaccineLotEntry_ImmunizationVaccineInfo_Immuniz~",
                        column: x => x.ImmunizationVaccineInfoId1,
                        principalTable: "ImmunizationVaccineInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationVaccineLotEntry_ImmunizationVaccineInfoId1",
                table: "ImmunizationVaccineLotEntry",
                column: "ImmunizationVaccineInfoId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImmunizationVaccineLotEntry");

            migrationBuilder.DropTable(
                name: "ImmunizationVaccineInfo");
        }
    }
}
