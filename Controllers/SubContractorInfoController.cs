using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using Malama.Attributes;
using Malama.Models;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ILogger<SubContractorInfoController> _logger;
        private const string CLASSNAME = "SubContractorInfoController";


        public SubContractorInfoController(ILogger<SubContractorInfoController> logger, ISubContractorService subContractorService, UserManager<ApplicationUser> userManager)
        {
            _subContractorService = subContractorService;
            _userManager = userManager;
            _logger = logger;
        }

        [RoleAttributeAuthorizeFromConfig("SubContractorInfo_View")]
        public async Task<IActionResult> Index()
        {
            const string methodName = "Index";
            List<SubContractorAndContractViewModel> subContractorList = new();

            try
            {
                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Loading subcontractors",
                    CLASSNAME, methodName);

                subContractorList = await _subContractorService.GetAllSubContractors();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Subcontractors loaded, Count: {Count}",
                    CLASSNAME, methodName, subContractorList.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Error loading subcontractors",
                    CLASSNAME, methodName);

                TempData["ErrorMessage"] =
                    "We encountered an issue while loading subcontractors. Please try again later.";
            }

            var viewModel = new SubContractorViewModel
            {
                SubContractor = subContractorList,
                SingleSubContractor = null
            };

            return View(viewModel);
        }



        [HttpPost]
        [RoleAttributeAuthorizeFromConfig("SubContractorInfo_Save")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSubContracttor(SubContractorViewModel contractDto, string action)
        {
            const string methodName = nameof(CreateSubContracttor);
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                ResponseDto res = new ResponseDto();

                if (contractDto.SingleSubContractor.Id == 0) // Adding a new record
                {
                    ModelState.Remove("SingleSubContractor.Id");
                }

                if (!ModelState.IsValid)
                {
                    // Log the validation errors
                    foreach (var error in ModelState.Values)
                    {
                        foreach (var err in error.Errors)
                        {
                            _logger.LogWarning("{ClassName}, {MethodName}, Validation error: {ErrorMessage}",
                                CLASSNAME, methodName, err.ErrorMessage);
                        }
                    }

                    return View("Index", contractDto);
                }

                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    foreach (var serviceType in contractDto.SingleSubContractor.SelectedServiceTypeProvided)
                    {
                        var serviceTypeProvided = new ServiceTypeProvided
                        {
                            SubContractorId = contractDto.SingleSubContractor.Id,
                            ServiceTypeProvidedName = serviceType
                        };

                        contractDto.SingleSubContractor.ServiceTypeProvided.Add(serviceTypeProvided);
                    }

                    if (action == "Add")
                    {
                        _logger.LogInformation("{ClassName}, {MethodName}, Adding new SubContractor, User: {UserName}",
                            CLASSNAME, methodName, user.UserName);

                        res = await _subContractorService.AddContractAsync(contractDto.SingleSubContractor, contractDto.SubmissionToken, user.UserName);
                    }
                    else if (action == "Update")
                    {
                        _logger.LogInformation("{ClassName}, {MethodName}, Updating SubContractor, User: {UserName}",
                            CLASSNAME, methodName, user.UserName);

                        res = await _subContractorService.UpdateContract(contractDto.SingleSubContractor, user.UserName);
                    }
                }
                else
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, User not authenticated", CLASSNAME, methodName);

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Error";
                    TempData["ResponseMessage"] = "Please login and try again";
                    return RedirectToAction("Index");
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Operation completed, Success: {Success}, Message: {Message}",
                    CLASSNAME, methodName, res.Success, res.Message);

                TempData["ResponseStatus"] = res.Success ? "success" : "error";
                TempData["ResponseTitle"] = res.Success ? "Success" : "Error";
                TempData["ResponseMessage"] = res.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Unexpected exception occurred", CLASSNAME, methodName);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = "An unexpected error occurred.";
                return RedirectToAction("Index", contractDto);
            }
        }


        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("SubContractorInfo_View")]
        public async Task<IActionResult> GetNextCompanyCode(string companyName)
        {
            const string methodName = nameof(GetNextCompanyCode);
            _logger.LogInformation("{ClassName}, {MethodName}, Called, CompanyName: {CompanyName}",
                CLASSNAME, methodName, companyName);

            try
            {
                // Call the service method to get the next CompanyCode
                string nextCompanyCode = await _subContractorService.GetLastCompanyCode(companyName);

                _logger.LogInformation("{ClassName}, {MethodName}, Retrieved next CompanyCode successfully, NextCompanyCode: {NextCompanyCode}",
                    CLASSNAME, methodName, nextCompanyCode);

                // Return the result as JSON response
                return Json(new { success = true, nextCompanyCode });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error while retrieving next CompanyCode, CompanyName: {CompanyName}",
                    CLASSNAME, methodName, companyName);

                // Handle any errors
                return Json(new { success = false, message = "An error occurred while retrieving the next CompanyCode.", error = ex.Message });
            }
        }


        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("SubContractorInfo_View")]
        public async Task<IActionResult> GetSubContractorById(long id)
        {
            const string methodName = nameof(GetSubContractorById);
            _logger.LogInformation("{ClassName}, {MethodName}, Called, SubContractorId: {Id}",
                CLASSNAME, methodName, id);

            try
            {
                var combinedData = await _subContractorService.GetSubContractorById(id);

                if (combinedData == null)
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, SubContractor not found, Id: {Id}",
                        CLASSNAME, methodName, id);

                    return Json(new { success = false, message = "SubContractor not found." });
                }

                _logger.LogInformation("{ClassName}, {MethodName}, SubContractor retrieved successfully, Id: {Id}",
                    CLASSNAME, methodName, id);

                return Json(new { success = true, combinedData });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error while retrieving SubContractor, Id: {Id}",
                    CLASSNAME, methodName, id);

                return Json(new { success = false, message = "An error occurred while retrieving the SubContractor." });
            }
        }


        [HttpGet]
        //[RoleAttributeAuthorizeFromConfig("SubContractorInfo_View")]
        public async Task<IActionResult> GetSubContractorByCompanyNameForSearching(string companyName)
        {
            const string methodName = nameof(GetSubContractorByCompanyNameForSearching);
            _logger.LogInformation("{ClassName}, {MethodName}, Called, CompanyName: {CompanyName}",
                CLASSNAME, methodName, companyName);

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

                _logger.LogInformation("{ClassName}, {MethodName}, SubContractors fetched successfully, Count: {Count}",
                    CLASSNAME, methodName, result.Count);

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error occurred while fetching SubContractors for CompanyName: {CompanyName}",
                    CLASSNAME, methodName, companyName);

                return StatusCode(500, new { message = "An error occurred while fetching contracts." });
            }
        }


        [HttpGet]
        //[RoleAttributeAuthorizeFromConfig("SubContractorInfo_View")]
        public async Task<IActionResult> GetContractIdsBySubContractorCompanyName(string companyName)
        {
            const string methodName = nameof(GetContractIdsBySubContractorCompanyName);
            _logger.LogInformation(
                "{ClassName}, {MethodName}, Called, CompanyName: {CompanyName}",
                CLASSNAME, methodName, companyName);

            try
            {
                // Fetch contract details
                var contractDetails = await _subContractorService.GetContractIdsBySubContractorCompanyName(companyName);

                var contractResults = contractDetails.Select(cd => new
                {
                    id = cd.Id,
                    text = cd.ContractName
                }).ToList();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Contract details fetched successfully, Count: {Count}",
                    CLASSNAME, methodName, contractResults.Count);

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
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Error occurred while fetching contract details for CompanyName: {CompanyName}",
                    CLASSNAME, methodName, companyName);

                return StatusCode(500, new { message = "An error occurred while fetching contract details and StaffID." });
            }
        }


        [HttpGet]
        //[RoleAttributeAuthorizeFromConfig("SubContractorInfo_View")]
        public async Task<IActionResult> GetCompanyNameSuggestion(string term)
        {
            const string methodName = nameof(GetCompanyNameSuggestion);
            _logger.LogInformation(
                "{ClassName}, {MethodName}, Called, Term: {Term}",
                CLASSNAME, methodName, term);

            try
            {
                var companies = await _subContractorService.GetCompanyNameByTermAsync(term);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Company name suggestions retrieved, Count: {Count}",
                    CLASSNAME, methodName, companies.Count());

                // Return data in the format required by jQuery UI Autocomplete
                return Json(companies.Select(c => c.CompanyMainName).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Error occurred while retrieving company name suggestions, Term: {Term}",
                    CLASSNAME, methodName, term);

                return StatusCode(500, new { Message = "An error occurred while retrieving company name suggestions." });
            }
        }


    }
}
