using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFilesCompiler.Migrations
{
    /// <inheritdoc />
    public partial class Test5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTypeProvided_SubContractor_SubContractorInfoDtoId",
                table: "ServiceTypeProvided");

            migrationBuilder.DropIndex(
                name: "IX_ServiceTypeProvided_SubContractorInfoDtoId",
                table: "ServiceTypeProvided");

            migrationBuilder.DropColumn(
                name: "SubContractorInfoDtoId",
                table: "ServiceTypeProvided");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTypeProvided_SubContractorId",
                table: "ServiceTypeProvided",
                column: "SubContractorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceTypeProvided_SubContractor_SubContractorId",
                table: "ServiceTypeProvided",
                column: "SubContractorId",
                principalTable: "SubContractor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTypeProvided_SubContractor_SubContractorId",
                table: "ServiceTypeProvided");

            migrationBuilder.DropIndex(
                name: "IX_ServiceTypeProvided_SubContractorId",
                table: "ServiceTypeProvided");

            migrationBuilder.AddColumn<long>(
                name: "SubContractorInfoDtoId",
                table: "ServiceTypeProvided",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTypeProvided_SubContractorInfoDtoId",
                table: "ServiceTypeProvided",
                column: "SubContractorInfoDtoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceTypeProvided_SubContractor_SubContractorInfoDtoId",
                table: "ServiceTypeProvided",
                column: "SubContractorInfoDtoId",
                principalTable: "SubContractor",
                principalColumn: "Id");
        }
    }
}
