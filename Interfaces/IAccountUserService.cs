using Malama.Models;

namespace Malama.Interfaces
{
    public interface IDawsonUserService
    {
        Task<List<DawsonUserListDto>> GetDawsonUsersAsync();
    }

}
