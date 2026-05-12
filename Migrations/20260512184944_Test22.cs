using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VitalStation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceMembersChildId = table.Column<long>(type: "bigint", nullable: false),
                    Height = table.Column<decimal>(type: "numeric", nullable: true),
                    Weight = table.Column<decimal>(type: "numeric", nullable: true),
                    FinalBpStatus = table.Column<string>(type: "text", nullable: true),
                    TotalReadingsTaken = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VitalStation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VitalStation_ServiceMembersChild_ServiceMembersChildId",
                        column: x => x.ServiceMembersChildId,
                        principalTable: "ServiceMembersChild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VitalStationBloodPressureReading",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VitalStationId = table.Column<long>(type: "bigint", nullable: false),
                    ReadingNumber = table.Column<int>(type: "integer", nullable: false),
                    Systolic = table.Column<int>(type: "integer", nullable: false),
                    Diastolic = table.Column<int>(type: "integer", nullable: false),
                    ReadingStatus = table.Column<string>(type: "text", nullable: false),
                    IsRetakeRequired = table.Column<bool>(type: "boolean", nullable: false),
                    ReadingTakenAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VitalStationBloodPressureReading", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VitalStationBloodPressureReading_VitalStation_VitalStationId",
                        column: x => x.VitalStationId,
                        principalTable: "VitalStation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VitalStation_ServiceMembersChildId",
                table: "VitalStation",
                column: "ServiceMembersChildId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VitalStationBloodPressureReading_VitalStationId",
                table: "VitalStationBloodPressureReading",
                column: "VitalStationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VitalStationBloodPressureReading");

            migrationBuilder.DropTable(
                name: "VitalStation");
        }
    }
}
