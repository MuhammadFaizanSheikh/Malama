using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostEventLabStation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceMembersChildId = table.Column<long>(type: "bigint", nullable: false),
                    G6pdResultReceived = table.Column<bool>(type: "boolean", nullable: false),
                    G6pdResultReason = table.Column<string>(type: "text", nullable: true),
                    G6pdResultReceivedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    G6pdResultMalamaUploaded = table.Column<bool>(type: "boolean", nullable: false),
                    G6pdResultMalamaUploadedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    G6pdResultEMRUploaded = table.Column<bool>(type: "boolean", nullable: false),
                    G6pdResultEMRUploadedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    G6pdResultSORUploaded = table.Column<bool>(type: "boolean", nullable: false),
                    G6pdResultSORUploadedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AboResultReceived = table.Column<bool>(type: "boolean", nullable: false),
                    AboResultReason = table.Column<string>(type: "text", nullable: true),
                    AboResultReceivedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AboResultMalamaUploaded = table.Column<bool>(type: "boolean", nullable: false),
                    AboResultMalamaUploadedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AboResultEMRUploaded = table.Column<bool>(type: "boolean", nullable: false),
                    AboResultEMRUploadedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AboResultSORUploaded = table.Column<bool>(type: "boolean", nullable: false),
                    AboResultSORUploadedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HivResultReceived = table.Column<bool>(type: "boolean", nullable: false),
                    HivResultReason = table.Column<string>(type: "text", nullable: true),
                    HivResultReceivedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HivResultMalamaUploaded = table.Column<bool>(type: "boolean", nullable: false),
                    HivResultMalamaUploadedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HivResultEMRUploaded = table.Column<bool>(type: "boolean", nullable: false),
                    HivResultEMRUploadedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HivResultSORUploaded = table.Column<bool>(type: "boolean", nullable: false),
                    HivResultSORUploadedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PregnancyResultReceived = table.Column<bool>(type: "boolean", nullable: false),
                    PregnancyResultReason = table.Column<string>(type: "text", nullable: true),
                    PregnancyResultReceivedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PregnancyResultMalamaUploaded = table.Column<bool>(type: "boolean", nullable: false),
                    PregnancyResultMalamaUploadedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PregnancyResultEMRUploaded = table.Column<bool>(type: "boolean", nullable: false),
                    PregnancyResultEMRUploadedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PregnancyResultSORUploaded = table.Column<bool>(type: "boolean", nullable: false),
                    PregnancyResultSORUploadedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    PostEventManagementId = table.Column<long>(type: "bigint", nullable: true),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostEventLabStation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostEventLabStation_PostEventManagement_PostEventManagement~",
                        column: x => x.PostEventManagementId,
                        principalTable: "PostEventManagement",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PostEventLabStation_ServiceMembersChild_ServiceMembersChild~",
                        column: x => x.ServiceMembersChildId,
                        principalTable: "ServiceMembersChild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostEventLabStation_PostEventManagementId",
                table: "PostEventLabStation",
                column: "PostEventManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_PostEventLabStation_ServiceMembersChildId",
                table: "PostEventLabStation",
                column: "ServiceMembersChildId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostEventLabStation");
        }
    }
}
