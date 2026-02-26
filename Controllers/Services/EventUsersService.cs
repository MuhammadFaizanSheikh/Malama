using ExcelFilesCompiler;
using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using Malama.Interfaces;
using Malama.Models;
using Malama.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cms;
using static Malama.Utilities.RoleAttributeConfig;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace Malama.Controllers.Services
{
    public class EventUsersService : IEventUsersService
    {
        private const string CLASSNAME = nameof(EventUsersService);

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EventUsersService> _logger;
        private readonly IEventManagementService _eventManagementService;
        private readonly IEventStaffService _eventStaffService;

        public EventUsersService(
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            ILogger<EventUsersService> logger, IEventManagementService eventManagementService, IEventStaffService eventStaffService, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _eventManagementService = eventManagementService;
            _eventStaffService = eventStaffService;
        }

        public async Task<List<EventViewModel>> GetAllEventsAsync()
        {
            var methodName = nameof(GetAllEventsAsync);

            try
            {
                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Fetching event dropdown data",
                    CLASSNAME, methodName);
                var events = await _eventManagementService.GetAllEventID();

                return events.Select(e => new EventViewModel
                {
                    EventId = e.Id,
                    EventName = e.EventID
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "{ClassName}, {MethodName}, Error fetching events: {Message}",
                    CLASSNAME, methodName, ex.Message);

                return new List<EventViewModel>();
            }
        }

        public async Task<List<EventUserListDto>> GetEventUsersByEventIdAsync(long selectedEventId)
        {
            var methodName = nameof(GetEventUsersByEventIdAsync);

            try
            {
                _logger.LogInformation("{ClassName}, {MethodName}, Called. SelectedEventId: {EventId}",
                 CLASSNAME, methodName, selectedEventId);

                var listOfEventUsers = await _eventStaffService.GetAllEventStaffByEventId(selectedEventId);

                if (listOfEventUsers == null || !listOfEventUsers.Any())
                    return new List<EventUserListDto>();

                // 2️⃣ Extract distinct UserIds
                var userIds = listOfEventUsers
                    .Where(x => x.EventStaff != null)
                    .Select(x => x.EventStaff.UserId)
                    .Distinct()
                    .ToList();

                // 3️⃣ Extract primary and secondary role IDs
                var primaryRoleIds = listOfEventUsers
                    .SelectMany(x => x.EventWiseStaffRoleList ?? new List<EventWiseStaffRole>())
                    .Select(r => r.RoleId)
                    .Distinct()
                    .ToList();

                var secondaryRoleIds = listOfEventUsers
                    .SelectMany(x => x.EventWiseStaffSecondaryRoleList ?? new List<EventWiseStaffSecondaryRole>())
                    .Select(r => r.RoleId)
                    .Distinct()
                    .ToList();

                // 4️⃣ Fetch users
                var users = await _userManager.Users
                    .Where(u => userIds.Contains(u.Id))
                    .ToListAsync();

                // 5️⃣ Fetch roles
                var primaryRoles = await _roleManager.Roles
                    .Where(r => primaryRoleIds.Contains(r.Id))
                    .ToListAsync();

                var secondaryRoles = await _roleManager.Roles
                    .Where(r => secondaryRoleIds.Contains(r.Id))
                    .ToListAsync();

                // 6️⃣ Create dictionaries for quick lookup
                var userDictionary = users.ToDictionary(u => u.Id);
                var primaryRoleDictionary = primaryRoles.ToDictionary(r => r.Id, r => r.Name);
                var secondaryRoleDictionary = secondaryRoles.ToDictionary(r => r.Id, r => r.Name);

                // 7️⃣ Map to DTO
                var result = listOfEventUsers.Select(eventStaffDetail =>
                {
                    var eventStaff = eventStaffDetail.EventStaff;

                    // --- Attributes ---
                    var attributes = eventStaff?.StaffQualification?
                        .SelectMany(q => q.StaffAttributeDetails ?? new List<StaffAttributeDetails>())
                        .Select(a => a.Attribute)
                        .Distinct()
                        .ToList() ?? new List<string>();

                    var attributesCommaSeparated = attributes.Any()
                        ? string.Join(", ", attributes)
                        : string.Empty;

                    // --- Primary Roles ---
                    var primaryRoleNames = (eventStaffDetail.EventWiseStaffRoleList ?? new List<EventWiseStaffRole>())
                        .Where(r => primaryRoleDictionary.ContainsKey(r.RoleId))
                        .Select(r => primaryRoleDictionary[r.RoleId])
                        .Distinct()
                        .ToList();

                    var primaryRolesCommaSeparated = primaryRoleNames.Any()
                        ? string.Join(", ", primaryRoleNames)
                        : string.Empty;

                    // --- Secondary Roles ---
                    var secondaryRoleNames = (eventStaffDetail.EventWiseStaffSecondaryRoleList ?? new List<EventWiseStaffSecondaryRole>())
                        .Where(r => secondaryRoleDictionary.ContainsKey(r.RoleId))
                        .Select(r => secondaryRoleDictionary[r.RoleId])
                        .Distinct()
                        .ToList();

                    var secondaryRolesCommaSeparated = secondaryRoleNames.Any()
                        ? string.Join(", ", secondaryRoleNames)
                        : string.Empty;

                    var allRoles = primaryRoleNames.Concat(secondaryRoleNames).Distinct().ToList();
                    ;
                    var allowedPages = PagePermissionResolver.ResolveUserPages(
                        allRoles,    // pass actual role names
                        attributes           // pass attribute strings
                    );

                    return new EventUserListDto
                    {
                        UserId = eventStaff?.UserId,
                        UserName = eventStaff?.StaffFirstName + " " + eventStaff?.StaffLastName,
                        Email = eventStaff?.UserEmail,
                        PrimaryRole = primaryRolesCommaSeparated,
                        SecondaryRole = secondaryRolesCommaSeparated,
                        Attributes = attributesCommaSeparated,
                        AllowedPages = allowedPages,
                        PrimaryStation = eventStaffDetail.SelectedStation,
                        SecondaryStation = eventStaffDetail.SelectedSecondaryStation,
                        DetailSummaryAccess = eventStaffDetail.ProfileButtonAccess
                    };
                }).ToList();



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

                return new List<EventUserListDto>(); // never crash, return empty list
            }
        }

    }

}
