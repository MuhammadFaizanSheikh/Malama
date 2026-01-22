using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test60 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AllergicToLatex",
                table: "LabStation",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AnyComplicationInBloodDrawn",
                table: "LabStation",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FeelAlright",
                table: "LabStation",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllergicToLatex",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "AnyComplicationInBloodDrawn",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "FeelAlright",
                table: "LabStation");
        }
    }
}
