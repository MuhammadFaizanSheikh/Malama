using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace ExcelFilesCompiler.Controllers
{
    //[Authorize(Roles = "DAWSON Admin - Event Staff,Project Manager & Program Manager,Super Admin")]
    public class VitalsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IContainerMonitoringService _service;
        private readonly ILogger<ImmunizationStationController> _logger;
         

        public VitalsController(ILogger<ImmunizationStationController> logger, IConfiguration configuration, UserManager<ApplicationUser> userManager, IContainerMonitoringService service)
        {
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager;
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}