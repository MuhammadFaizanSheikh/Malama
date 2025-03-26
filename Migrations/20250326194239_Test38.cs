using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFilesCompiler.Migrations
{
    /// <inheritdoc />
    public partial class Test38 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskForce",
                table: "EventManagement");

            migrationBuilder.CreateTable(
                name: "EventManagementTaskforces",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventManagementId = table.Column<long>(type: "bigint", nullable: false),
                    Taskforce = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventManagementTaskforces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventManagementTaskforces_EventManagement_EventManagementId",
                        column: x => x.EventManagementId,
                        principalTable: "EventManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventManagementTaskforces_EventManagementId",
                table: "EventManagementTaskforces",
                column: "EventManagementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventManagementTaskforces");

            migrationBuilder.AddColumn<string>(
                name: "TaskForce",
                table: "EventManagement",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
