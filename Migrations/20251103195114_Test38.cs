using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test38 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventWiseStaffSecondaryRole",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventStaffDetailId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventWiseStaffSecondaryRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventWiseStaffSecondaryRole_EventStaffDetail_EventStaffDeta~",
                        column: x => x.EventStaffDetailId,
                        principalTable: "EventStaffDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventWiseStaffSecondaryRole_EventStaffDetailId",
                table: "EventWiseStaffSecondaryRole",
                column: "EventStaffDetailId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventWiseStaffSecondaryRole");
        }
    }
}
