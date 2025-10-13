using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test24 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Container",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<string>(type: "text", nullable: false),
                    ContainerName = table.Column<string>(type: "text", nullable: false),
                    ContainerTypeId = table.Column<long>(type: "bigint", nullable: false),
                    StartDateTimeUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    InitialTemperature = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrentStatus = table.Column<string>(type: "text", nullable: false),
                    NextExpectedReadingUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    MonitoringIntervalMinutes = table.Column<int>(type: "integer", nullable: false),
                    EscalationIntervalMinutes = table.Column<int>(type: "integer", nullable: false),
                    ConsecutiveNormalReadings = table.Column<int>(type: "integer", nullable: false),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Container", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Container_ContainerType_ContainerTypeId",
                        column: x => x.ContainerTypeId,
                        principalTable: "ContainerType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContainerTemperatureReading",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContainerId = table.Column<long>(type: "bigint", nullable: false),
                    ReadingTimeUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Temperature = table.Column<decimal>(type: "numeric", nullable: false),
                    IsOutOfRange = table.Column<bool>(type: "boolean", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    AttemptNumber = table.Column<int>(type: "integer", nullable: false),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerTemperatureReading", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContainerTemperatureReading_Container_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Container",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Container_ContainerTypeId",
                table: "Container",
                column: "ContainerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerTemperatureReading_ContainerId",
                table: "ContainerTemperatureReading",
                column: "ContainerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContainerTemperatureReading");

            migrationBuilder.DropTable(
                name: "Container");
        }
    }
}
