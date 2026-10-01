using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IDentalTreatmentService
    {
        Task<DentalTreatment?> GetByServiceMembersChildIdAsync(long serviceMembersChildId);

        Task<DentalTreatmentCoordinator?> GetCoordinatorByServiceMembersChildIdAsync(long serviceMembersChildId);

        Task<long?> TryGetEventStaffIdForUserAsync(string userId);

        Task<bool> IsEligibleForDentalTreatmentAsync(
            long serviceMembersChildId,
            long eventId,
            long eventStaffId);

        Task<HashSet<long>> GetAssignedExamFindingIdsAsync(
            long serviceMembersChildId,
            long eventStaffId);

        Task<List<DentalTreatmentDentistAppointmentGroupDto>> GetDentistAppointmentFindingGroupsAsync(
            long serviceMembersChildId,
            long eventStaffId);

        Task SaveOrUpdateFromFormDataAsync(
            DentalTreatmentStationSaveDto dto,
            string userName,
            string userId,
            IReadOnlySet<long> assignedExamFindingIds);

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
