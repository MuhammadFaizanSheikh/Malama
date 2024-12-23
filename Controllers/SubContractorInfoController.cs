using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExcelFilesCompiler.Controllers
{
    public class SubContractorInfoController : Controller
    {
        private readonly ISubContractorService _subContractorService;
        private readonly UserManager<ApplicationUser> _userManager;


        public SubContractorInfoController(ISubContractorService subContractorService, UserManager<ApplicationUser> userManager)
        {
            _subContractorService = subContractorService;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            //var responseDto = new ResponseDto();
            //List<SubContractorInfoDto> subContractorList = new List<SubContractorInfoDto>();

            //try
            //{
            //    subContractorList = await _subContractorService.GetAllSubContractors();
            //}
            //catch (Exception ex)
            //{
            //    TempData["ErrorMessage"] = "We encountered an issue while loading sub contractors. Please try again later.";
            //}

            //var viewModel = new SubContractorViewModel
            //{
            //    SubContractor = subContractorList,
            //    SingleSubContractor = null
            //};
            //// Pass contracts data to the view
            //return View(viewModel);

            return View();
        }
    }
}
