using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test38 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DentistSignatureDateTime",
                table: "DentalExam",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DentistSignatureEntered",
                table: "DentalExam",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DentistSignatureName",
                table: "DentalExam",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinalComments",
                table: "DentalExam",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "QuestionnaireReviewed",
                table: "DentalExam",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DentistSignatureDateTime",
                table: "DentalExam");

            migrationBuilder.DropColumn(
                name: "DentistSignatureEntered",
                table: "DentalExam");

            migrationBuilder.DropColumn(
                name: "DentistSignatureName",
                table: "DentalExam");

            migrationBuilder.DropColumn(
                name: "FinalComments",
                table: "DentalExam");

            migrationBuilder.DropColumn(
                name: "QuestionnaireReviewed",
                table: "DentalExam");
        }
    }
}
