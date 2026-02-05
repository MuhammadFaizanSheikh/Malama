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
    [RoleAttributeAuthorizeFromConfig("AccountUser_View")]
    public class AccountUsersController : Controller
    {
        private const string CLASSNAME = nameof(AccountUsersController);

        private readonly IAccountUserService _accountUserService;
        private readonly ILogger<AccountUsersController> _logger;

        public AccountUsersController(
            IAccountUserService accountUserService,
            ILogger<AccountUsersController> logger)
        {
            _accountUserService = accountUserService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var methodName = nameof(Index);

            try
            {
                var users = await _accountUserService.GetAccountUsersAsync();
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
