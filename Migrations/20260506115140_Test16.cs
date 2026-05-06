using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboResultMalamaUploadedFileName",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DNAResultMalamaUploadedFileName",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "G6pdResultMalamaUploadedFileName",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HivResultMalamaUploadedFileName",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LipidPanelResultMalamaUploadedFileName",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PregnancyResultMalamaUploadedFileName",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SickleCellResultMalamaUploadedFileName",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboResultMalamaUploadedFileName",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "DNAResultMalamaUploadedFileName",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "G6pdResultMalamaUploadedFileName",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "HivResultMalamaUploadedFileName",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "LipidPanelResultMalamaUploadedFileName",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "PregnancyResultMalamaUploadedFileName",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "SickleCellResultMalamaUploadedFileName",
                table: "PostEventLabStation");
        }
    }
}
