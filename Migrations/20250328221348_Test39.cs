using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFilesCompiler.Migrations
{
    /// <inheritdoc />
    public partial class Test39 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PreEventAvailability",
                table: "EventStaffDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EventManagementStaffAvailability",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventStaffDetailId = table.Column<long>(type: "bigint", nullable: false),
                    AvailabilityDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventManagementStaffAvailability", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventManagementStaffAvailability_EventStaffDetail_EventStaffDetailId",
                        column: x => x.EventStaffDetailId,
                        principalTable: "EventStaffDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventManagementStaffAvailability_EventStaffDetailId",
                table: "EventManagementStaffAvailability",
                column: "EventStaffDetailId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventManagementStaffAvailability");

            migrationBuilder.DropColumn(
                name: "PreEventAvailability",
                table: "EventStaffDetail");
        }
    }
}
