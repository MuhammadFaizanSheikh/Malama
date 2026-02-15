using Malama.Models;

namespace Malama.Interfaces
{
    public interface IEventUsersService
    {
        Task<List<EventViewModel>> GetAllEventsAsync();
        Task<List<EventUserListDto>> GetEventUsersByEventIdAsync(long eventId);
    }

}
