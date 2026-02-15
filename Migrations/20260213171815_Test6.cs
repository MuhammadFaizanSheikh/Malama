using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EventStaffDetail_EventStaffId",
                table: "EventStaffDetail",
                column: "EventStaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventStaffDetail_EventStaff_EventStaffId",
                table: "EventStaffDetail",
                column: "EventStaffId",
                principalTable: "EventStaff",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventStaffDetail_EventStaff_EventStaffId",
                table: "EventStaffDetail");

            migrationBuilder.DropIndex(
                name: "IX_EventStaffDetail_EventStaffId",
                table: "EventStaffDetail");
        }
    }
}
