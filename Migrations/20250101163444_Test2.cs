using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFilesCompiler.Migrations
{
    /// <inheritdoc />
    public partial class Test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StaffDoDID",
                table: "EventStaff",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.CreateIndex(
                name: "IX_StaffContractAffiliation_EventStaffId",
                table: "StaffContractAffiliation",
                column: "EventStaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffContractAffiliation_EventStaff_EventStaffId",
                table: "StaffContractAffiliation",
                column: "EventStaffId",
                principalTable: "EventStaff",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffContractAffiliation_EventStaff_EventStaffId",
                table: "StaffContractAffiliation");

            migrationBuilder.DropIndex(
                name: "IX_StaffContractAffiliation_EventStaffId",
                table: "StaffContractAffiliation");

            migrationBuilder.AlterColumn<string>(
                name: "StaffDoDID",
                table: "EventStaff",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);
        }
    }
}
