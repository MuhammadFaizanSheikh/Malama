using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class AddDentalXRayStation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DentalXRayStation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceMembersChildId = table.Column<long>(type: "bigint", nullable: false),
                    AreYouPregnant = table.Column<string>(type: "text", nullable: true),
                    PregnancyApproval = table.Column<string>(type: "text", nullable: true),
                    BwxStatus = table.Column<string>(type: "text", nullable: true),
                    BwxReason = table.Column<string>(type: "text", nullable: true),
                    BwxUploadedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BwLeftMolarFileName = table.Column<string>(type: "text", nullable: true),
                    BwLeftMolarOriginalFileName = table.Column<string>(type: "text", nullable: true),
                    BwLeftMolarUploadedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BwLeftPremolarFileName = table.Column<string>(type: "text", nullable: true),
                    BwLeftPremolarOriginalFileName = table.Column<string>(type: "text", nullable: true),
                    BwLeftPremolarUploadedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BwRightMolarFileName = table.Column<string>(type: "text", nullable: true),
                    BwRightMolarOriginalFileName = table.Column<string>(type: "text", nullable: true),
                    BwRightMolarUploadedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BwRightPremolarFileName = table.Column<string>(type: "text", nullable: true),
                    BwRightPremolarOriginalFileName = table.Column<string>(type: "text", nullable: true),
                    BwRightPremolarUploadedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PaStatus = table.Column<string>(type: "text", nullable: true),
                    PaReason = table.Column<string>(type: "text", nullable: true),
                    PaUploadedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PanoStatus = table.Column<string>(type: "text", nullable: true),
                    PanoReason = table.Column<string>(type: "text", nullable: true),
                    PanoFileName = table.Column<string>(type: "text", nullable: true),
                    PanoOriginalFileName = table.Column<string>(type: "text", nullable: true),
                    PanoUploadedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalXRayStation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalXRayStation_ServiceMembersChild_ServiceMembersChildId",
                        column: x => x.ServiceMembersChildId,
                        principalTable: "ServiceMembersChild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DentalXRayPaImage",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DentalXRayStationId = table.Column<long>(type: "bigint", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: true),
                    OriginalFileName = table.Column<string>(type: "text", nullable: true),
                    UploadedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalXRayPaImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalXRayPaImage_DentalXRayStation_DentalXRayStationId",
                        column: x => x.DentalXRayStationId,
                        principalTable: "DentalXRayStation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DentalXRayPaImage_DentalXRayStationId",
                table: "DentalXRayPaImage",
                column: "DentalXRayStationId");

            migrationBuilder.CreateIndex(
                name: "IX_DentalXRayStation_ServiceMembersChildId",
                table: "DentalXRayStation",
                column: "ServiceMembersChildId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DentalXRayPaImage");

            migrationBuilder.DropTable(
                name: "DentalXRayStation");
        }
    }
}
