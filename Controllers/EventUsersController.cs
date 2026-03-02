using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using ExcelFilesCompiler.Utilities;
using Malama.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPOI.SS.Formula.Functions;
using Malama.Attributes;
using Malama.Interfaces;

namespace ExcelFilesCompiler.Controllers
{
    [RoleAttributeAuthorizeFromConfig("EventUser_View")]
    public class EventUsersController : Controller
    {
        private readonly IAccountUserService _accountUserService;
        private readonly IEventUsersService _eventService;
        private readonly ILogger<EventUsersController> _logger;

        private const string CLASSNAME = nameof(EventUsersController);

        public EventUsersController(
            IAccountUserService accountUserService,
            IEventUsersService eventService,
            ILogger<EventUsersController> logger)
        {
            _accountUserService = accountUserService;
            _eventService = eventService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(long? eventId)
        {
            var methodName = nameof(Index);

            try
            {
                var model = new EventUsersViewModel();

                // Always load events (for non Event Manager users)
                model.Events = await _eventService.GetAllEventsAsync();

                // 🔹 If user is Event Manager → get EventId from claim
                if (User.IsInRole("Event Manager"))
                {
                    var eventIdClaim = User.FindFirst("EventIdLong")?.Value;

                    if (!string.IsNullOrEmpty(eventIdClaim) &&
                        long.TryParse(eventIdClaim, out long claimEventId))
                    {
                        model.SelectedEventId = claimEventId;

                        model.Users = await _eventService
                            .GetEventUsersByEventIdAsync(claimEventId);
                    }
                }
                else
                {
                    // Normal flow
                    model.SelectedEventId = eventId;

                    if (eventId.HasValue)
                    {
                        model.Users = await _eventService
                            .GetEventUsersByEventIdAsync(eventId.Value);
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "{ClassName}, {MethodName}, Failed to load event users page: {Message}",
                    CLASSNAME, methodName, ex.Message);

                return View("Error");
            }
        }
    }


}
