using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostEventManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventManagementId = table.Column<long>(type: "bigint", nullable: false),
                    EventStartDateUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EventEndDateUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PostEventNotes = table.Column<string>(type: "text", nullable: false),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostEventManagement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostEventManagement_EventManagement_EventManagementId",
                        column: x => x.EventManagementId,
                        principalTable: "EventManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostEventStartEndTimeDayWise",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PostEventManagementId = table.Column<long>(type: "bigint", nullable: false),
                    EventDay = table.Column<int>(type: "integer", nullable: false),
                    EventStartTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    EventEndTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ServiceMemberPercentPerDay = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostEventStartEndTimeDayWise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostEventStartEndTimeDayWise_PostEventManagement_PostEventM~",
                        column: x => x.PostEventManagementId,
                        principalTable: "PostEventManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostEventManagement_EventManagementId",
                table: "PostEventManagement",
                column: "EventManagementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostEventStartEndTimeDayWise_PostEventManagementId",
                table: "PostEventStartEndTimeDayWise",
                column: "PostEventManagementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostEventStartEndTimeDayWise");

            migrationBuilder.DropTable(
                name: "PostEventManagement");
        }
    }
}
