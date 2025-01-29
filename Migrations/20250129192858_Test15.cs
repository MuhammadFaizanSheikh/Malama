using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFilesCompiler.Migrations
{
    /// <inheritdoc />
    public partial class Test15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventEndTimeDay1",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "EventStartTimeDay1",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "ServiceMemberPercentPerDay",
                table: "EventManagement");

            migrationBuilder.CreateTable(
                name: "EventStartEndTimeDayWise",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventManagementId = table.Column<long>(type: "bigint", nullable: false),
                    EventDay = table.Column<int>(type: "int", nullable: false),
                    EventStartTimeDay1 = table.Column<TimeSpan>(type: "time", nullable: true),
                    EventEndTimeDay1 = table.Column<TimeSpan>(type: "time", nullable: true),
                    ServiceMemberPercentPerDay = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStartEndTimeDayWise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventStartEndTimeDayWise_EventManagement_EventManagementId",
                        column: x => x.EventManagementId,
                        principalTable: "EventManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventStartEndTimeDayWise_EventManagementId",
                table: "EventStartEndTimeDayWise",
                column: "EventManagementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventStartEndTimeDayWise");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EventEndTimeDay1",
                table: "EventManagement",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EventStartTimeDay1",
                table: "EventManagement",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceMemberPercentPerDay",
                table: "EventManagement",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
