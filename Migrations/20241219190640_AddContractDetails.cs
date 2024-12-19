using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFilesCompiler.Migrations
{
    /// <inheritdoc />
    public partial class AddContractDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContractDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractID = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    ContractAgency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContractServiceBranch = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContractComponent = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContractClient = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContractType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DawsonRoleOnContract = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContractStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContractStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContractEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KoLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KoFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KOPhone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    KOPhone2 = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    KOEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KONotes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CORLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CORPrefix = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    CORFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CORKORank = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CORPhone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    CORPhone2 = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    COREmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CORNotes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DawsonProgramManagerLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DawsonProgramManagerFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DawsonDeputyProgramManagerLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DawsonDeputyProgramManagerFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DawsonProjectManagerLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DawsonProjectManagerFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AddedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractDetails", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractDetails");
        }
    }
}
