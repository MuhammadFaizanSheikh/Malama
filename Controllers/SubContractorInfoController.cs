using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

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
            List<SubContractorAndContractViewModel> subContractorList = new List<SubContractorAndContractViewModel>();

            try
            {
                subContractorList = await _subContractorService.GetAllSubContractors();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "We encountered an issue while loading subcontractors. Please try again later.";
                // Optionally log the exception
            }

            var viewModel = new SubContractorViewModel
            {
                SubContractor = subContractorList,
                SingleSubContractor = null
            };

            // Pass contracts data to the view
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    foreach (var serviceType in contractDto.SingleSubContractor.SelectedServiceTypeProvided)
                    {
                        var serviceTypeProvided = new ServiceTypeProvided
                        {
                            SubContractorId = contractDto.SingleSubContractor.Id, // Associate with the saved SubContractor
                            ServiceTypeProvidedName = serviceType
                        };

                        contractDto.SingleSubContractor.ServiceTypeProvided.Add(serviceTypeProvided);
                    }

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
        public async Task<IActionResult> GetNextCompanyCode(string companyName)
        {
            try
            {
                // Call the service method to get the next CompanyCode
                string nextCompanyCode = await _subContractorService.GetLastCompanyCode(companyName);

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
                var combinedData = await _subContractorService.GetSubContractorById(id);
                if (combinedData == null)
                {
                    return Json(new { success = false, message = "SubContractor not found." });
                }

                return Json(new { success = true, combinedData });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while retrieving the SubContractor." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSubContractorByCompanyNameForSearching(string companyName)
        {
            try
            {
                var contracts = await _subContractorService.GetSubContractorByCompanyNameForSearching(companyName);

                // Distinct by CompanyMainName
                var result = contracts
                    .GroupBy(c => c.CompanyMainName)
                    .Select(g => g.First())
                    .Select(c => new
                    {
                        id = c.Id,
                        text = c.CompanyMainName
                    })
                    .ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching contracts." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetContractIdsBySubContractorCompanyName(string companyName)
        {
            try
            {
                // Fetch contract details
                var contractDetails = await _subContractorService.GetContractIdsBySubContractorCompanyName(companyName);

                var contractResults = contractDetails.Select(cd => new
                {
                    id = cd.Id,
                    text = cd.ContractName
                }).ToList();

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        contractDetails = contractResults
                    }
                });
            }
            catch (Exception ex)
            {
                // Return an error response if something goes wrong
                return StatusCode(500, new { message = "An error occurred while fetching contract details and StaffID." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanyNameSuggestion(string term)
        {
            try
            {
                var companies = await _subContractorService.GetCompanyNameByTermAsync(term);

                // Return data in the format required by jQuery UI Autocomplete
                return Json(companies.Select(c => c.CompanyMainName).ToList());
            }
            catch (Exception ex)
            {
                // Log the exception and return an appropriate error response
                return StatusCode(500, new { Message = "An error occurred while retrieving company name suggestions." });
            }
        }

    }
}
