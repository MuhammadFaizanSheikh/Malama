using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IEventManagementService
    {
        Task<ResponseDto> AddEventManagementAsync(EventManagement eventManagement, string submissionToken, string loggedinUserName);
        Task<List<EventManagementPreview>> GetAllEventManagements(long? eventIdFilter = null);
        Task<List<PostEventManagementPreview>> GetAllForPostEventManagements();
        Task<(PostEventManagementDto Data, string EventID)> GetForPostEventManagement(long eventManagementId);
        Task<List<EventManagementPreview>> GetAllEventID(bool includeVersion = true);
        Task<string> GetNextEventIdNumber();
        Task<CombinedEventManagementAndContractDetails> GetEventManagementById(long id);
        Task<ResponseDto> UpdateEventManagementAsync(EventManagement eventManagement, string loggedinUserName, string action);
        Task<EventManagement> GetEventManagementForEventSelectionById(long id);
        Task<EventManagement> GetEventManagementForEventSelectionByIdWithoutInclude(long id);
        Task<EventManagement> GetEventManagementByEventIdWithoutInclude(string eventId);
        Task<(DateTime StartDate, DateTime EndDate, int Version)> GetEventDetailsById(long eventId);
        Task<bool> HasServiceMembersAsync(string eventId);
        Task<List<FileDataDto>> GetServiceMembersByEventAsync(long eventId);
    }
}
