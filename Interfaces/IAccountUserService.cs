using Malama.Models;

namespace Malama.Interfaces
{
    public interface IAccountUserService
    {
        Task<List<AccountUserListDto>> GetAccountUsersAsync();
    }

}
