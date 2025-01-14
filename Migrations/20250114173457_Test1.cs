using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFilesCompiler.Migrations
{
    /// <inheritdoc />
    public partial class Test1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContractDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractID = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    ContractName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ContractAgency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContractServiceBranch = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContractComponent = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContractClient = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContractType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DawsonRoleOnContract = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContractStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContractStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContractEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KoLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KoFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KOPhone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    KOPhone2 = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    KOEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KONotes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CORLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CORPrefix = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    CORFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CORKORank = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CORPhone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    CORPhone2 = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    COREmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CORNotes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DawsonProgramManagerLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DawsonProgramManagerFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DawsonDeputyProgramManagerLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DawsonDeputyProgramManagerFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DawsonProjectManagerLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DawsonProjectManagerFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AddedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventStaff",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffID = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StaffStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StaffLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StaffFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StaffMiddleInitial = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StaffSSN = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    StaffDOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventOnCallStaff = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventOnCallStaffEvent = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NPI = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DAE = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    CredentialingProcessDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HistoricalCredentialingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DAWSONInternalCredentialingCompleteDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OnboardingTrainingComplete = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutstandingTrainings = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BackgroundCheckConcerns = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BLSCertDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BLSCertNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ACLSCertDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ACLSCertNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CACApplicationProcessStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StaffCAC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StaffDoDID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CacExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StaffCellNumber = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    StaffPhone2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrimaryAddress1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PrimaryAddress2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PrimaryCity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PrimaryState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PrimaryZip = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SecondaryAddress1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SecondaryAddress2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SecondaryCity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SecondaryState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SecondaryZip = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StaffInfoEnteredBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TravelHonorAir = table.Column<bool>(type: "bit", nullable: false),
                    TravelHonorCar = table.Column<bool>(type: "bit", nullable: false),
                    TravelHonorHotel = table.Column<bool>(type: "bit", nullable: false),
                    AddedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStaff", x => x.Id);
                });

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
                    CheckIn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CheckInDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckOut = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CheckOutDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VisionWin = table.Column<int>(type: "int", nullable: true),
                    DentalWin = table.Column<int>(type: "int", nullable: true),
                    PhaWin = table.Column<int>(type: "int", nullable: true),
                    HivWin = table.Column<int>(type: "int", nullable: true),
                    HearingWin = table.Column<int>(type: "int", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true),
                    AddedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubContractor",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SmallBusinessType = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SolicitationNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CompanyMainName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyMainAddress1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CompanyMainAddress2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CompanyMainCity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyMainState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyMainZip = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyMainLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyMainFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyMainPhone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    CompanyMainEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FinanceLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FinanceFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FinanceAddress1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FinanceAddress2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FinanceCity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FinanceState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FinanceZip = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FinancePhone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    FinanceEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EventLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EventFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EventPhone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    EventEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrainingLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrainingFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrainingPhone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    TrainingEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AddedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubContractor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffContractAffiliation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventStaffId = table.Column<long>(type: "bigint", nullable: false),
                    SubContractorId = table.Column<long>(type: "bigint", nullable: false),
                    ContractId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffContractAffiliation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffContractAffiliation_EventStaff_EventStaffId",
                        column: x => x.EventStaffId,
                        principalTable: "EventStaff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffLicense",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventStaffId = table.Column<long>(type: "bigint", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LicenseState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LicenseType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LicenseActiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LicenseExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffLicense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffLicense_EventStaff_EventStaffId",
                        column: x => x.EventStaffId,
                        principalTable: "EventStaff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TravelHonor",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventStaffId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rewards = table.Column<decimal>(type: "decimal(6,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelHonor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelHonor_EventStaff_EventStaffId",
                        column: x => x.EventStaffId,
                        principalTable: "EventStaff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypeProvided",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubContractorId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceTypeProvidedName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypeProvided", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceTypeProvided_SubContractor_SubContractorId",
                        column: x => x.SubContractorId,
                        principalTable: "SubContractor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTypeProvided_SubContractorId",
                table: "ServiceTypeProvided",
                column: "SubContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffContractAffiliation_EventStaffId",
                table: "StaffContractAffiliation",
                column: "EventStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffLicense_EventStaffId",
                table: "StaffLicense",
                column: "EventStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelHonor_EventStaffId",
                table: "TravelHonor",
                column: "EventStaffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ContractDetails");

            migrationBuilder.DropTable(
                name: "FileData");

            migrationBuilder.DropTable(
                name: "ServiceTypeProvided");

            migrationBuilder.DropTable(
                name: "StaffContractAffiliation");

            migrationBuilder.DropTable(
                name: "StaffLicense");

            migrationBuilder.DropTable(
                name: "TravelHonor");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "SubContractor");

            migrationBuilder.DropTable(
                name: "EventStaff");
        }
    }
}
