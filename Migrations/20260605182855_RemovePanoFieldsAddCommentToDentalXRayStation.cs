using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class RemovePanoFieldsAddCommentToDentalXRayStation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PanoFileName",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "PanoOriginalFileName",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "PanoReason",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "PanoStatus",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "PanoUploadedDateTime",
                table: "DentalXRayStation");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "DentalXRayStation");

            migrationBuilder.AddColumn<string>(
                name: "PanoFileName",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PanoOriginalFileName",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PanoReason",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PanoStatus",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PanoUploadedDateTime",
                table: "DentalXRayStation",
                type: "timestamp without time zone",
                nullable: true);
        }
    }
}
