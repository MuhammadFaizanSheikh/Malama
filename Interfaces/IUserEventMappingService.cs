using Malama.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IUserEventMappingService
    {
        Task<bool> IsUserAssignedToEventAsync(string userId, long eventId);
        Task<List<int>> GetEventsAgainstUserId(string userId);
    }

}
