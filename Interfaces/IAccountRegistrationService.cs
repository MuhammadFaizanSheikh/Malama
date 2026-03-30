using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IAccountRegistrationService
    {
        Task<ResponseDto> GetRegisterRolesAsync();
        Task<ResponseDto> RegisterUserAsync(RegisterViewModel model, bool IsEventUser = false, string addedBy = "");
        Task<ResponseDto> GetUsersAsync(string currentUserId);
        Task<ResponseDto> GetUserDetailsAsync(string userId);
        Task<ResponseDto> DeleteUserAsync(string userId);
        Task<ResponseDto> UpdateUserAsync(UserUpdateDto updatedUser);
        Task<ResponseDto> UpdateUserRolesAndEventsAsync(RegisterViewModel model);
    }

}
