using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceMembersParent_EventManagementId",
                table: "ServiceMembersParent");

            migrationBuilder.DropIndex(
                name: "IX_ServiceMembersChild_ServiceMembersParentId",
                table: "ServiceMembersChild");

            migrationBuilder.AddColumn<string>(
                name: "AddedBy",
                table: "ServiceMembersChild",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedOn",
                table: "ServiceMembersChild",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ServiceMembersChild",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "ServiceMembersChild",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceMembersParent_EventManagementId",
                table: "ServiceMembersParent",
                column: "EventManagementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceMembersChild_ServiceMembersParentId_SmId",
                table: "ServiceMembersChild",
                columns: new[] { "ServiceMembersParentId", "SmId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceMembersParent_EventManagementId",
                table: "ServiceMembersParent");

            migrationBuilder.DropIndex(
                name: "IX_ServiceMembersChild_ServiceMembersParentId_SmId",
                table: "ServiceMembersChild");

            migrationBuilder.DropColumn(
                name: "AddedBy",
                table: "ServiceMembersChild");

            migrationBuilder.DropColumn(
                name: "AddedOn",
                table: "ServiceMembersChild");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ServiceMembersChild");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "ServiceMembersChild");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceMembersParent_EventManagementId",
                table: "ServiceMembersParent",
                column: "EventManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceMembersChild_ServiceMembersParentId",
                table: "ServiceMembersChild",
                column: "ServiceMembersParentId");
        }
    }
}
