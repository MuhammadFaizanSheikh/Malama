using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Glucose_GreaterThan600",
                table: "LabStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Glucose_LessThan20",
                table: "LabStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HdlCholesterol_GreaterThan120",
                table: "LabStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HdlCholesterol_LessThan20",
                table: "LabStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TotalCholesterol_GreaterThan400",
                table: "LabStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TotalCholesterol_LessThan100",
                table: "LabStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Triglycerides_GreaterThan500",
                table: "LabStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Triglycerides_LessThan50",
                table: "LabStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Glucose_GreaterThan600",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "Glucose_LessThan20",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "HdlCholesterol_GreaterThan120",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "HdlCholesterol_LessThan20",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "TotalCholesterol_GreaterThan400",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "TotalCholesterol_LessThan100",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "Triglycerides_GreaterThan500",
                table: "LabStation");

            migrationBuilder.DropColumn(
                name: "Triglycerides_LessThan50",
                table: "LabStation");
        }
    }
}
