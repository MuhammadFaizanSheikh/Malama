using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFilesCompiler.Migrations
{
    /// <inheritdoc />
    public partial class one : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileData",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SmId = table.Column<long>(type: "bigint", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullSsn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Last4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DodId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: true),
                    Sex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Agr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Uic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mrc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dob = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Over40 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DentalDue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DentalExam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DentalNeeded = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PanoNeeded = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BwxNeeded = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Drc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhaDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhaDue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pha = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pulhes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Vision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NearVision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Vrc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Vision2pg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Vision1mi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HearingDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hearing = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hrc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HearingProfile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LabNeeded = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Abo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AboNeeded = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dna = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SickleDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sickle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    G6pd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    G6pdDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    G6pdStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HivNextTestDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hiv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LipidNeeded = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LipidPanel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CholesterolHdlCholesterol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Framingham = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ekg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EkgNeeded = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hcg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Imm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HepB = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HepA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Flu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TetTdp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mmr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Varicella = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaskForce = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Over44 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventEndDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisionWin = table.Column<int>(type: "int", nullable: true),
                    DentalWin = table.Column<int>(type: "int", nullable: true),
                    PhaWin = table.Column<int>(type: "int", nullable: true),
                    HivWin = table.Column<int>(type: "int", nullable: true),
                    HearingWin = table.Column<int>(type: "int", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileData", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileData");
        }
    }
}
