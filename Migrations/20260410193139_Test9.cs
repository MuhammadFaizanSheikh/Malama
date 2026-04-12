using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalServiceMember",
                table: "PostEventStartEndTimeDayWise");

            migrationBuilder.AddColumn<long>(
                name: "TotalServiceMember",
                table: "PostEventManagement",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalServiceMember",
                table: "PostEventManagement");

            migrationBuilder.AddColumn<long>(
                name: "TotalServiceMember",
                table: "PostEventStartEndTimeDayWise",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
