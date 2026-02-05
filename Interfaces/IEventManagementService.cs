using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IEventManagementService
    {
        Task<ResponseDto> AddEventManagementAsync(EventManagement eventManagement, string loggedinUserName);
        Task<List<EventManagementPreview>> GetAllEventManagements();
        Task<List<EventManagementPreview>> GetAllEventID();
        Task<string> GetNextEventManagementId();
        Task<CombinedEventManagementAndContractDetails> GetEventManagementById(long id);
        Task<ResponseDto> UpdateEventManagementAsync(EventManagement eventManagement, string loggedinUserName);
        Task<EventManagement> GetEventManagementForEventSelectionById(long id);
        Task<EventManagement> GetEventManagementForEventSelectionByIdWithoutInclude(long id);
        Task<EventManagement> GetEventManagementByEventIdWithoutInclude(string eventId);
        Task<EventManagement> GetEventStartAndEndDateById(long eventId);
    }
}
