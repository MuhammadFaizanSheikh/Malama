using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFilesCompiler.Migrations
{
    /// <inheritdoc />
    public partial class addcheckincheckout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CheckInout",
                table: "FileData",
                newName: "CheckOut");

            migrationBuilder.AddColumn<string>(
                name: "CheckIn",
                table: "FileData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckInDateTime",
                table: "FileData",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckOutDateTime",
                table: "FileData",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckIn",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "CheckInDateTime",
                table: "FileData");

            migrationBuilder.DropColumn(
                name: "CheckOutDateTime",
                table: "FileData");

            migrationBuilder.RenameColumn(
                name: "CheckOut",
                table: "FileData",
                newName: "CheckInout");
        }
    }
}
