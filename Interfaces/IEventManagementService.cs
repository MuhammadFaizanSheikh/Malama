using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IEventManagementService
    {
        Task<ResponseDto> AddEventManagementAsync(EventManagement eventManagement, string loggedinUserName);
        Task<List<EventManagementPreview>> GetAllEventManagements();
        Task<string> GetNextEventManagementId();
        Task<CombinedEventManagementAndContractDetails> GetEventManagementById(long id);
        Task<ResponseDto> UpdateEventManagementAsync(EventManagement eventManagement, string loggedinUserName);
        Task<EventManagement> GetEventManagementForEventSelectionById(long id);
    }
}
