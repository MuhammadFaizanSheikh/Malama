using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IDentalTreatmentService
    {
        Task<DentalTreatment?> GetByServiceMembersChildIdAsync(long serviceMembersChildId);
        Task SaveOrUpdateFromFormDataAsync(DentalTreatmentStationSaveDto dto, string userName, string userId);

        /// <summary>
        /// Upserts Treatment Coordinator details, documents metadata, and appointments on DentalTreatment.
        /// Requires an existing DentalExam for the service member.
        /// </summary>
        Task ApplyCoordinatorSectionAsync(
            long serviceMembersChildId,
            string? comments,
            string status,
            string userName,
            string userId,
            long eventStaffId,
            IReadOnlyList<TreatmentCoordinatorDocumentMetaDto> documents,
            IReadOnlyList<TreatmentCoordinatorAppointmentJsonDto> appointments,
            IReadOnlyDictionary<string, long> findingIdByClientKey,
            bool saveChanges = true);
    }
}
