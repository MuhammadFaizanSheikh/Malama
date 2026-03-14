using ExcelFilesCompiler;
using ExcelFilesCompiler.UnitOfWork;
using Malama.Interfaces;
using Malama.Models;
using Malama.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cms;
using static Malama.Utilities.RoleAttributeConfig;

namespace Malama.Controllers.Services
{
    public class DawsonUserService : IDawsonUserService
    {
        private const string CLASSNAME = nameof(DawsonUserService);

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DawsonUserService> _logger;

        public DawsonUserService(
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            ILogger<DawsonUserService> logger)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<DawsonUserListDto>> GetDawsonUsersAsync()
        {
            var methodName = nameof(GetDawsonUsersAsync);

            try
            {
                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Fetching account users",
                    CLASSNAME, methodName);

                var users = await _userManager.Users
                    .Where(u => !u.IsEventUser)
                    .ToListAsync();

                var result = new List<DawsonUserListDto>();

                foreach (var user in users)
                {
                    try
                    {
                        var roles = await _userManager.GetRolesAsync(user);

                        var dto = new DawsonUserListDto
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
                            Email = user.Email,
                            IsActive = user.IsActive,
                            IsEventManager = roles.Contains("Event Manager"),
                            Role = roles.Any() ? string.Join(", ", roles) : ""
                        };

                        // Safe permission resolving
                        try
                        {
                            dto.AllowedPages = PagePermissionResolver.ResolveUserPages(roles);
                        }
                        catch (Exception exPerm)
                        {
                            _logger.LogWarning(
                                "{ClassName}, {MethodName}, Failed to resolve pages for user {UserId}: {Message}",
                                CLASSNAME, methodName, user.Id, exPerm.Message);
                            dto.AllowedPages = new List<UserPagePermissionDto>(); // fallback empty list
                        }

                        // ONLY Event Managers have event mappings
                        if (roles.Contains("Event Manager"))
                        {
                            try
                            {
                                var eventIds = _unitOfWork.UserEventMapping
                                    .FindForSearching(x => x.UserId == user.Id)
                                    .Select(x => x.EventId)
                                    .ToList();

                                if (eventIds.Any())
                                {
                                    var eventIdsLong = eventIds.Select(id => (long)id).ToList();

                                    dto.Events = _unitOfWork.EventManagement
                                    .FindForSearching(e => eventIdsLong.Contains(e.Id))
                                    .Select(e => e.EventID + " (V" + e.EventVersion + ")")
                                    .ToList();
                                }
                            }
                            catch (Exception exEvents)
                            {
                                _logger.LogWarning(
                                    "{ClassName}, {MethodName}, Failed to fetch events for user {UserId}: {Message}",
                                    CLASSNAME, methodName, user.Id, exEvents.Message);
                                dto.Events = new List<string>(); // fallback empty
                            }
                        }

                        result.Add(dto);
                    }
                    catch (Exception exUser)
                    {
                        _logger.LogWarning(
                            "{ClassName}, {MethodName}, Failed to process user {UserId}: {Message}",
                            CLASSNAME, methodName, user.Id, exUser.Message);
                        // skip this user, continue processing others
                    }
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

                return new List<DawsonUserListDto>(); // never crash, return empty list
            }
        }

    }

}
