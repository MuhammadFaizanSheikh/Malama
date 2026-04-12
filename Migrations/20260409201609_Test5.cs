using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostEventServiceDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PostEventManagementId = table.Column<long>(type: "bigint", nullable: false),
                    EventServiceDetailId = table.Column<long>(type: "bigint", nullable: false),
                    Completed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostEventServiceDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostEventServiceDetail_EventServiceDetail_EventServiceDetai~",
                        column: x => x.EventServiceDetailId,
                        principalTable: "EventServiceDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostEventServiceDetail_PostEventManagement_PostEventManagem~",
                        column: x => x.PostEventManagementId,
                        principalTable: "PostEventManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostEventServiceDetail_EventServiceDetailId",
                table: "PostEventServiceDetail",
                column: "EventServiceDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PostEventServiceDetail_PostEventManagementId",
                table: "PostEventServiceDetail",
                column: "PostEventManagementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostEventServiceDetail");
        }
    }
}
