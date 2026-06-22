using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test40 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DenClass",
                table: "DentalExam",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DenClassReasonComments",
                table: "DentalExam",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PanoXRayAcknowledged",
                table: "DentalExam",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DenClass",
                table: "DentalExam");

            migrationBuilder.DropColumn(
                name: "DenClassReasonComments",
                table: "DentalExam");

            migrationBuilder.DropColumn(
                name: "PanoXRayAcknowledged",
                table: "DentalExam");
        }
    }
}
