using ExcelFilesCompiler.Interfaces;
using Malama.Attributes;
using Malama.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;

namespace ExcelFilesCompiler.Controllers
{
    public class ContractDetailsController : Controller
    {
        private readonly IContractService _contractService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ContractDetailsController> _logger;
        private const string CLASSNAME = "ContractDetailsController";

        public ContractDetailsController(ILogger<ContractDetailsController> logger, IContractService contractService, UserManager<ApplicationUser> userManager)
        {
            _contractService = contractService;
            _userManager = userManager;
            _logger = logger;
        }

        [RoleAttributeAuthorizeFromConfig("ContractDetails_View")]
        public async Task<IActionResult> Index()
        {
            const string methodName = "Index";
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            List<ContractDetails> contractsList = new List<ContractDetails>();

            try
            {
                contractsList = await _contractService.GetAllContracts();
                _logger.LogInformation("{ClassName}, {MethodName}, Successfully retrieved contracts, Count: {Count}",
                    CLASSNAME, methodName, contractsList.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Failed to load contracts", CLASSNAME, methodName);
                TempData["ErrorMessage"] = "We encountered an issue while loading contracts. Please try again later.";
            }

            var viewModel = new ContractViewModel
            {
                Contracts = contractsList,
                SingleContract = null
            };

            _logger.LogInformation("{ClassName}, {MethodName}, Returning view with contracts, Count: {Count}",
                CLASSNAME, methodName, contractsList.Count);

            return View(viewModel);
        }


        [HttpPost]
        [RoleAttributeAuthorizeFromConfig("ContractDetails_Save")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateContractDetails(ContractViewModel contractDto, string action)
        {
            const string methodName = "CreateContractDetails";
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                ResponseDto res = new ResponseDto();

                if (contractDto.SingleContract.Id == 0) // Adding a new record
                {
                    ModelState.Remove("SingleContract.Id");
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

                    _logger.LogInformation("{ClassName}, {MethodName}, ModelState invalid, returning view", CLASSNAME, methodName);
                    return View("Index", contractDto);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, Authenticated user: {UserName}, Action: {Action}",
                        CLASSNAME, methodName, user.UserName, action);

                    if (action == "Add")
                    {
                        res = await _contractService.AddContractAsync(contractDto.SingleContract, contractDto.SubmissionToken, user.UserName);
                        _logger.LogInformation("{ClassName}, {MethodName}, AddContractAsync completed, Success: {Success}, Message: {Message}",
                            CLASSNAME, methodName, res.Success, res.Message);
                    }
                    else if (action == "Update")
                    {
                        res = await _contractService.UpdateContract(contractDto.SingleContract, user.UserName);
                        _logger.LogInformation("{ClassName}, {MethodName}, UpdateContract completed, Success: {Success}, Message: {Message}",
                            CLASSNAME, methodName, res.Success, res.Message);
                    }
                }
                else
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, User not authenticated, redirecting to Index", CLASSNAME, methodName);

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Error";
                    TempData["ResponseMessage"] = "Please login and try again";
                    TempData["ContractDto"] = contractDto;

                    return RedirectToAction("Index");
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Operation completed, Success: {Success}, Message: {Message}",
                    CLASSNAME, methodName, res.Success, res.Message);

                TempData["ResponseStatus"] = res.Success ? "success" : "error"; // SweetAlert2 icon
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
                TempData["ContractDto"] = contractDto;

                return RedirectToAction("Index", contractDto);
            }
        }


        [HttpGet]
        //[RoleAttributeAuthorizeFromConfig("ContractDetails_View")]
        public async Task<IActionResult> GetContractById(long id, string companyName = null, bool checkIfContractAlreadyExist = false)
        {
            string methodName = nameof(GetContractById);

            _logger.LogInformation(
                "{ClassName}, {MethodName}, Request received. Id: {Id}, CompanyName: {CompanyName}, CheckExists: {CheckExists}",
                CLASSNAME, methodName, id, companyName, checkIfContractAlreadyExist);

            try
            {
                var result = await _contractService.GetContractById(id, companyName, checkIfContractAlreadyExist);

                if (!result.Success)
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Operation completed, Success: {Success}, Message: {Message}",
                        CLASSNAME, methodName, result.Success, result.Message);

                    return Json(new { success = false, message = result.Message });
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Operation completed, Success: {Success}, Message: {Message}",
                    CLASSNAME, methodName, result.Success, result.Message);

                return Json(new { success = true, contractDetails = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Unexpected exception occurred",
                    CLASSNAME, methodName);

                return Json(new
                {
                    success = false,
                    message = "An unexpected error occurred while processing the request. Please try again later."
                });
            }
        }




        [HttpGet]
        //[RoleAttributeAuthorizeFromConfig("ContractDetails_View")]
        public async Task<IActionResult> GetContractsForSearching(string contractId)
        {
            string methodName = nameof(GetContractsForSearching);

            _logger.LogInformation(
                "{ClassName}, {MethodName}, Request received. ContractId filter: {ContractId}",
                CLASSNAME, methodName, contractId);

            try
            {
                var contracts = _contractService.GetContractForSearchingByContractId(contractId);

                var result = contracts.Select(c => new
                {
                    id = c.Id,
                    text = c.ContractName
                });

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Operation completed, Success: {Success}, ReturnedCount: {Count}",
                    CLASSNAME, methodName, true, result.Count());

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Unexpected exception occurred",
                    CLASSNAME, methodName);

                return StatusCode(500, new { message = "An error occurred while fetching contracts." });
            }
        }


        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("ContractDetails_View")]
        public async Task<IActionResult> CheckIfContractExists(string contractId = null, string contractName = null, string checkType = "id")
        {
            string methodName = nameof(CheckIfContractExists);

            _logger.LogInformation(
                "{ClassName}, {MethodName}, Request received. ContractId: {ContractId}, ContractName: {ContractName}, CheckType: {CheckType}",
                CLASSNAME, methodName, contractId, contractName, checkType);

            try
            {
                var contract = await _contractService.CheckIfContractIDAlreadyExist(contractId, contractName, checkType);

                bool exists = (contract != null);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Operation completed, Exists: {Exists}",
                    CLASSNAME, methodName, exists);

                return Json(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Unexpected exception occurred",
                    CLASSNAME, methodName);

                return StatusCode(500, new { message = "An error occurred while fetching contracts.", error = ex.Message });
            }
        }


    }
}
