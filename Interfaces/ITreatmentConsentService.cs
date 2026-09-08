using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface ITreatmentConsentService
    {
        /// <summary>
        /// Returns checked-in service members for the event, mapped for the Treatment Consent index page.
        /// </summary>
        Task<TreatmentConsentIndexViewModel> GetCheckedInServiceMembersByEventIdAsync(long eventId, string? eventIdDisplay = null);

        /// <summary>
        /// Loads a service member, questionnaire, form selection, and dentist lists for the station UI.
        /// Returns null when the service member is not found.
        /// </summary>
        Task<TreatmentConsentStationViewModel?> GetStationPageAsync(long serviceMembersChildId);

        Task<TreatmentConsentFormSelectionDto> GetFormSelectionAsync(long serviceMembersChildId);

        Task<TreatmentConsentSaveFormSelectionResponse> SaveFormSelectionAsync(
            TreatmentConsentSaveFormSelectionRequest request,
            string userName);
    }
}
