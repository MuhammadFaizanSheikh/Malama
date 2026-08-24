using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IDentalQuestionnaireService
    {
        Task<DentalQuestionnaire?> GetByServiceMembersChildIdAsync(long serviceMembersChildId);
        Task SaveOrUpdateFromFormDataAsync(IDentalQuestionnaireFormData dto, string userName, string source);
        DentalQuestionnaire MapFormDataToEntity(IDentalQuestionnaireFormData dto, DentalQuestionnaire? existing = null);
    }
}
