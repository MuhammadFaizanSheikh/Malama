using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Malama.Migrations
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
                    Id = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Types = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsEventUser = table.Column<bool>(type: "boolean", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContainerType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TemperatureFromRange = table.Column<int>(type: "integer", nullable: false),
                    TemperatureToRange = table.Column<int>(type: "integer", nullable: false),
                    TemperatureUnit = table.Column<string>(type: "text", nullable: false),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContractDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractID = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    ContractName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ContractAgency = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ContractServiceBranch = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ContractComponent = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ContractClient = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ContractType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DawsonRoleOnContract = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ContractStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ClientName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ContractStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ContractEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SiteId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    KoLastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    KoFirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    KOPhone = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    KOPhone2 = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    KOEmail = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    KONotes = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    CORLastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CORPrefix = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    CORFirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CORKORank = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CORPhone = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    CORPhone2 = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    COREmail = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CORNotes = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    DawsonProgramManagerLastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DawsonProgramManagerFirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DawsonDeputyProgramManagerLastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DawsonDeputyProgramManagerFirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DawsonProjectManagerLastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DawsonProjectManagerFirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventID = table.Column<string>(type: "text", nullable: false),
                    SubEventID = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    EventStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ContractId = table.Column<long>(type: "bigint", nullable: false),
                    EventAddress1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EventAddress2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    EventState = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EventCity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EventZipCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TotalRequestedServiceMembers = table.Column<int>(type: "integer", nullable: false),
                    EventStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EventEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Deploy = table.Column<string>(type: "text", nullable: false),
                    MOBDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RegardingSites = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    EventHelpLine = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    MainPocLastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MainPocFirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MainPocRank = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MainPocPhonePrimary = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    MainPocPhoneSecondary = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    MainPocEmailPrimary = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MainPocEmailSecondary = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SecondaryPocLastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SecondaryPocFirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SecondaryPocRank = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SecondaryPocPhonePrimary = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    SecondaryPocPhoneSecondary = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    SecondaryPocEmailPrimary = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SecondaryPocEmailSecondary = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AddAddtionalAlternatePoc = table.Column<bool>(type: "boolean", nullable: false),
                    AddtionalAlternatePocLastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AddtionalAlternatePocFirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AddtionalAlternatePocRank = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AddtionalAlternatePocPhonePrimary = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    AddtionalAlternatePocPhoneSecondary = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    AddtionalAlternatePocEmailPrimary = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AddtionalAlternatePocEmailSecondary = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AddtionalAlternatePocRole = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ShippingAddressLine1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ShippingAddressLine2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ShippingAddressState = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ShippingAddressCity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ShippingAddressZipCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ShippingPocLastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ShippingPocRank = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ShippingPocFirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ShippingPocPrimaryPhone = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    ShippingPocSecondaryPhone = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    ShippingPocPrimaryEmail = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ShippingPocSecondaryEmail = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ShippingPocOpenAt = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ShippingPocCloseAt = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ShippingPocInstruction = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    ShippingPocPickupDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ShippingPocPickupTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ShippingPocDeliveryFromDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ShippingPocDeliveryToDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ShippingPocSuggestedHourlyFlow = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ShippingPocSpecialGateInstructions = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    ShippingPocParkingInstructions = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    ShippingPocTablesAndChairsAvailable = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    ShippingPocLocationSecured = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    ShippingPocRefrigeratorAvailable = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    ShippingPocLockableRefrigerator = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    ShippingPocEventSetupDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ShippingPocEventSetupTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    PharmacyName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PharmacyAddressLine1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PharmacyAddressLine2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PharmacyState = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PharmacyCity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PharmacyZipCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PharmacyPhoneNumber = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    PharmacyMilitaryArrangement = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    HIVSuppliesNeeded = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    HIVSupplyMilitaryContactPOCLastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    HIVSupplyMilitaryContactPOCRank = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    HIVSupplyMilitaryContactPOCFirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    HIVSupplyMilitaryContactPOCPhonePrimary = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    HIVSupplyMilitaryContactPOCPhoneSecondary = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    HIVSupplyMilitaryContactPOCEmailPrimary = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    HIVSupplyMilitaryContactPOCEmailSecondary = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ImmunizationVaccineNeeded = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    ImmVaccineSupplyMilitaryContactPOCLastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ImmVaccineSupplyMilitaryContactPOCRank = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ImmVaccineSupplyMilitaryContactPOCFirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ImmVaccineSupplyMilitaryContactPOCPhonePrimary = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    ImmVaccineSupplyMilitaryContactPOCPhoneSecondary = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    ImmVaccineSupplyMilitaryContactPOCEmailPrimary = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ImmVaccineSupplyMilitaryContactPOCEmailSecondary = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    QuestPickupAddressLine1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    QuestPickupAddressLine2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    QuestPickupState = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    QuestPickupCity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    QuestPickupZipCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    HIVDropOffStaffId = table.Column<long>(type: "bigint", nullable: true),
                    StatusDescription = table.Column<string>(type: "text", nullable: true),
                    CompletedSections = table.Column<string>(type: "text", nullable: true),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventManagement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventStaff",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    UserEmail = table.Column<string>(type: "text", nullable: false),
                    UserPassword = table.Column<string>(type: "text", nullable: false),
                    StaffID = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    StaffStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StaffLastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StaffFirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StaffMiddleInitial = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StaffSSN = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    StaffDOB = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EventOnCallStaff = table.Column<string>(type: "text", nullable: false),
                    EventOnCallStaffEvent = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NPI = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    DAE = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    CredentialingProcessDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    HistoricalCredentialingDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DAWSONInternalCredentialingCompleteDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OnboardingTrainingComplete = table.Column<string>(type: "text", nullable: false),
                    OutstandingTrainings = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BackgroundCheckConcerns = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BLSCertDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BLSCertNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ACLSCertDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ACLSCertNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CACApplicationProcessStatus = table.Column<string>(type: "text", nullable: false),
                    StaffCAC = table.Column<string>(type: "text", nullable: false),
                    StaffDoDID = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CacExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    StaffCellNumber = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    StaffPhone2 = table.Column<string>(type: "text", nullable: true),
                    PrimaryAddress1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PrimaryAddress2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PrimaryCity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PrimaryState = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PrimaryZip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SecondaryAddress1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SecondaryAddress2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SecondaryCity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SecondaryState = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SecondaryZip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StaffInfoEnteredBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TravelHonorAir = table.Column<bool>(type: "boolean", nullable: false),
                    TravelHonorCar = table.Column<bool>(type: "boolean", nullable: false),
                    TravelHonorHotel = table.Column<bool>(type: "boolean", nullable: false),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SmId = table.Column<long>(type: "bigint", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    FullSsn = table.Column<string>(type: "text", nullable: true),
                    Last4 = table.Column<string>(type: "text", nullable: true),
                    DodId = table.Column<string>(type: "text", nullable: true),
                    Rank = table.Column<string>(type: "text", nullable: true),
                    Age = table.Column<int>(type: "integer", nullable: true),
                    Sex = table.Column<string>(type: "text", nullable: true),
                    Mos = table.Column<string>(type: "text", nullable: true),
                    Agr = table.Column<string>(type: "text", nullable: true),
                    Uic = table.Column<string>(type: "text", nullable: true),
                    Mrc = table.Column<string>(type: "text", nullable: true),
                    Dob = table.Column<string>(type: "text", nullable: true),
                    Over40 = table.Column<string>(type: "text", nullable: true),
                    DentalDue = table.Column<string>(type: "text", nullable: true),
                    DentalExam = table.Column<string>(type: "text", nullable: true),
                    DentalNeeded = table.Column<string>(type: "text", nullable: true),
                    PanoNeeded = table.Column<string>(type: "text", nullable: true),
                    BwxNeeded = table.Column<string>(type: "text", nullable: true),
                    Drc = table.Column<string>(type: "text", nullable: true),
                    PhaDate = table.Column<string>(type: "text", nullable: true),
                    PhaDue = table.Column<string>(type: "text", nullable: true),
                    Pha = table.Column<string>(type: "text", nullable: true),
                    Pulhes = table.Column<string>(type: "text", nullable: true),
                    VisionDate = table.Column<string>(type: "text", nullable: true),
                    Vision = table.Column<string>(type: "text", nullable: true),
                    NearVision = table.Column<string>(type: "text", nullable: true),
                    Vrc = table.Column<string>(type: "text", nullable: true),
                    Vision2pg = table.Column<string>(type: "text", nullable: true),
                    Vision1mi = table.Column<string>(type: "text", nullable: true),
                    HearingDate = table.Column<string>(type: "text", nullable: true),
                    Hearing = table.Column<string>(type: "text", nullable: true),
                    Hrc = table.Column<string>(type: "text", nullable: true),
                    HearingProfile = table.Column<string>(type: "text", nullable: true),
                    Quest = table.Column<string>(type: "text", nullable: true),
                    LabNeeded = table.Column<string>(type: "text", nullable: true),
                    Abo = table.Column<string>(type: "text", nullable: true),
                    AboNeeded = table.Column<string>(type: "text", nullable: true),
                    Dna = table.Column<string>(type: "text", nullable: true),
                    SickleDate = table.Column<string>(type: "text", nullable: true),
                    Sickle = table.Column<string>(type: "text", nullable: true),
                    G6pd = table.Column<string>(type: "text", nullable: true),
                    G6pdDate = table.Column<string>(type: "text", nullable: true),
                    G6pdStatus = table.Column<string>(type: "text", nullable: true),
                    HivNextTestDate = table.Column<string>(type: "text", nullable: true),
                    Hiv = table.Column<string>(type: "text", nullable: true),
                    LipidNeeded = table.Column<string>(type: "text", nullable: true),
                    LipidPanel = table.Column<string>(type: "text", nullable: true),
                    CholesterolHdlCholesterol = table.Column<string>(type: "text", nullable: true),
                    Framingham = table.Column<string>(type: "text", nullable: true),
                    Ekg = table.Column<string>(type: "text", nullable: true),
                    EkgNeeded = table.Column<string>(type: "text", nullable: true),
                    PregnancyTestNeeded = table.Column<string>(type: "text", nullable: true),
                    Imm = table.Column<string>(type: "text", nullable: true),
                    HepB = table.Column<string>(type: "text", nullable: true),
                    HepA = table.Column<string>(type: "text", nullable: true),
                    Flu = table.Column<string>(type: "text", nullable: true),
                    TetTdp = table.Column<string>(type: "text", nullable: true),
                    Mmr = table.Column<string>(type: "text", nullable: true),
                    Varicella = table.Column<string>(type: "text", nullable: true),
                    TaskForce = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Over44 = table.Column<string>(type: "text", nullable: true),
                    EventDate = table.Column<string>(type: "text", nullable: true),
                    EventEndDate = table.Column<string>(type: "text", nullable: true),
                    EventId = table.Column<string>(type: "text", nullable: true),
                    CheckIn = table.Column<string>(type: "text", nullable: true),
                    CheckInBy = table.Column<string>(type: "text", nullable: true),
                    CheckInTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CheckOut = table.Column<string>(type: "text", nullable: true),
                    CheckOutBy = table.Column<string>(type: "text", nullable: true),
                    CheckOutTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    WalkInServiceMember = table.Column<string>(type: "text", nullable: true),
                    VisionWin = table.Column<int>(type: "integer", nullable: true),
                    DentalWin = table.Column<int>(type: "integer", nullable: true),
                    PhaWin = table.Column<int>(type: "integer", nullable: true),
                    HivWin = table.Column<int>(type: "integer", nullable: true),
                    HearingWin = table.Column<int>(type: "integer", nullable: true),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Barcode = table.Column<string>(type: "text", nullable: true),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImmunizationVaccineInfo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EventDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ImmunizationType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Vaccine = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Manufacturer = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Dose = table.Column<decimal>(type: "numeric", nullable: false),
                    Unit = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    StartingDoses = table.Column<int>(type: "integer", nullable: false),
                    FinalDoses = table.Column<int>(type: "integer", nullable: true),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmunizationVaccineInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubContractor",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SmallBusinessType = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    SolicitationNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CompanyMainName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CompanyMainAddress1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyMainAddress2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CompanyMainCity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CompanyMainState = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CompanyMainZip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CompanyMainLastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CompanyMainFirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CompanyMainPhone = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    CompanyMainEmail = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FinanceLastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FinanceFirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FinanceAddress1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FinanceAddress2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FinanceCity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FinanceState = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FinanceZip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FinancePhone = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    FinanceEmail = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EventLastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EventFirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EventPhone = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    EventEmail = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TrainingLastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TrainingFirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TrainingPhone = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    TrainingEmail = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubContractor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserEventMapping",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEventMapping", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
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
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
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
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
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
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
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
                name: "Container",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<string>(type: "text", nullable: false),
                    ContainerName = table.Column<string>(type: "text", nullable: false),
                    ContainerTypeId = table.Column<long>(type: "bigint", nullable: false),
                    StartDateTimeUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    InitialTemperature = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrentStatus = table.Column<string>(type: "text", nullable: false),
                    NextExpectedReadingUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    MonitoringIntervalMinutes = table.Column<int>(type: "integer", nullable: false),
                    EscalationIntervalMinutes = table.Column<int>(type: "integer", nullable: false),
                    ConsecutiveNormalReadings = table.Column<int>(type: "integer", nullable: false),
                    FinalTemp = table.Column<bool>(type: "boolean", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Container", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Container_ContainerType_ContainerTypeId",
                        column: x => x.ContainerTypeId,
                        principalTable: "ContainerType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventManagementTaskforces",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventManagementId = table.Column<long>(type: "bigint", nullable: false),
                    Taskforce = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventManagementTaskforces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventManagementTaskforces_EventManagement_EventManagementId",
                        column: x => x.EventManagementId,
                        principalTable: "EventManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventServiceDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventManagementId = table.Column<long>(type: "bigint", nullable: false),
                    EventService = table.Column<string>(type: "text", nullable: true),
                    IsSelected = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
                    ClientRequestInitial = table.Column<int>(type: "integer", nullable: true),
                    InitialReportNumbers = table.Column<int>(type: "integer", nullable: true),
                    FinalPreEventConfirmedNumbers = table.Column<int>(type: "integer", nullable: true),
                    IsConfirmed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventServiceDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventServiceDetail_EventManagement_EventManagementId",
                        column: x => x.EventManagementId,
                        principalTable: "EventManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventStaffDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventManagementId = table.Column<long>(type: "bigint", nullable: false),
                    EventStaffId = table.Column<long>(type: "bigint", nullable: false),
                    PreEventAvailability = table.Column<bool>(type: "boolean", nullable: false),
                    ProfileButtonAccess = table.Column<bool>(type: "boolean", nullable: false),
                    SelectedStation = table.Column<string>(type: "text", nullable: true),
                    SelectedSecondaryStation = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStaffDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventStaffDetail_EventManagement_EventManagementId",
                        column: x => x.EventManagementId,
                        principalTable: "EventManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventStartEndTimeDayWise",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventManagementId = table.Column<long>(type: "bigint", nullable: false),
                    EventDay = table.Column<int>(type: "integer", nullable: false),
                    EventStartTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    EventEndTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ServiceMemberPercentPerDay = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStartEndTimeDayWise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventStartEndTimeDayWise_EventManagement_EventManagementId",
                        column: x => x.EventManagementId,
                        principalTable: "EventManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffContractAffiliation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                name: "StaffQualification",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventStaffId = table.Column<long>(type: "bigint", nullable: false),
                    QualificationId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    QualificationName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffQualification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffQualification_EventStaff_EventStaffId",
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventStaffId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Rewards = table.Column<decimal>(type: "numeric(6,0)", nullable: false)
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
                name: "LabStation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileDataId = table.Column<long>(type: "bigint", nullable: false),
                    AreYouFasting = table.Column<string>(type: "text", nullable: false),
                    AnyComplicationInBloodDrawn = table.Column<string>(type: "text", nullable: false),
                    AllergicToLatex = table.Column<string>(type: "text", nullable: false),
                    FeelAlright = table.Column<string>(type: "text", nullable: false),
                    G6pdNeeded = table.Column<string>(type: "text", nullable: true),
                    G6pdReason = table.Column<string>(type: "text", nullable: true),
                    AboNeeded = table.Column<string>(type: "text", nullable: true),
                    AboReason = table.Column<string>(type: "text", nullable: true),
                    AboGrouping = table.Column<string>(type: "text", nullable: true),
                    AboRhFactor = table.Column<string>(type: "text", nullable: true),
                    LipidPanelNeeded = table.Column<string>(type: "text", nullable: true),
                    LipidPanelReason = table.Column<string>(type: "text", nullable: true),
                    LipidPanelRapidTesting = table.Column<bool>(type: "boolean", nullable: false),
                    TotalCholesterol = table.Column<string>(type: "text", nullable: true),
                    HdlCholesterol = table.Column<string>(type: "text", nullable: true),
                    Triglycerides = table.Column<string>(type: "text", nullable: true),
                    Glucose = table.Column<string>(type: "text", nullable: true),
                    LdlCholesterol = table.Column<string>(type: "text", nullable: true),
                    TotalCholesterolHdlRatio = table.Column<string>(type: "text", nullable: true),
                    LdlHdlLipoprotiens = table.Column<string>(type: "text", nullable: true),
                    NonHdlCholesterol = table.Column<string>(type: "text", nullable: true),
                    HivNeeded = table.Column<string>(type: "text", nullable: true),
                    HivReason = table.Column<string>(type: "text", nullable: true),
                    HivBarcodeCarebill = table.Column<string>(type: "text", nullable: true),
                    PregnancyTestNeeded = table.Column<string>(type: "text", nullable: true),
                    PregnancyTestResult = table.Column<string>(type: "text", nullable: true),
                    PregnancyTestReason = table.Column<string>(type: "text", nullable: true),
                    FedExTrackingNo = table.Column<string>(type: "text", nullable: true),
                    G6pdGivenDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AboGivenDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LipidPanelGivenDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HivGivenDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PregnancyTestGivenDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabStation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabStation_FileData_FileDataId",
                        column: x => x.FileDataId,
                        principalTable: "FileData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypeProvided",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubContractorId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceTypeProvidedName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ContainerNotification",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContainerId = table.Column<long>(type: "bigint", nullable: false),
                    DueAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsAcknowledged = table.Column<bool>(type: "boolean", nullable: false),
                    AcknowledgedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContainerNotification_Container_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Container",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContainerTemperatureReading",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContainerId = table.Column<long>(type: "bigint", nullable: false),
                    ReadingTimeUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Temperature = table.Column<decimal>(type: "numeric", nullable: false),
                    IsOutOfRange = table.Column<bool>(type: "boolean", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    AttemptNumber = table.Column<int>(type: "integer", nullable: false),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerTemperatureReading", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContainerTemperatureReading_Container_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Container",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImmunizationVaccineLotEntry",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LotNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Expiration = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ContainerId = table.Column<long>(type: "bigint", nullable: false),
                    ImmunizationVaccineInfoId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmunizationVaccineLotEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImmunizationVaccineLotEntry_Container_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Container",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImmunizationVaccineLotEntry_ImmunizationVaccineInfo_Immuniz~",
                        column: x => x.ImmunizationVaccineInfoId,
                        principalTable: "ImmunizationVaccineInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventManagementStaffAvailability",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventStaffDetailId = table.Column<long>(type: "bigint", nullable: false),
                    AvailabilityDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventManagementStaffAvailability", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventManagementStaffAvailability_EventStaffDetail_EventStaf~",
                        column: x => x.EventStaffDetailId,
                        principalTable: "EventStaffDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventWiseStaffRole",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventStaffDetailId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventWiseStaffRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventWiseStaffRole_EventStaffDetail_EventStaffDetailId",
                        column: x => x.EventStaffDetailId,
                        principalTable: "EventStaffDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventWiseStaffSecondaryRole",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventStaffDetailId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventWiseStaffSecondaryRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventWiseStaffSecondaryRole_EventStaffDetail_EventStaffDeta~",
                        column: x => x.EventStaffDetailId,
                        principalTable: "EventStaffDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffAttributeDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StaffQualificationId = table.Column<long>(type: "bigint", nullable: false),
                    Attribute = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffAttributeDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffAttributeDetails_StaffQualification_StaffQualification~",
                        column: x => x.StaffQualificationId,
                        principalTable: "StaffQualification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffLicenseDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StaffQualificationId = table.Column<long>(type: "bigint", nullable: false),
                    LicenseNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LicenseState = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LicenseType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LicenseActiveDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LicenseExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffLicenseDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffLicenseDetails_StaffQualification_StaffQualificationId",
                        column: x => x.StaffQualificationId,
                        principalTable: "StaffQualification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImmunizationStation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileDataId = table.Column<long>(type: "bigint", nullable: false),
                    IsSickToday = table.Column<string>(type: "text", nullable: false),
                    IsSickTodayReason = table.Column<string>(type: "text", nullable: true),
                    HasAllergiesToMedicationFoodVaccineOrLatex = table.Column<string>(type: "text", nullable: false),
                    HasAllergiesReason = table.Column<string>(type: "text", nullable: true),
                    HadSeriousReactionAfterVaccination = table.Column<string>(type: "text", nullable: false),
                    SeriousReactionReason = table.Column<string>(type: "text", nullable: true),
                    HasLongTermHealthProblem = table.Column<string>(type: "text", nullable: false),
                    LongTermHealthProblemReason = table.Column<string>(type: "text", nullable: true),
                    HasCancerOrImmuneSystemProblem = table.Column<string>(type: "text", nullable: false),
                    CancerOrImmuneSystemReason = table.Column<string>(type: "text", nullable: true),
                    TookImmuneSuppressingMedicationRecently = table.Column<string>(type: "text", nullable: false),
                    ImmuneSuppressionReason = table.Column<string>(type: "text", nullable: true),
                    HadSeizureOrNervousSystemProblem = table.Column<string>(type: "text", nullable: false),
                    SeizureReason = table.Column<string>(type: "text", nullable: true),
                    HadBloodTransfusionOrAntiviralInPastYear = table.Column<string>(type: "text", nullable: false),
                    BloodTransfusionReason = table.Column<string>(type: "text", nullable: true),
                    IsPregnantOrCouldBePregnant = table.Column<string>(type: "text", nullable: false),
                    PregnancyCheckboxSelected = table.Column<bool>(type: "boolean", nullable: false),
                    ReceivedVaccineInPast4Weeks = table.Column<string>(type: "text", nullable: false),
                    ReceivedVaccineReason = table.Column<string>(type: "text", nullable: true),
                    HepBNeeded = table.Column<string>(type: "text", nullable: true),
                    HepBReason = table.Column<string>(type: "text", nullable: true),
                    HepBReasonExcusedComments = table.Column<string>(type: "text", nullable: true),
                    HepBVaccineInfoId = table.Column<long>(type: "bigint", nullable: true),
                    HepBVaccineLotEntryId = table.Column<long>(type: "bigint", nullable: true),
                    HepBExpirationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HepBType = table.Column<string>(type: "text", nullable: true),
                    HepBBodyPart = table.Column<string>(type: "text", nullable: true),
                    HepBBodyPartOther = table.Column<string>(type: "text", nullable: true),
                    HepBSite = table.Column<string>(type: "text", nullable: true),
                    HepBStaffName = table.Column<string>(type: "text", nullable: true),
                    HepBGivenDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FluNeeded = table.Column<string>(type: "text", nullable: true),
                    FluReason = table.Column<string>(type: "text", nullable: true),
                    FluReasonExcusedComments = table.Column<string>(type: "text", nullable: true),
                    FluVaccineInfoId = table.Column<long>(type: "bigint", nullable: true),
                    FluVaccineLotEntryId = table.Column<long>(type: "bigint", nullable: true),
                    FluExpirationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FluType = table.Column<string>(type: "text", nullable: true),
                    FluBodyPart = table.Column<string>(type: "text", nullable: true),
                    FluBodyPartOther = table.Column<string>(type: "text", nullable: true),
                    FluSite = table.Column<string>(type: "text", nullable: true),
                    FluStaffName = table.Column<string>(type: "text", nullable: true),
                    FluGivenDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    MMRNeeded = table.Column<string>(type: "text", nullable: true),
                    MMRReason = table.Column<string>(type: "text", nullable: true),
                    MMRReasonExcusedComments = table.Column<string>(type: "text", nullable: true),
                    MMRVaccineInfoId = table.Column<long>(type: "bigint", nullable: true),
                    MMRVaccineLotEntryId = table.Column<long>(type: "bigint", nullable: true),
                    MMRExpirationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    MMRType = table.Column<string>(type: "text", nullable: true),
                    MMRBodyPart = table.Column<string>(type: "text", nullable: true),
                    MMRBodyPartOther = table.Column<string>(type: "text", nullable: true),
                    MMRSite = table.Column<string>(type: "text", nullable: true),
                    MMRStaffName = table.Column<string>(type: "text", nullable: true),
                    MMRGivenDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HepANeeded = table.Column<string>(type: "text", nullable: true),
                    HepAReason = table.Column<string>(type: "text", nullable: true),
                    HepAReasonExcusedComments = table.Column<string>(type: "text", nullable: true),
                    HepAVaccineInfoId = table.Column<long>(type: "bigint", nullable: true),
                    HepAVaccineLotEntryId = table.Column<long>(type: "bigint", nullable: true),
                    HepAExpirationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HepAType = table.Column<string>(type: "text", nullable: true),
                    HepABodyPart = table.Column<string>(type: "text", nullable: true),
                    HepABodyPartOther = table.Column<string>(type: "text", nullable: true),
                    HepASite = table.Column<string>(type: "text", nullable: true),
                    HepAStaffName = table.Column<string>(type: "text", nullable: true),
                    HepAGivenDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TetTdpNeeded = table.Column<string>(type: "text", nullable: true),
                    TetTdpReason = table.Column<string>(type: "text", nullable: true),
                    TetTdpReasonExcusedComments = table.Column<string>(type: "text", nullable: true),
                    TetTdpVaccineInfoId = table.Column<long>(type: "bigint", nullable: true),
                    TetTdpVaccineLotEntryId = table.Column<long>(type: "bigint", nullable: true),
                    TetTdpExpirationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TetTdpType = table.Column<string>(type: "text", nullable: true),
                    TetTdpBodyPart = table.Column<string>(type: "text", nullable: true),
                    TetTdpBodyPartOther = table.Column<string>(type: "text", nullable: true),
                    TetTdpSite = table.Column<string>(type: "text", nullable: true),
                    TetTdpStaffName = table.Column<string>(type: "text", nullable: true),
                    TetTdpGivenDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    VaricellaNeeded = table.Column<string>(type: "text", nullable: true),
                    VaricellaReason = table.Column<string>(type: "text", nullable: true),
                    VaricellaReasonExcusedComments = table.Column<string>(type: "text", nullable: true),
                    VaricellaVaccineInfoId = table.Column<long>(type: "bigint", nullable: true),
                    VaricellaVaccineLotEntryId = table.Column<long>(type: "bigint", nullable: true),
                    VaricellaExpirationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    VaricellaType = table.Column<string>(type: "text", nullable: true),
                    VaricellaBodyPart = table.Column<string>(type: "text", nullable: true),
                    VaricellaBodyPartOther = table.Column<string>(type: "text", nullable: true),
                    VaricellaSite = table.Column<string>(type: "text", nullable: true),
                    VaricellaStaffName = table.Column<string>(type: "text", nullable: true),
                    VaricellaGivenDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmunizationStation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImmunizationStation_FileData_FileDataId",
                        column: x => x.FileDataId,
                        principalTable: "FileData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImmunizationStation_ImmunizationVaccineInfo_FluVaccineInfoId",
                        column: x => x.FluVaccineInfoId,
                        principalTable: "ImmunizationVaccineInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmunizationStation_ImmunizationVaccineInfo_HepAVaccineInfo~",
                        column: x => x.HepAVaccineInfoId,
                        principalTable: "ImmunizationVaccineInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmunizationStation_ImmunizationVaccineInfo_HepBVaccineInfo~",
                        column: x => x.HepBVaccineInfoId,
                        principalTable: "ImmunizationVaccineInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmunizationStation_ImmunizationVaccineInfo_MMRVaccineInfoId",
                        column: x => x.MMRVaccineInfoId,
                        principalTable: "ImmunizationVaccineInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmunizationStation_ImmunizationVaccineInfo_TetTdpVaccineIn~",
                        column: x => x.TetTdpVaccineInfoId,
                        principalTable: "ImmunizationVaccineInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmunizationStation_ImmunizationVaccineInfo_VaricellaVaccin~",
                        column: x => x.VaricellaVaccineInfoId,
                        principalTable: "ImmunizationVaccineInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmunizationStation_ImmunizationVaccineLotEntry_FluVaccineL~",
                        column: x => x.FluVaccineLotEntryId,
                        principalTable: "ImmunizationVaccineLotEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmunizationStation_ImmunizationVaccineLotEntry_HepAVaccine~",
                        column: x => x.HepAVaccineLotEntryId,
                        principalTable: "ImmunizationVaccineLotEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmunizationStation_ImmunizationVaccineLotEntry_HepBVaccine~",
                        column: x => x.HepBVaccineLotEntryId,
                        principalTable: "ImmunizationVaccineLotEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmunizationStation_ImmunizationVaccineLotEntry_MMRVaccineL~",
                        column: x => x.MMRVaccineLotEntryId,
                        principalTable: "ImmunizationVaccineLotEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmunizationStation_ImmunizationVaccineLotEntry_TetTdpVacci~",
                        column: x => x.TetTdpVaccineLotEntryId,
                        principalTable: "ImmunizationVaccineLotEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImmunizationStation_ImmunizationVaccineLotEntry_VaricellaVa~",
                        column: x => x.VaricellaVaccineLotEntryId,
                        principalTable: "ImmunizationVaccineLotEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

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
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Container_ContainerTypeId",
                table: "Container",
                column: "ContainerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerNotification_ContainerId",
                table: "ContainerNotification",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerTemperatureReading_ContainerId",
                table: "ContainerTemperatureReading",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_EventManagementStaffAvailability_EventStaffDetailId",
                table: "EventManagementStaffAvailability",
                column: "EventStaffDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_EventManagementTaskforces_EventManagementId",
                table: "EventManagementTaskforces",
                column: "EventManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_EventServiceDetail_EventManagementId",
                table: "EventServiceDetail",
                column: "EventManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_EventStaffDetail_EventManagementId",
                table: "EventStaffDetail",
                column: "EventManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_EventStartEndTimeDayWise_EventManagementId",
                table: "EventStartEndTimeDayWise",
                column: "EventManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_EventWiseStaffRole_EventStaffDetailId",
                table: "EventWiseStaffRole",
                column: "EventStaffDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_EventWiseStaffSecondaryRole_EventStaffDetailId",
                table: "EventWiseStaffSecondaryRole",
                column: "EventStaffDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_FileData_EventId_isDeleted",
                table: "FileData",
                columns: new[] { "EventId", "isDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_FileDataId",
                table: "ImmunizationStation",
                column: "FileDataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_FluVaccineInfoId",
                table: "ImmunizationStation",
                column: "FluVaccineInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_FluVaccineLotEntryId",
                table: "ImmunizationStation",
                column: "FluVaccineLotEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_HepAVaccineInfoId",
                table: "ImmunizationStation",
                column: "HepAVaccineInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_HepAVaccineLotEntryId",
                table: "ImmunizationStation",
                column: "HepAVaccineLotEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_HepBVaccineInfoId",
                table: "ImmunizationStation",
                column: "HepBVaccineInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_HepBVaccineLotEntryId",
                table: "ImmunizationStation",
                column: "HepBVaccineLotEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_MMRVaccineInfoId",
                table: "ImmunizationStation",
                column: "MMRVaccineInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_MMRVaccineLotEntryId",
                table: "ImmunizationStation",
                column: "MMRVaccineLotEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_TetTdpVaccineInfoId",
                table: "ImmunizationStation",
                column: "TetTdpVaccineInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_TetTdpVaccineLotEntryId",
                table: "ImmunizationStation",
                column: "TetTdpVaccineLotEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_VaricellaVaccineInfoId",
                table: "ImmunizationStation",
                column: "VaricellaVaccineInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationStation_VaricellaVaccineLotEntryId",
                table: "ImmunizationStation",
                column: "VaricellaVaccineLotEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationVaccineLotEntry_ContainerId",
                table: "ImmunizationVaccineLotEntry",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_ImmunizationVaccineLotEntry_ImmunizationVaccineInfoId",
                table: "ImmunizationVaccineLotEntry",
                column: "ImmunizationVaccineInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_LabStation_FileDataId",
                table: "LabStation",
                column: "FileDataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTypeProvided_SubContractorId",
                table: "ServiceTypeProvided",
                column: "SubContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffAttributeDetails_StaffQualificationId",
                table: "StaffAttributeDetails",
                column: "StaffQualificationId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffContractAffiliation_EventStaffId",
                table: "StaffContractAffiliation",
                column: "EventStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffLicenseDetails_StaffQualificationId",
                table: "StaffLicenseDetails",
                column: "StaffQualificationId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffQualification_EventStaffId",
                table: "StaffQualification",
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
                name: "ContainerNotification");

            migrationBuilder.DropTable(
                name: "ContainerTemperatureReading");

            migrationBuilder.DropTable(
                name: "ContractDetails");

            migrationBuilder.DropTable(
                name: "EventManagementStaffAvailability");

            migrationBuilder.DropTable(
                name: "EventManagementTaskforces");

            migrationBuilder.DropTable(
                name: "EventServiceDetail");

            migrationBuilder.DropTable(
                name: "EventStartEndTimeDayWise");

            migrationBuilder.DropTable(
                name: "EventWiseStaffRole");

            migrationBuilder.DropTable(
                name: "EventWiseStaffSecondaryRole");

            migrationBuilder.DropTable(
                name: "ImmunizationStation");

            migrationBuilder.DropTable(
                name: "LabStation");

            migrationBuilder.DropTable(
                name: "ServiceTypeProvided");

            migrationBuilder.DropTable(
                name: "StaffAttributeDetails");

            migrationBuilder.DropTable(
                name: "StaffContractAffiliation");

            migrationBuilder.DropTable(
                name: "StaffLicenseDetails");

            migrationBuilder.DropTable(
                name: "TravelHonor");

            migrationBuilder.DropTable(
                name: "UserEventMapping");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "EventStaffDetail");

            migrationBuilder.DropTable(
                name: "ImmunizationVaccineLotEntry");

            migrationBuilder.DropTable(
                name: "FileData");

            migrationBuilder.DropTable(
                name: "SubContractor");

            migrationBuilder.DropTable(
                name: "StaffQualification");

            migrationBuilder.DropTable(
                name: "EventManagement");

            migrationBuilder.DropTable(
                name: "Container");

            migrationBuilder.DropTable(
                name: "ImmunizationVaccineInfo");

            migrationBuilder.DropTable(
                name: "EventStaff");

            migrationBuilder.DropTable(
                name: "ContainerType");
        }
    }
}
