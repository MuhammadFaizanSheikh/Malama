using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test37 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompletedOn",
                table: "ImmunizationStation",
                newName: "UpdatedOn");

            migrationBuilder.RenameColumn(
                name: "CompletedBy",
                table: "ImmunizationStation",
                newName: "UpdatedBy");

            migrationBuilder.AddColumn<string>(
                name: "AddedBy",
                table: "ImmunizationStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedOn",
                table: "ImmunizationStation",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddedBy",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "AddedOn",
                table: "ImmunizationStation");

            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "ImmunizationStation",
                newName: "CompletedOn");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "ImmunizationStation",
                newName: "CompletedBy");
        }
    }
}
