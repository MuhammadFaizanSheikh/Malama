using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IDentalTreatmentService
    {
        Task<DentalTreatment?> GetByServiceMembersChildIdAsync(long serviceMembersChildId);

        Task<DentalTreatmentCoordinator?> GetCoordinatorByServiceMembersChildIdAsync(long serviceMembersChildId);

        Task SaveOrUpdateFromFormDataAsync(DentalTreatmentStationSaveDto dto, string userName, string userId);

        /// <summary>
        /// Upserts Treatment Coordinator details, documents metadata, and appointments on DentalTreatmentCoordinator.
        /// Does not create a DentalTreatment row.
        /// </summary>
        Task ApplyCoordinatorSectionAsync(
            long serviceMembersChildId,
            bool isTreatmentRequired,
            string? comments,
            string status,
            string userName,
            string userId,
            long eventStaffId,
            IReadOnlyList<TreatmentCoordinatorDocumentMetaDto> documents,
            IReadOnlyList<TreatmentCoordinatorAppointmentJsonDto> appointments,
            IReadOnlyDictionary<string, long> findingIdByClientKey,
            bool saveChanges = true);

        Task<List<TreatmentCoordinatorEventAppointmentDto>> GetEventAppointmentsExcludingAsync(
            long eventId,
            long excludeServiceMembersChildId);
    }
}
