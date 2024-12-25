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
            var responseDto = new ResponseDto();
            List<SubContractorInfoDto> subContractorList = new List<SubContractorInfoDto>();

            try
            {
                subContractorList = await _subContractorService.GetAllSubContractors();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "We encountered an issue while loading sub contractors. Please try again later.";
            }

            var viewModel = new SubContractorViewModel
            {
                SubContractor = subContractorList,
                SingleSubContractor = null
            };
            // Pass contracts data to the view
            return View(viewModel);

            //return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubContracttor(SubContractorViewModel contractDto, string action)
        {
            try
            {
                ResponseDto res = new ResponseDto();

                if (contractDto.SingleSubContractor.Id == 0) // Adding a new record
                {
                    ModelState.Remove("SingleSubContractor.Id");
                }

                if (!ModelState.IsValid)
                {
                    // Log the validation errors for debugging
                    foreach (var error in ModelState.Values)
                    {
                        foreach (var err in error.Errors)
                        {
                            Console.WriteLine($"Error: {err.ErrorMessage}");
                        }
                    }

                    return View("Index", contractDto);
                }

                var user = _userManager.GetUserAsync(User).Result;

                if (user != null)
                {
                    if (action == "Add")
                    {
                        res = await _subContractorService.AddContractAsync(contractDto.SingleSubContractor, user.UserName);
                    }
                    else if (action == "Update")
                    {
                        res = await _subContractorService.UpdateContract(contractDto.SingleSubContractor, user.UserName);
                    }
                }
                else
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Error";
                    TempData["ResponseMessage"] = "Please login and try again";
                    return RedirectToAction("Index");
                }

                TempData["ResponseStatus"] = res.Success ? "success" : "error"; // SweetAlert2 icon
                TempData["ResponseTitle"] = res.Success ? "Success" : "Error";
                TempData["ResponseMessage"] = res.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = "An unexpected error occurred.";
                return RedirectToAction("Index", contractDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetNextCompanyCode()
        {
            try
            {
                // Call the service method to get the next CompanyCode
                string nextCompanyCode = await _subContractorService.GetLastCompanyCode();

                // Return the result as JSON response
                return Json(new { success = true, nextCompanyCode });
            }
            catch (Exception ex)
            {
                // Handle any errors
                return Json(new { success = false, message = "An error occurred while retrieving the next CompanyCode.", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSubContractorById(long id)
        {
            try
            {
                var subContractor = await _subContractorService.GetSubContractorById(id);
                if (subContractor == null)
                {
                    return Json(new { success = false, message = "SubContractor not found." });
                }

                return Json(new { success = true, subContractor });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while retrieving the SubContractor." });
            }
        }
    }
}
