using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test26 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboStatus",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DnaStatus",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "G6pdStatus",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HivStatus",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LipidPanelStatus",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PregnancyStatus",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SickleCellStatus",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboStatus",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "DnaStatus",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "G6pdStatus",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "HivStatus",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "LipidPanelStatus",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "PregnancyStatus",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "SickleCellStatus",
                table: "PostEventLabStation");
        }
    }
}
