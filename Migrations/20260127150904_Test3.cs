using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EventManagement_ContractId",
                table: "EventManagement",
                column: "ContractId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventManagement_ContractDetails_ContractId",
                table: "EventManagement",
                column: "ContractId",
                principalTable: "ContractDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventManagement_ContractDetails_ContractId",
                table: "EventManagement");

            migrationBuilder.DropIndex(
                name: "IX_EventManagement_ContractId",
                table: "EventManagement");
        }
    }
}
