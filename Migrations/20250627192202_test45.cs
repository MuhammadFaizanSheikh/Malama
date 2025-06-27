using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFilesCompiler.Migrations
{
    /// <inheritdoc />
    public partial class test45 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hcg",
                table: "FileData");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "FileData",
                newName: "PregnancyTestNeeded");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PregnancyTestNeeded",
                table: "FileData",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Hcg",
                table: "FileData",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
