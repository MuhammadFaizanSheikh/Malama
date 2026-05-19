using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class RenameLabStatusCompletedToComplete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE "PostEventLabStation" SET "G6pdStatus" = 'Complete' WHERE "G6pdStatus" = 'Completed';
                UPDATE "PostEventLabStation" SET "AboStatus" = 'Complete' WHERE "AboStatus" = 'Completed';
                UPDATE "PostEventLabStation" SET "HivStatus" = 'Complete' WHERE "HivStatus" = 'Completed';
                UPDATE "PostEventLabStation" SET "PregnancyStatus" = 'Complete' WHERE "PregnancyStatus" = 'Completed';
                UPDATE "PostEventLabStation" SET "LipidPanelStatus" = 'Complete' WHERE "LipidPanelStatus" = 'Completed';
                UPDATE "PostEventLabStation" SET "SickleCellStatus" = 'Complete' WHERE "SickleCellStatus" = 'Completed';
                UPDATE "PostEventLabStation" SET "DnaStatus" = 'Complete' WHERE "DnaStatus" = 'Completed';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE "PostEventLabStation" SET "G6pdStatus" = 'Completed' WHERE "G6pdStatus" = 'Complete';
                UPDATE "PostEventLabStation" SET "AboStatus" = 'Completed' WHERE "AboStatus" = 'Complete';
                UPDATE "PostEventLabStation" SET "HivStatus" = 'Completed' WHERE "HivStatus" = 'Complete';
                UPDATE "PostEventLabStation" SET "PregnancyStatus" = 'Completed' WHERE "PregnancyStatus" = 'Complete';
                UPDATE "PostEventLabStation" SET "LipidPanelStatus" = 'Completed' WHERE "LipidPanelStatus" = 'Complete';
                UPDATE "PostEventLabStation" SET "SickleCellStatus" = 'Completed' WHERE "SickleCellStatus" = 'Complete';
                UPDATE "PostEventLabStation" SET "DnaStatus" = 'Completed' WHERE "DnaStatus" = 'Complete';
                """);
        }
    }
}
