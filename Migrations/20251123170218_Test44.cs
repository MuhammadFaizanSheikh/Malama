using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test44 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSent",
                table: "ContainerNotification");

            migrationBuilder.RenameColumn(
                name: "DueAtUtc",
                table: "ContainerNotification",
                newName: "DueAt");

            migrationBuilder.RenameColumn(
                name: "AcknowledgedAtUtc",
                table: "ContainerNotification",
                newName: "AcknowledgedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DueAt",
                table: "ContainerNotification",
                newName: "DueAtUtc");

            migrationBuilder.RenameColumn(
                name: "AcknowledgedAt",
                table: "ContainerNotification",
                newName: "AcknowledgedAtUtc");

            migrationBuilder.AddColumn<bool>(
                name: "IsSent",
                table: "ContainerNotification",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
