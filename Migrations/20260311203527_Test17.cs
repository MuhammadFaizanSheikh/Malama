using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventStartDate",
                table: "EventManagement",
                newName: "EventStartDateUtc");

            migrationBuilder.RenameColumn(
                name: "EventEndDate",
                table: "EventManagement",
                newName: "EventEndDateUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventStartDateUtc",
                table: "EventManagement",
                newName: "EventStartDate");

            migrationBuilder.RenameColumn(
                name: "EventEndDateUtc",
                table: "EventManagement",
                newName: "EventEndDate");
        }
    }
}
