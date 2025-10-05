using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "PregnancyCheckboxSelected",
                table: "ImmunizationStation",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "PregnancyCheckboxSelected",
                table: "ImmunizationStation",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }
    }
}
