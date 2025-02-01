using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFilesCompiler.Migrations
{
    /// <inheritdoc />
    public partial class Test16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventStartTimeDay1",
                table: "EventStartEndTimeDayWise",
                newName: "EventStartTime");

            migrationBuilder.RenameColumn(
                name: "EventEndTimeDay1",
                table: "EventStartEndTimeDayWise",
                newName: "EventEndTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventStartTime",
                table: "EventStartEndTimeDayWise",
                newName: "EventStartTimeDay1");

            migrationBuilder.RenameColumn(
                name: "EventEndTime",
                table: "EventStartEndTimeDayWise",
                newName: "EventEndTimeDay1");
        }
    }
}
