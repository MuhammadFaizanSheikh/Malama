using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IDentalXRayStationService
    {
        Task<(DentalXRayStation DentalXRayStation, long EventId)> GetDentalXRayStationByIdWithEventIdAsync(long dentalXRayStationId);
        Task<DentalXRayStation?> GetByServiceMembersChildIdAsync(long serviceMembersChildId);
        Task AddAsync(DentalXRayStation model, string userName);
        Task UpdateAsync(DentalXRayStation model, string userName);
        string ComputeOverallStatus(DentalXRayStation model, ServiceMembersChild serviceMember, DentalQuestionnaire? questionnaire = null);
        DentalXRayStation MapSaveDtoToEntity(DentalXRayStationSaveDto dto, DentalXRayStation? existing = null);
    }
}
