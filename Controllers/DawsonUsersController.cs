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
    [RoleAttributeAuthorizeFromConfig("DawsonUser_View")]
    public class DawsonUsersController : Controller
    {
        private const string CLASSNAME = nameof(DawsonUsersController);

        private readonly IDawsonUserService _dawsonUserService;
        private readonly ILogger<DawsonUsersController> _logger;

        public DawsonUsersController(
            IDawsonUserService dawsonUserService,
            ILogger<DawsonUsersController> logger)
        {
            _dawsonUserService = dawsonUserService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var methodName = nameof(Index);

            try
            {
                var users = await _dawsonUserService.GetDawsonUsersAsync();
                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "{ClassName}, {MethodName}, Failed to load account users page: {Message}",
                    CLASSNAME, methodName, ex.Message);

                return View("Error");
            }
        }
    }

}
