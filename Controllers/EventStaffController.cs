using ExcelFilesCompiler.Models;
using ExcelToCsv.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExcelFilesCompiler.Controllers
{
    public class EventStaffController : Controller
    {
        public async Task<IActionResult> Index()
        {
            //List<SubContractorAndContractViewModel> subContractorList = new List<SubContractorAndContractViewModel>();

            //try
            //{
            //    subContractorList = await _subContractorService.GetAllSubContractors();
            //}
            //catch (Exception ex)
            //{
            //    TempData["ErrorMessage"] = "We encountered an issue while loading subcontractors. Please try again later.";
            //    // Optionally log the exception
            //}

            var viewModel = new EventStaffViewModel
            {
                EventStaff = null,
                SingleEventStaff = null
            };

            // Pass contracts data to the view
            return View(viewModel);
        }
    }
}
