using ExcelFilesCompiler;
using ExcelFilesCompiler.UnitOfWork;
using Malama.Interfaces;
using Malama.Models;
using Malama.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Malama.Controllers.Services
{
    public class AccountUserService : IAccountUserService
    {
        private const string CLASSNAME = nameof(AccountUserService);

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountUserService> _logger;

        public AccountUserService(
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            ILogger<AccountUserService> logger)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<AccountUserListDto>> GetAccountUsersAsync()
        {
            var methodName = nameof(GetAccountUsersAsync);

            try
            {
                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Fetching account users",
                    CLASSNAME, methodName);

                var users = await _userManager.Users.Where(u => !u.IsEventUser).ToListAsync();
                var result = new List<AccountUserListDto>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    var dto = new AccountUserListDto
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        IsActive = user.IsActive,
                        IsEventManager = roles.Contains("Event Manager"),
                        Role = roles.Any() ? string.Join(", ", roles) : ""
                    };

                    //dto.AccessiblePages = DashboardAuthorizationHelper.GetAccessByRoles(roles);

                    // ✅ ONLY Event Managers have event mappings
                    if (roles.Contains("Event Manager"))
                    {
                        // 1️⃣ Get mapped event IDs
                        var eventIds = _unitOfWork.UserEventMapping
                            .FindForSearching(x => x.UserId == user.Id)
                            .Select(x => x.EventId)
                            .ToList();

                        if (eventIds.Any())
                        {
                            // 2️⃣ Convert int → long
                            var eventIdsLong = eventIds
                                .Select(id => (long)id)
                                .ToList();

                            // 3️⃣ Fetch event text
                            dto.Events = _unitOfWork.EventManagement
                                .FindForSearching(e => eventIdsLong.Contains(e.Id))
                                .Select(e => e.EventID) // or EventName / EventCode
                                .ToList();
                        }
                    }

                    


                    result.Add(dto);
                }


                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Successfully fetched account users. Count: {Count}",
                    CLASSNAME, methodName, result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "{ClassName}, {MethodName}, Failed to get account users: {Message}",
                    CLASSNAME, methodName, ex.Message);

                throw; // Let controller decide how to respond
            }
        }
    }

}
