using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test39 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DentalExamFinding",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DentalExamId = table.Column<long>(type: "bigint", nullable: false),
                    IsPrimaryTooth = table.Column<bool>(type: "boolean", nullable: false),
                    AffectedTooth = table.Column<string>(type: "text", nullable: false),
                    DiseaseConditionType = table.Column<string>(type: "text", nullable: false),
                    AffectedSurfacesJson = table.Column<string>(type: "text", nullable: true),
                    CdtCodesJson = table.Column<string>(type: "text", nullable: true),
                    CdtCodesNotes = table.Column<string>(type: "text", nullable: true),
                    DescriptionDetails = table.Column<string>(type: "text", nullable: true),
                    Classification = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalExamFinding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalExamFinding_DentalExam_DentalExamId",
                        column: x => x.DentalExamId,
                        principalTable: "DentalExam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DentalExamFinding_DentalExamId",
                table: "DentalExamFinding",
                column: "DentalExamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DentalExamFinding");
        }
    }
}
