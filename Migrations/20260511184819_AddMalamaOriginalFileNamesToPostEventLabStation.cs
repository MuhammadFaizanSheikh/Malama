using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class AddMalamaOriginalFileNamesToPostEventLabStation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboResultMalamaUploadedOriginalFileName",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "G6pdResultMalamaUploadedOriginalFileName",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HivResultMalamaUploadedOriginalFileName",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LipidPanelResultMalamaUploadedOriginalFileName",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PregnancyResultMalamaUploadedOriginalFileName",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SickleCellResultMalamaUploadedOriginalFileName",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboResultMalamaUploadedOriginalFileName",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "G6pdResultMalamaUploadedOriginalFileName",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "HivResultMalamaUploadedOriginalFileName",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "LipidPanelResultMalamaUploadedOriginalFileName",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "PregnancyResultMalamaUploadedOriginalFileName",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "SickleCellResultMalamaUploadedOriginalFileName",
                table: "PostEventLabStation");
        }
    }
}
