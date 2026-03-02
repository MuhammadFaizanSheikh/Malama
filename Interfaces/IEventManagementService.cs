using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IEventManagementService
    {
        Task<ResponseDto> AddEventManagementAsync(EventManagement eventManagement, string submissionToken, string loggedinUserName);
        Task<List<EventManagementPreview>> GetAllEventManagements(long? eventIdFilter = null);
        Task<List<EventManagementPreview>> GetAllEventID();
        Task<string> GetNextEventManagementId();
        Task<CombinedEventManagementAndContractDetails> GetEventManagementById(long id);
        Task<ResponseDto> UpdateEventManagementAsync(EventManagement eventManagement, string loggedinUserName, string action);
        Task<EventManagement> GetEventManagementForEventSelectionById(long id);
        Task<EventManagement> GetEventManagementForEventSelectionByIdWithoutInclude(long id);
        Task<EventManagement> GetEventManagementByEventIdWithoutInclude(string eventId);
        Task<(DateTime StartDate, DateTime EndDate, int Version)> GetEventDetailsById(long eventId);
    }
}
