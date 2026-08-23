using ExcelFilesCompiler.Interfaces;
using Malama.Attributes;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
                SingleContract = new ContractDetails()
            };

            return View(viewModel);
        }

        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("ContractDetails_Save")]
        public IActionResult Create()
        {
            const string methodName = "Create";
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            var viewModel = new ContractViewModel
            {
                SingleContract = new ContractDetails(),
                SubmissionToken = Guid.NewGuid().ToString()
            };

            ViewBag.FormMode = "Add";
            ViewBag.IsReadOnly = false;
            return View("ContractForm", viewModel);
        }

        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("ContractDetails_Save")]
        public async Task<IActionResult> Edit(long id)
        {
            return await LoadContractFormAsync(id, isReadOnly: false, methodName: "Edit");
        }

        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("ContractDetails_View")]
        public async Task<IActionResult> ViewContract(long id)
        {
            return await LoadContractFormAsync(id, isReadOnly: true, methodName: "ViewContract");
        }

        private async Task<IActionResult> LoadContractFormAsync(long id, bool isReadOnly, string methodName)
        {
            _logger.LogInformation("{ClassName}, {MethodName}, Called. Id={Id}", CLASSNAME, methodName, id);

            try
            {
                var result = await _contractService.GetContractById(id, string.Empty, false);
                if (!result.Success || result.Data == null)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Not Found";
                    TempData["ResponseMessage"] = result.Message ?? "Contract not found.";
                    return RedirectToAction(nameof(Index));
                }

                var contract = result.Data as ContractDetails;
                if (contract == null)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Error";
                    TempData["ResponseMessage"] = "Unable to load contract.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new ContractViewModel
                {
                    SingleContract = contract,
                    SubmissionToken = Guid.NewGuid().ToString()
                };

                ViewBag.FormMode = isReadOnly ? "View" : "Edit";
                ViewBag.IsReadOnly = isReadOnly;
                return View("ContractForm", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Failed to load contract Id={Id}", CLASSNAME, methodName, id);
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = "An unexpected error occurred while loading the contract.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [RoleAttributeAuthorizeFromConfig("ContractDetails_Save")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateContractDetails(ContractViewModel contractDto, string action)
        {
            const string methodName = "CreateContractDetails";
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            contractDto ??= new ContractViewModel();
            contractDto.SingleContract ??= new ContractDetails();

            bool isUpdate = string.Equals(action, "Update", StringComparison.OrdinalIgnoreCase)
                            || contractDto.SingleContract.Id > 0;

            try
            {
                ResponseDto res = new ResponseDto();

                if (contractDto.SingleContract.Id == 0)
                {
                    ModelState.Remove("SingleContract.Id");
                }

                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values)
                    {
                        foreach (var err in error.Errors)
                        {
                            _logger.LogWarning("{ClassName}, {MethodName}, Validation error: {ErrorMessage}",
                                CLASSNAME, methodName, err.ErrorMessage);
                        }
                    }

                    ViewBag.FormMode = isUpdate ? "Edit" : "Add";
                    ViewBag.IsReadOnly = false;
                    return View("ContractForm", contractDto);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, User not authenticated", CLASSNAME, methodName);
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Error";
                    TempData["ResponseMessage"] = "Please login and try again";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Authenticated user: {UserName}, Action: {Action}",
                    CLASSNAME, methodName, user.UserName, action);

                if (string.Equals(action, "Add", StringComparison.OrdinalIgnoreCase))
                {
                    res = await _contractService.AddContractAsync(contractDto.SingleContract, contractDto.SubmissionToken, user.UserName);
                }
                else if (string.Equals(action, "Update", StringComparison.OrdinalIgnoreCase))
                {
                    res = await _contractService.UpdateContract(contractDto.SingleContract, user.UserName);
                }
                else
                {
                    res.Success = false;
                    res.Message = "Invalid form action.";
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Operation completed, Success: {Success}, Message: {Message}",
                    CLASSNAME, methodName, res.Success, res.Message);

                if (!res.Success)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Error";
                    TempData["ResponseMessage"] = res.Message;

                    // Keep all posted values on the form (same idea as station pages)
                    ViewBag.FormMode = isUpdate ? "Edit" : "Add";
                    ViewBag.IsReadOnly = false;
                    return View("ContractForm", contractDto);
                }

                TempData["ResponseStatus"] = "success";
                TempData["ResponseTitle"] = "Success";
                TempData["ResponseMessage"] = res.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Unexpected exception occurred", CLASSNAME, methodName);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = "An unexpected error occurred.";

                ViewBag.FormMode = isUpdate ? "Edit" : "Add";
                ViewBag.IsReadOnly = false;
                return View("ContractForm", contractDto);
            }
        }

        [HttpGet]
        [RoleAttributeAuthorizeFromConfig(
            "ContractDetails_View",
            "SubContractorInfo_View",
            "EventManagement_View")]
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
                    return Json(new { success = false, message = result.Message });
                }

                return Json(new { success = true, contractDetails = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Unexpected exception occurred", CLASSNAME, methodName);
                return Json(new
                {
                    success = false,
                    message = "An unexpected error occurred while processing the request. Please try again later."
                });
            }
        }

        [HttpGet]
        [RoleAttributeAuthorizeFromConfig(
            "ContractDetails_View",
            "SubContractorInfo_View",
            "EventManagement_View")]
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

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Unexpected exception occurred", CLASSNAME, methodName);
                return StatusCode(500, new { message = "An error occurred while fetching contracts." });
            }
        }

        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("ContractDetails_View")]
        public async Task<IActionResult> CheckIfContractExists(string contractId = null, string contractName = null, string checkType = "id")
        {
            string methodName = nameof(CheckIfContractExists);

            try
            {
                var contract = await _contractService.CheckIfContractIDAlreadyExist(contractId, contractName, checkType);
                return Json(new { exists = contract != null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Unexpected exception occurred", CLASSNAME, methodName);
                return StatusCode(500, new { message = "An error occurred while fetching contracts.", error = ex.Message });
            }
        }
    }
}
