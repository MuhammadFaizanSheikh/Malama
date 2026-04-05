using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SubContractor_ContractId",
                table: "SubContractor",
                column: "ContractId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubContractor_ContractDetails_ContractId",
                table: "SubContractor",
                column: "ContractId",
                principalTable: "ContractDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubContractor_ContractDetails_ContractId",
                table: "SubContractor");

            migrationBuilder.DropIndex(
                name: "IX_SubContractor_ContractId",
                table: "SubContractor");
        }
    }
}
