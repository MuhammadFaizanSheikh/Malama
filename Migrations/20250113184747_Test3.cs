using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFilesCompiler.Migrations
{
    /// <inheritdoc />
    public partial class Test3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractAffiliation",
                table: "SubContractor");

            migrationBuilder.CreateTable(
                name: "ServiceTypeProvided",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubContractorId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceTypeProvidedName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubContractorInfoDtoId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypeProvided", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceTypeProvided_SubContractor_SubContractorInfoDtoId",
                        column: x => x.SubContractorInfoDtoId,
                        principalTable: "SubContractor",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTypeProvided_SubContractorInfoDtoId",
                table: "ServiceTypeProvided",
                column: "SubContractorInfoDtoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceTypeProvided");

            migrationBuilder.AddColumn<string>(
                name: "ContractAffiliation",
                table: "SubContractor",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
