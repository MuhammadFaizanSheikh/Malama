using ExcelFilesCompiler.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface ISubContractorService
    {
        Task<List<SubContractorInfoDto>> GetAllSubContractors();
    }
}
