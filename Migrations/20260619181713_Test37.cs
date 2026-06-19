using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test37 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DentalExam",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceMembersChildId = table.Column<long>(type: "bigint", nullable: false),
                    PsrUpperRight = table.Column<string>(type: "text", nullable: true),
                    PsrUpperAnterior = table.Column<string>(type: "text", nullable: true),
                    PsrUpperLeft = table.Column<string>(type: "text", nullable: true),
                    PsrLowerRight = table.Column<string>(type: "text", nullable: true),
                    PsrLowerAnterior = table.Column<string>(type: "text", nullable: true),
                    PsrLowerLeft = table.Column<string>(type: "text", nullable: true),
                    PsrCarrierRisk = table.Column<string>(type: "text", nullable: true),
                    SoftTissuesWnl = table.Column<string>(type: "text", nullable: true),
                    SoftTissuesConditionDetail = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalExam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalExam_ServiceMembersChild_ServiceMembersChildId",
                        column: x => x.ServiceMembersChildId,
                        principalTable: "ServiceMembersChild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DentalExam_ServiceMembersChildId",
                table: "DentalExam",
                column: "ServiceMembersChildId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DentalExam");
        }
    }
}
