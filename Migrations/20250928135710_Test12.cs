using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompletedBy",
                table: "ImmunizationStation",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedOn",
                table: "ImmunizationStation",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedBy",
                table: "ImmunizationStation");

            migrationBuilder.DropColumn(
                name: "CompletedOn",
                table: "ImmunizationStation");
        }
    }
}
