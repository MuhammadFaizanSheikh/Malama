using ExcelFilesCompiler;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260720180000_Test41_DentalExamSelectedTooth")]
    public class Test41_DentalExamSelectedTooth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DentalExamSelectedTooth",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DentalExamId = table.Column<long>(type: "bigint", nullable: false),
                    ToothNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalExamSelectedTooth", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalExamSelectedTooth_DentalExam_DentalExamId",
                        column: x => x.DentalExamId,
                        principalTable: "DentalExam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DentalExamSelectedTooth_DentalExamId_ToothNumber",
                table: "DentalExamSelectedTooth",
                columns: new[] { "DentalExamId", "ToothNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DentalExamSelectedTooth");
        }
    }
}
