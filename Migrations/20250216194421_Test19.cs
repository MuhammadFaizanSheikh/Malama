using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFilesCompiler.Migrations
{
    /// <inheritdoc />
    public partial class Test19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventStaffAssociation");

            migrationBuilder.CreateTable(
                name: "EventStaffDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventManagementId = table.Column<long>(type: "bigint", nullable: false),
                    EventStaffId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStaffDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventStaffDetail_EventManagement_EventManagementId",
                        column: x => x.EventManagementId,
                        principalTable: "EventManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventStaffDetail_EventManagementId",
                table: "EventStaffDetail",
                column: "EventManagementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventStaffDetail");

            migrationBuilder.CreateTable(
                name: "EventStaffAssociation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventManagementId = table.Column<long>(type: "bigint", nullable: false),
                    EventStaffId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStaffAssociation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventStaffAssociation_EventManagement_EventManagementId",
                        column: x => x.EventManagementId,
                        principalTable: "EventManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventStaffAssociation_EventManagementId",
                table: "EventStaffAssociation",
                column: "EventManagementId");
        }
    }
}
