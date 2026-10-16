using Malama.Models;



namespace ExcelFilesCompiler.Interfaces

{

    public interface ITreatmentConsentService

    {

        Task<TreatmentConsentIndexViewModel> GetCheckedInServiceMembersByEventIdAsync(long eventId, string? eventIdDisplay = null);



        Task<TreatmentConsentStationViewModel?> GetStationPageAsync(long serviceMembersChildId, long eventId);



        Task<TreatmentConsentFormSelectionDto> GetFormSelectionAsync(long serviceMembersChildId);

        /// <summary>
        /// Re-derives consent forms from current appointments/findings, invalidates signatures on
        /// material finding-scope changes, persists when changed, and returns files to delete.
        /// </summary>
        Task<TreatmentConsentAppointmentSyncResult> SyncFromAppointmentsAsync(
            long serviceMembersChildId,
            string? userId = null,
            bool saveChanges = true);

        Task<TreatmentConsentStationSaveResult> SaveStationAsync(
            TreatmentConsentStationSaveDto dto,
            string userId,
            long eventId);

    }

}

