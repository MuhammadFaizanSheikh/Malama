using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test34 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BwxConsolidatedFileName",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BwxConsolidatedOriginalFileName",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BwxConsolidatedUploadedDateTime",
                table: "DentalXRayStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BwxUploadMode",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BwxConsolidatedFileName",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "BwxConsolidatedOriginalFileName",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "BwxConsolidatedUploadedDateTime",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "BwxUploadMode",
                table: "DentalXRayStation");
        }
    }
}
