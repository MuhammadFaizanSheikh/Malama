using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CheckOutDateTime",
                table: "FileData",
                newName: "CheckOutTime");

            migrationBuilder.RenameColumn(
                name: "CheckInDateTime",
                table: "FileData",
                newName: "CheckInTime");

            migrationBuilder.AddColumn<string>(
                name: "CheckInBy",
                table: "FileData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckOutBy",
                table: "FileData",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckInBy",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "CheckOutBy",
                table: "FileData");

            migrationBuilder.RenameColumn(
                name: "CheckOutTime",
                table: "FileData",
                newName: "CheckOutDateTime");

            migrationBuilder.RenameColumn(
                name: "CheckInTime",
                table: "FileData",
                newName: "CheckInDateTime");
        }
    }
}
