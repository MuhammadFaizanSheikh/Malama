using System;
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
            migrationBuilder.DropForeignKey(
                name: "FK_PostEventLabStation_PostEventManagement_PostEventManagement~",
                table: "PostEventLabStation");

            migrationBuilder.AlterColumn<long>(
                name: "PostEventManagementId",
                table: "PostEventLabStation",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

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

            migrationBuilder.AddColumn<bool>(
                name: "LipidPanelResultEMRUploaded",
                table: "PostEventLabStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LipidPanelResultEMRUploadedDateTime",
                table: "PostEventLabStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LipidPanelResultMalamaUploaded",
                table: "PostEventLabStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LipidPanelResultMalamaUploadedDateTime",
                table: "PostEventLabStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LipidPanelResultReason",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LipidPanelResultReceived",
                table: "PostEventLabStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LipidPanelResultReceivedDateTime",
                table: "PostEventLabStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LipidPanelResultSORUploaded",
                table: "PostEventLabStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LipidPanelResultSORUploadedDateTime",
                table: "PostEventLabStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SickleCellResultEMRUploaded",
                table: "PostEventLabStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SickleCellResultEMRUploadedDateTime",
                table: "PostEventLabStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SickleCellResultMalamaUploaded",
                table: "PostEventLabStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SickleCellResultMalamaUploadedDateTime",
                table: "PostEventLabStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SickleCellResultReason",
                table: "PostEventLabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SickleCellResultReceived",
                table: "PostEventLabStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SickleCellResultReceivedDateTime",
                table: "PostEventLabStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SickleCellResultSORUploaded",
                table: "PostEventLabStation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SickleCellResultSORUploadedDateTime",
                table: "PostEventLabStation",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PostEventLabStation_PostEventManagement_PostEventManagement~",
                table: "PostEventLabStation",
                column: "PostEventManagementId",
                principalTable: "PostEventManagement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostEventLabStation_PostEventManagement_PostEventManagement~",
                table: "PostEventLabStation");

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

            migrationBuilder.DropColumn(
                name: "LipidPanelResultEMRUploaded",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "LipidPanelResultEMRUploadedDateTime",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "LipidPanelResultMalamaUploaded",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "LipidPanelResultMalamaUploadedDateTime",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "LipidPanelResultReason",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "LipidPanelResultReceived",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "LipidPanelResultReceivedDateTime",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "LipidPanelResultSORUploaded",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "LipidPanelResultSORUploadedDateTime",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "SickleCellResultEMRUploaded",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "SickleCellResultEMRUploadedDateTime",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "SickleCellResultMalamaUploaded",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "SickleCellResultMalamaUploadedDateTime",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "SickleCellResultReason",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "SickleCellResultReceived",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "SickleCellResultReceivedDateTime",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "SickleCellResultSORUploaded",
                table: "PostEventLabStation");

            migrationBuilder.DropColumn(
                name: "SickleCellResultSORUploadedDateTime",
                table: "PostEventLabStation");

            migrationBuilder.AlterColumn<long>(
                name: "PostEventManagementId",
                table: "PostEventLabStation",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_PostEventLabStation_PostEventManagement_PostEventManagement~",
                table: "PostEventLabStation",
                column: "PostEventManagementId",
                principalTable: "PostEventManagement",
                principalColumn: "Id");
        }
    }
}
