using Malama.Models;



namespace ExcelFilesCompiler.Interfaces

{

    public interface ITreatmentConsentService

    {

        Task<TreatmentConsentIndexViewModel> GetCheckedInServiceMembersByEventIdAsync(long eventId, string? eventIdDisplay = null);



        Task<TreatmentConsentStationViewModel?> GetStationPageAsync(long serviceMembersChildId, long eventId);



        Task<TreatmentConsentFormSelectionDto> GetFormSelectionAsync(long serviceMembersChildId);



        Task<TreatmentConsentStationSaveResult> SaveStationAsync(
            TreatmentConsentStationSaveDto dto,
            string userId,
            long eventId);

    }

}

