using ExcelFilesCompiler.Models;
using ExcelToCsv.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IEventStaffService
    {
        Task<ResponseDto> AddContractAsync(EventStaffDto evebtStaff, string loggedinUserName);
        Task<List<EventStaffDto>> GetAllEventStaff();
        Task<EventStaffDto> GetEventStaffById(long id);
        Task<ResponseDto> UpdateContract(EventStaffDto eventStaff, string loggedinUserName);
    }
}
