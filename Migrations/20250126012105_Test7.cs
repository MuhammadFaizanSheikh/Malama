using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFilesCompiler.Migrations
{
    /// <inheritdoc />
    public partial class Test7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HIVDropOffStaffLastName",
                table: "EventManagement",
                newName: "AddtionalAlternatePocRole");

            migrationBuilder.RenameColumn(
                name: "HIVDropOffStaffFirstName",
                table: "EventManagement",
                newName: "AddtionalAlternatePocRank");

            migrationBuilder.AlterColumn<long>(
                name: "HIVDropOffStaffId",
                table: "EventManagement",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AddAddtionalAlternatePoc",
                table: "EventManagement",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AddtionalAlternatePocEmailPrimary",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddtionalAlternatePocEmailSecondary",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddtionalAlternatePocFirstName",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddtionalAlternatePocLastName",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddtionalAlternatePocPhonePrimary",
                table: "EventManagement",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddtionalAlternatePocPhoneSecondary",
                table: "EventManagement",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ContractId",
                table: "EventManagement",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddAddtionalAlternatePoc",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "AddtionalAlternatePocEmailPrimary",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "AddtionalAlternatePocEmailSecondary",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "AddtionalAlternatePocFirstName",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "AddtionalAlternatePocLastName",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "AddtionalAlternatePocPhonePrimary",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "AddtionalAlternatePocPhoneSecondary",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "EventManagement");

            migrationBuilder.RenameColumn(
                name: "AddtionalAlternatePocRole",
                table: "EventManagement",
                newName: "HIVDropOffStaffLastName");

            migrationBuilder.RenameColumn(
                name: "AddtionalAlternatePocRank",
                table: "EventManagement",
                newName: "HIVDropOffStaffFirstName");

            migrationBuilder.AlterColumn<string>(
                name: "HIVDropOffStaffId",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
