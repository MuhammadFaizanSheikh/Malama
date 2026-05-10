using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SickleCellResultEMRUploadedDateTime",
                table: "PostEventLabStation",
                newName: "SickleCellResultHRRUploadedDateTime");

            migrationBuilder.RenameColumn(
                name: "SickleCellResultEMRUploaded",
                table: "PostEventLabStation",
                newName: "SickleCellResultHRRUploaded");

            migrationBuilder.RenameColumn(
                name: "PregnancyResultEMRUploadedDateTime",
                table: "PostEventLabStation",
                newName: "PregnancyResultHRRUploadedDateTime");

            migrationBuilder.RenameColumn(
                name: "PregnancyResultEMRUploaded",
                table: "PostEventLabStation",
                newName: "PregnancyResultHRRUploaded");

            migrationBuilder.RenameColumn(
                name: "LipidPanelResultEMRUploadedDateTime",
                table: "PostEventLabStation",
                newName: "LipidPanelResultHRRUploadedDateTime");

            migrationBuilder.RenameColumn(
                name: "LipidPanelResultEMRUploaded",
                table: "PostEventLabStation",
                newName: "LipidPanelResultHRRUploaded");

            migrationBuilder.RenameColumn(
                name: "HivResultEMRUploadedDateTime",
                table: "PostEventLabStation",
                newName: "HivResultHRRUploadedDateTime");

            migrationBuilder.RenameColumn(
                name: "HivResultEMRUploaded",
                table: "PostEventLabStation",
                newName: "HivResultHRRUploaded");

            migrationBuilder.RenameColumn(
                name: "G6pdResultEMRUploadedDateTime",
                table: "PostEventLabStation",
                newName: "G6pdResultHRRUploadedDateTime");

            migrationBuilder.RenameColumn(
                name: "G6pdResultEMRUploaded",
                table: "PostEventLabStation",
                newName: "G6pdResultHRRUploaded");

            migrationBuilder.RenameColumn(
                name: "AboResultEMRUploadedDateTime",
                table: "PostEventLabStation",
                newName: "AboResultHRRUploadedDateTime");

            migrationBuilder.RenameColumn(
                name: "AboResultEMRUploaded",
                table: "PostEventLabStation",
                newName: "AboResultHRRUploaded");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SickleCellResultHRRUploadedDateTime",
                table: "PostEventLabStation",
                newName: "SickleCellResultEMRUploadedDateTime");

            migrationBuilder.RenameColumn(
                name: "SickleCellResultHRRUploaded",
                table: "PostEventLabStation",
                newName: "SickleCellResultEMRUploaded");

            migrationBuilder.RenameColumn(
                name: "PregnancyResultHRRUploadedDateTime",
                table: "PostEventLabStation",
                newName: "PregnancyResultEMRUploadedDateTime");

            migrationBuilder.RenameColumn(
                name: "PregnancyResultHRRUploaded",
                table: "PostEventLabStation",
                newName: "PregnancyResultEMRUploaded");

            migrationBuilder.RenameColumn(
                name: "LipidPanelResultHRRUploadedDateTime",
                table: "PostEventLabStation",
                newName: "LipidPanelResultEMRUploadedDateTime");

            migrationBuilder.RenameColumn(
                name: "LipidPanelResultHRRUploaded",
                table: "PostEventLabStation",
                newName: "LipidPanelResultEMRUploaded");

            migrationBuilder.RenameColumn(
                name: "HivResultHRRUploadedDateTime",
                table: "PostEventLabStation",
                newName: "HivResultEMRUploadedDateTime");

            migrationBuilder.RenameColumn(
                name: "HivResultHRRUploaded",
                table: "PostEventLabStation",
                newName: "HivResultEMRUploaded");

            migrationBuilder.RenameColumn(
                name: "G6pdResultHRRUploadedDateTime",
                table: "PostEventLabStation",
                newName: "G6pdResultEMRUploadedDateTime");

            migrationBuilder.RenameColumn(
                name: "G6pdResultHRRUploaded",
                table: "PostEventLabStation",
                newName: "G6pdResultEMRUploaded");

            migrationBuilder.RenameColumn(
                name: "AboResultHRRUploadedDateTime",
                table: "PostEventLabStation",
                newName: "AboResultEMRUploadedDateTime");

            migrationBuilder.RenameColumn(
                name: "AboResultHRRUploaded",
                table: "PostEventLabStation",
                newName: "AboResultEMRUploaded");
        }
    }
}
