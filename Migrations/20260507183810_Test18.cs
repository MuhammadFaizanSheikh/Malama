using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DNAResultEMRUploaded",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "DNAResultEMRUploadedDateTime",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "DNAResultMalamaUploaded",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "DNAResultMalamaUploadedDateTime",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "DNAResultMalamaUploadedFileName",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "DNAResultReason",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "DNAResultReceived",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "DNAResultReceivedDateTime",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "DNAResultSORUploaded",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "DNAResultSORUploadedDateTime",
                table: "PostEventLabStation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DNAResultEMRUploaded",
                table: "PostEventLabStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DNAResultEMRUploadedDateTime",
                table: "PostEventLabStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DNAResultMalamaUploaded",
                table: "PostEventLabStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DNAResultMalamaUploadedDateTime",
                table: "PostEventLabStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DNAResultMalamaUploadedFileName",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DNAResultReason",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DNAResultReceived",
                table: "PostEventLabStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DNAResultReceivedDateTime",
                table: "PostEventLabStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DNAResultSORUploaded",
                table: "PostEventLabStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DNAResultSORUploadedDateTime",
                table: "PostEventLabStation",
                type: "timestamp without time zone",
                nullable: true);
        }
    }
}
