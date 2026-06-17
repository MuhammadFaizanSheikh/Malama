using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class Test36 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlcoholicBeveragesFrequencyQuantity",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "ApplicableHealthConditionsJson",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "AreYouPregnant",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "BleederOrExcessiveBleeding",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "DrinkAlcoholicBeverages",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "HealthChangeLastTwoYears",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "HealthcareProviderCareLast2Years",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "HepatitisOrJaundice",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "MedicationFoodAllergy",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "MedicationFoodAllergyDetail",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "PregnancyApproval",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "SeriousIllnessOperationHospitalization",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "SeriousIllnessOperationHospitalizationDetail",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "ShortOfBreathOneFlightStairs",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "SickFromDentalTreatment",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "TakingMedications",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "TakingMedicationsDetail",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "TobaccoUseDetailsJson",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "UseTobaccoOrVape",
                table: "DentalXRayStation");

            migrationBuilder.CreateTable(
                name: "DentalQuestionaire",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceMembersChildId = table.Column<long>(type: "bigint", nullable: false),
                    HealthcareProviderCareLast2Years = table.Column<string>(type: "text", nullable: true),
                    SeriousIllnessOperationHospitalization = table.Column<string>(type: "text", nullable: true),
                    SeriousIllnessOperationHospitalizationDetail = table.Column<string>(type: "text", nullable: true),
                    MedicationFoodAllergy = table.Column<string>(type: "text", nullable: true),
                    MedicationFoodAllergyDetail = table.Column<string>(type: "text", nullable: true),
                    TakingMedications = table.Column<string>(type: "text", nullable: true),
                    TakingMedicationsDetail = table.Column<string>(type: "text", nullable: true),
                    HepatitisOrJaundice = table.Column<string>(type: "text", nullable: true),
                    HealthChangeLastTwoYears = table.Column<string>(type: "text", nullable: true),
                    UseTobaccoOrVape = table.Column<string>(type: "text", nullable: true),
                    TobaccoUseDetailsJson = table.Column<string>(type: "text", nullable: true),
                    DrinkAlcoholicBeverages = table.Column<string>(type: "text", nullable: true),
                    AlcoholicBeveragesFrequencyQuantity = table.Column<string>(type: "text", nullable: true),
                    SickFromDentalTreatment = table.Column<string>(type: "text", nullable: true),
                    BleederOrExcessiveBleeding = table.Column<string>(type: "text", nullable: true),
                    ShortOfBreathOneFlightStairs = table.Column<string>(type: "text", nullable: true),
                    AreYouPregnant = table.Column<string>(type: "text", nullable: true),
                    PregnancyApproval = table.Column<string>(type: "text", nullable: true),
                    ApplicableHealthConditionsJson = table.Column<string>(type: "text", nullable: true),
                    AddedBy = table.Column<string>(type: "text", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalQuestionaire", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalQuestionaire_ServiceMembersChild_ServiceMembersChildId",
                        column: x => x.ServiceMembersChildId,
                        principalTable: "ServiceMembersChild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DentalQuestionaire_ServiceMembersChildId",
                table: "DentalQuestionaire",
                column: "ServiceMembersChildId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DentalQuestionaire");

            migrationBuilder.AddColumn<string>(
                name: "AlcoholicBeveragesFrequencyQuantity",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicableHealthConditionsJson",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AreYouPregnant",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BleederOrExcessiveBleeding",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DrinkAlcoholicBeverages",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HealthChangeLastTwoYears",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HealthcareProviderCareLast2Years",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HepatitisOrJaundice",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MedicationFoodAllergy",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MedicationFoodAllergyDetail",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PregnancyApproval",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeriousIllnessOperationHospitalization",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeriousIllnessOperationHospitalizationDetail",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortOfBreathOneFlightStairs",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SickFromDentalTreatment",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TakingMedications",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TakingMedicationsDetail",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TobaccoUseDetailsJson",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UseTobaccoOrVape",
                table: "DentalXRayStation",
                type: "text",
                nullable: true);
        }
    }
}
