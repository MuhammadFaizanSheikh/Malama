using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IEventStaffService
    {
        Task<ResponseDto> AddContractAsync(EventStaff eventStaff, string submissionToken, string loggedinUserName);
        Task<List<EventStaff>> GetAllEventStaff();
        Task<CombinedEventStaffSubContractorAndContractDto> GetEventStaffById(long id);
        Task<ResponseDto> UpdateContract(EventStaff eventStaff, string loggedinUserName);
        Task<string> GetNextStaffId();
        Task<IEnumerable<EventStaff>> GetEventStaffForSearchingByStaffId(string staffId);
        Task<EventStaff> GetEventStaffWithoutIncludeById(long id);
        Task<EventStaff> GetEventStaffWithAttributesByUserId(string userId);
        Task<List<CombinedEventStaffRolesNameAndLicense>> GetAllEventStaffWithRolesAndLicenses();
        Task<bool> CheckSSNExistsAsync(string ssn);
        Task<List<EventStaffDetail>> GetAllEventStaffByEventId(long id);
    }
}
