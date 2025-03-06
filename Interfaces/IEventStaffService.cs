using ExcelFilesCompiler.Models;
using ExcelToCsv.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IEventStaffService
    {
        Task<ResponseDto> AddContractAsync(EventStaff evebtStaff, string loggedinUserName);
        Task<List<EventStaff>> GetAllEventStaff();
        Task<CombinedEventStaffSubContractorAndContractDto> GetEventStaffById(long id);
        Task<ResponseDto> UpdateContract(EventStaff eventStaff, string loggedinUserName);
        Task<string> GetNextStaffId();
        Task<IEnumerable<EventStaff>> GetEventStaffForSearchingByStaffId(string staffId);
        Task<EventStaff> GetEventStaffWithoutIncludeById(long id);
        Task<EventStaff> GetEventStaffByColumn(string userId);
        Task<List<CombinedEventStaffRolesNameAndLicense>> GetAllEventStaffWithRolesAndLicenses();
        Task<bool> CheckSSNExistsAsync(string ssn);
    }
}
