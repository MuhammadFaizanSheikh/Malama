using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IDentalQuestionnaireService
    {
        Task<DentalQuestionnaire?> GetByServiceMembersChildIdAsync(long serviceMembersChildId);
        Task SaveOrUpdateFromSaveDtoAsync(DentalXRayStationSaveDto dto, string userName);
        DentalQuestionnaire MapSaveDtoToEntity(DentalXRayStationSaveDto dto, DentalQuestionnaire? existing = null);
    }
}
