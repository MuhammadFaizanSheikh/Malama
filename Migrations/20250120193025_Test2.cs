using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFilesCompiler.Migrations
{
    /// <inheritdoc />
    public partial class Test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubEventID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaskForce = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventAddress1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventAddress2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EventEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EventStartTimeDay1 = table.Column<TimeSpan>(type: "time", nullable: true),
                    EventEndTimeDay1 = table.Column<TimeSpan>(type: "time", nullable: true),
                    Deploy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MOBDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegardingSites = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainPocLastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainPocFirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainPocRank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainPocPhonePrimary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainPocPhoneSecondary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainPocEmailPrimary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainPocEmailSecondary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondaryPocLastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondaryPocFirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondaryPocRank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondaryPocPhonePrimary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondaryPocPhoneSecondary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondaryPocEmailPrimary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondaryPocEmailSecondary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventManagement", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventManagement");
        }
    }
}
