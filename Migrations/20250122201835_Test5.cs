using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFilesCompiler.Migrations
{
    /// <inheritdoc />
    public partial class Test5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SuggestedHourlyFlow",
                table: "EventManagement",
                newName: "ShippingPocSuggestedHourlyFlow");

            migrationBuilder.RenameColumn(
                name: "ShippingZipCode",
                table: "EventManagement",
                newName: "ShippingAddressZipCode");

            migrationBuilder.RenameColumn(
                name: "ShippingState",
                table: "EventManagement",
                newName: "ShippingAddressState");

            migrationBuilder.RenameColumn(
                name: "ShippingInstruction",
                table: "EventManagement",
                newName: "ShippingPocSpecialGateInstructions");

            migrationBuilder.RenameColumn(
                name: "ShippingCity",
                table: "EventManagement",
                newName: "ShippingAddressCity");

            migrationBuilder.RenameColumn(
                name: "PickupTime",
                table: "EventManagement",
                newName: "ShippingPocPickupTime");

            migrationBuilder.RenameColumn(
                name: "PickupDate",
                table: "EventManagement",
                newName: "ShippingPocPickupDate");

            migrationBuilder.RenameColumn(
                name: "OpenAt",
                table: "EventManagement",
                newName: "ShippingPocOpenAt");

            migrationBuilder.RenameColumn(
                name: "DeliveryToDate",
                table: "EventManagement",
                newName: "ShippingPocEventSetupDate");

            migrationBuilder.RenameColumn(
                name: "DeliveryFromDate",
                table: "EventManagement",
                newName: "ShippingPocDeliveryToDate");

            migrationBuilder.RenameColumn(
                name: "CloseAt",
                table: "EventManagement",
                newName: "ShippingPocEventSetupTime");

            migrationBuilder.AddColumn<string>(
                name: "HIVDropOffStaffFirstName",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HIVDropOffStaffId",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HIVDropOffStaffLastName",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HIVSuppliesNeeded",
                table: "EventManagement",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HIVSupplyMilitaryContactPOCEmailPrimary",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HIVSupplyMilitaryContactPOCEmailSecondary",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HIVSupplyMilitaryContactPOCFirstName",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HIVSupplyMilitaryContactPOCLastName",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HIVSupplyMilitaryContactPOCPhonePrimary",
                table: "EventManagement",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HIVSupplyMilitaryContactPOCPhoneSecondary",
                table: "EventManagement",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HIVSupplyMilitaryContactPOCRank",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImmVaccineSupplyMilitaryContactPOCEmailPrimary",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImmVaccineSupplyMilitaryContactPOCEmailSecondary",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImmVaccineSupplyMilitaryContactPOCFirstName",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImmVaccineSupplyMilitaryContactPOCLastName",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImmVaccineSupplyMilitaryContactPOCPhonePrimary",
                table: "EventManagement",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImmVaccineSupplyMilitaryContactPOCPhoneSecondary",
                table: "EventManagement",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImmVaccineSupplyMilitaryContactPOCRank",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImmunizationVaccineNeeded",
                table: "EventManagement",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PharmacyAddressLine1",
                table: "EventManagement",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PharmacyAddressLine2",
                table: "EventManagement",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PharmacyCity",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PharmacyMilitaryArrangement",
                table: "EventManagement",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PharmacyName",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PharmacyPhoneNumber",
                table: "EventManagement",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PharmacyState",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PharmacyZipCode",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestPickupAddressLine1",
                table: "EventManagement",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestPickupAddressLine2",
                table: "EventManagement",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestPickupCity",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestPickupState",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestPickupZipCode",
                table: "EventManagement",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ShippingPocCloseAt",
                table: "EventManagement",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShippingPocDeliveryFromDate",
                table: "EventManagement",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingPocInstruction",
                table: "EventManagement",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingPocLocationSecured",
                table: "EventManagement",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingPocLockableRefrigerator",
                table: "EventManagement",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingPocParkingInstructions",
                table: "EventManagement",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingPocRefrigeratorAvailable",
                table: "EventManagement",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingPocTablesAndChairsAvailable",
                table: "EventManagement",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HIVDropOffStaffFirstName",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "HIVDropOffStaffId",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "HIVDropOffStaffLastName",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "HIVSuppliesNeeded",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "HIVSupplyMilitaryContactPOCEmailPrimary",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "HIVSupplyMilitaryContactPOCEmailSecondary",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "HIVSupplyMilitaryContactPOCFirstName",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "HIVSupplyMilitaryContactPOCLastName",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "HIVSupplyMilitaryContactPOCPhonePrimary",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "HIVSupplyMilitaryContactPOCPhoneSecondary",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "HIVSupplyMilitaryContactPOCRank",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "ImmVaccineSupplyMilitaryContactPOCEmailPrimary",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "ImmVaccineSupplyMilitaryContactPOCEmailSecondary",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "ImmVaccineSupplyMilitaryContactPOCFirstName",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "ImmVaccineSupplyMilitaryContactPOCLastName",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "ImmVaccineSupplyMilitaryContactPOCPhonePrimary",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "ImmVaccineSupplyMilitaryContactPOCPhoneSecondary",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "ImmVaccineSupplyMilitaryContactPOCRank",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "ImmunizationVaccineNeeded",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "PharmacyAddressLine1",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "PharmacyAddressLine2",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "PharmacyCity",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "PharmacyMilitaryArrangement",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "PharmacyName",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "PharmacyPhoneNumber",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "PharmacyState",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "PharmacyZipCode",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "QuestPickupAddressLine1",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "QuestPickupAddressLine2",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "QuestPickupCity",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "QuestPickupState",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "QuestPickupZipCode",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "ShippingPocCloseAt",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "ShippingPocDeliveryFromDate",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "ShippingPocInstruction",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "ShippingPocLocationSecured",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "ShippingPocLockableRefrigerator",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "ShippingPocParkingInstructions",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "ShippingPocRefrigeratorAvailable",
                table: "EventManagement");

            migrationBuilder.DropColumn(
                name: "ShippingPocTablesAndChairsAvailable",
                table: "EventManagement");

            migrationBuilder.RenameColumn(
                name: "ShippingPocSuggestedHourlyFlow",
                table: "EventManagement",
                newName: "SuggestedHourlyFlow");

            migrationBuilder.RenameColumn(
                name: "ShippingPocSpecialGateInstructions",
                table: "EventManagement",
                newName: "ShippingInstruction");

            migrationBuilder.RenameColumn(
                name: "ShippingPocPickupTime",
                table: "EventManagement",
                newName: "PickupTime");

            migrationBuilder.RenameColumn(
                name: "ShippingPocPickupDate",
                table: "EventManagement",
                newName: "PickupDate");

            migrationBuilder.RenameColumn(
                name: "ShippingPocOpenAt",
                table: "EventManagement",
                newName: "OpenAt");

            migrationBuilder.RenameColumn(
                name: "ShippingPocEventSetupTime",
                table: "EventManagement",
                newName: "CloseAt");

            migrationBuilder.RenameColumn(
                name: "ShippingPocEventSetupDate",
                table: "EventManagement",
                newName: "DeliveryToDate");

            migrationBuilder.RenameColumn(
                name: "ShippingPocDeliveryToDate",
                table: "EventManagement",
                newName: "DeliveryFromDate");

            migrationBuilder.RenameColumn(
                name: "ShippingAddressZipCode",
                table: "EventManagement",
                newName: "ShippingZipCode");

            migrationBuilder.RenameColumn(
                name: "ShippingAddressState",
                table: "EventManagement",
                newName: "ShippingState");

            migrationBuilder.RenameColumn(
                name: "ShippingAddressCity",
                table: "EventManagement",
                newName: "ShippingCity");
        }
    }
}
