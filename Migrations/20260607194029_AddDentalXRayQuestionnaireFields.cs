using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Malama.Migrations
{
    /// <inheritdoc />
    public partial class AddDentalXRayQuestionnaireFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlcoholicBeveragesFrequencyQuantity",
                table: "DentalXRayStation");

            migrationBuilder.DropColumn(
                name: "ApplicableHealthConditionsJson",
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
        }
    }
}
