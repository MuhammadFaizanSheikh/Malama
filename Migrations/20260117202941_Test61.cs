using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test61 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientName",
                table: "ContractDetails",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SiteId",
                table: "ContractDetails",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientName",
                table: "ContractDetails");

            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "ContractDetails");
        }
    }
}
