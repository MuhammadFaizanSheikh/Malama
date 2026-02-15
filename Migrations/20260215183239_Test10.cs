using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "EventId",
                table: "UserEventMapping",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_UserEventMapping_EventId",
                table: "UserEventMapping",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserEventMapping_EventManagement_EventId",
                table: "UserEventMapping",
                column: "EventId",
                principalTable: "EventManagement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserEventMapping_EventManagement_EventId",
                table: "UserEventMapping");

            migrationBuilder.DropIndex(
                name: "IX_UserEventMapping_EventId",
                table: "UserEventMapping");

            migrationBuilder.AlterColumn<int>(
                name: "EventId",
                table: "UserEventMapping",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
