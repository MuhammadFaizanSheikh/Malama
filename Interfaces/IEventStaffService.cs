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
    }
}
