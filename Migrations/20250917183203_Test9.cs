using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FinalTreatmentClassThreeReason",
                table: "FileData",
                newName: "FinalTreatmentClass3Reason");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FinalTreatmentClass3Reason",
                table: "FileData",
                newName: "FinalTreatmentClassThreeReason");
        }
    }
}
