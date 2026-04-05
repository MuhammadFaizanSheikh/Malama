using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IEventStaffService
    {
        Task<ResponseDto> AddEventStaffAsync(EventStaff eventStaff, string submissionToken, string loggedinUserName);
        Task<List<EventStaff>> GetAllEventStaff();
        Task<CombinedEventStaffSubContractorAndContractDto> GetEventStaffById(long id);
        Task<ResponseDto> UpdateEventStaffAsync(EventStaff eventStaff, string loggedinUserName);
        Task<string> GetNextStaffId();
        IQueryable<EventStaff> GetEventStaffForSearchingByStaffId(string staffId);
        Task<EventStaff> GetEventStaffWithoutIncludeById(long id);
        Task<EventStaff> GetEventStaffWithAttributesByUserId(string userId);
        Task<List<CombinedEventStaffRolesNameAndLicense>> GetAllEventStaffWithRolesAndLicenses();
        Task<bool> CheckSSNExistsAsync(string ssn);
        Task<List<EventStaffDetail>> GetAllEventStaffByEventId(long id);
    }
}
