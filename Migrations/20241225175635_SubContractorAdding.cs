using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFilesCompiler.Migrations
{
    /// <inheritdoc />
    public partial class SubContractorAdding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubContractor",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<int>(type: "int", maxLength: 13, nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContractClient = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContractType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SmallBusinessType = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ContractAffiliation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContractServiceBranch = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContractComponent = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SolicitationNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CompanyMainName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyMainAddress1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CompanyMainAddress2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CompanyMainCity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyMainState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyMainZip = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyMainLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyMainFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyMainPhone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    CompanyMainEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FinanceLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FinanceFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FinanceAddress1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FinanceAddress2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FinanceCity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FinanceState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FinanceZip = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FinancePhone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    FinanceEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EventLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EventFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EventPhone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    EventEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrainingLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrainingFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrainingPhone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    TrainingEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubContractor", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubContractor");
        }
    }
}
