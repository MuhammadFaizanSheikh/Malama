using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IDentalQuestionnaireService
    {
        Task<DentalQuestionnaire?> GetByServiceMembersChildIdAsync(long serviceMembersChildId);
        Task SaveOrUpdateFromFormDataAsync(IDentalQuestionnaireFormData dto, string userName, string source, bool saveChanges = true);
        DentalQuestionnaire MapFormDataToEntity(IDentalQuestionnaireFormData dto, DentalQuestionnaire? existing = null);
    }
}
