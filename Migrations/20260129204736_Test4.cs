using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop old text columns
            migrationBuilder.DropColumn(name: "Triglycerides", table: "LabStation");
            migrationBuilder.DropColumn(name: "TotalCholesterolHdlRatio", table: "LabStation");
            migrationBuilder.DropColumn(name: "TotalCholesterol", table: "LabStation");
            migrationBuilder.DropColumn(name: "NonHdlCholesterol", table: "LabStation");
            migrationBuilder.DropColumn(name: "LdlHdlLipoprotiens", table: "LabStation");
            migrationBuilder.DropColumn(name: "LdlCholesterol", table: "LabStation");
            migrationBuilder.DropColumn(name: "HdlCholesterol", table: "LabStation");
            migrationBuilder.DropColumn(name: "Glucose", table: "LabStation");

            // Recreate columns with correct types
            migrationBuilder.AddColumn<int>(
                name: "Triglycerides",
                table: "LabStation",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCholesterolHdlRatio",
                table: "LabStation",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalCholesterol",
                table: "LabStation",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NonHdlCholesterol",
                table: "LabStation",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LdlHdlLipoprotiens",
                table: "LabStation",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LdlCholesterol",
                table: "LabStation",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HdlCholesterol",
                table: "LabStation",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Glucose",
                table: "LabStation",
                type: "integer",
                nullable: true);

            // New column
            migrationBuilder.AddColumn<decimal>(
                name: "A1C",
                table: "LabStation",
                type: "numeric",
                nullable: true);
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "A1C", table: "LabStation");
            migrationBuilder.DropColumn(name: "Triglycerides", table: "LabStation");
            migrationBuilder.DropColumn(name: "TotalCholesterolHdlRatio", table: "LabStation");
            migrationBuilder.DropColumn(name: "TotalCholesterol", table: "LabStation");
            migrationBuilder.DropColumn(name: "NonHdlCholesterol", table: "LabStation");
            migrationBuilder.DropColumn(name: "LdlHdlLipoprotiens", table: "LabStation");
            migrationBuilder.DropColumn(name: "LdlCholesterol", table: "LabStation");
            migrationBuilder.DropColumn(name: "HdlCholesterol", table: "LabStation");
            migrationBuilder.DropColumn(name: "Glucose", table: "LabStation");

            migrationBuilder.AddColumn<string>(
                name: "Triglycerides",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TotalCholesterolHdlRatio",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TotalCholesterol",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NonHdlCholesterol",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LdlHdlLipoprotiens",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LdlCholesterol",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HdlCholesterol",
                table: "LabStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Glucose",
                table: "LabStation",
                type: "text",
                nullable: true);
        }

    }
}
