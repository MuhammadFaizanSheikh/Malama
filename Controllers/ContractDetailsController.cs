using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
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

        public ContractDetailsController(IContractService contractService, UserManager<ApplicationUser> userManager)
        {
            _contractService = contractService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var responseDto = new ResponseDto();
            List<ContractDetails> contractsList = new List<ContractDetails>();

            try
            {
                contractsList = await _contractService.GetAllContracts();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "We encountered an issue while loading contracts. Please try again later.";
            }

            var viewModel = new ContractViewModel
            {
                Contracts = contractsList,
                SingleContract = null
            };
            // Pass contracts data to the view
            return View(viewModel);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateContractDetails(ContractViewModel contractDto, string action)
        {
            try
            {
                ResponseDto res = new ResponseDto();

                if (contractDto.SingleContract.Id == 0) // Adding a new record
                {
                    ModelState.Remove("SingleContract.Id");
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

                    TempData["ShowForm"] = true;  // Using TempData instead of ViewData
                    return View("Index", contractDto);
                }

                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    if (action == "Add")
                    {
                        res = await _contractService.AddContractAsync(contractDto.SingleContract, user.UserName);
                    }
                    else if (action == "Update")
                    {
                        res = await _contractService.UpdateContract(contractDto.SingleContract, user.UserName);
                    }
                }
                else
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Error";
                    TempData["ResponseMessage"] = "Please login and try again";
                    TempData["ShowForm"] = true;  // Using TempData instead of ViewData
                    TempData["ContractDto"] = contractDto;
                    //return RedirectToAction("Index", contractDto);
                    return RedirectToAction("Index");
                }

                TempData["ResponseStatus"] = res.Success ? "success" : "error"; // SweetAlert2 icon
                TempData["ResponseTitle"] = res.Success ? "Success" : "Error";
                TempData["ResponseMessage"] = res.Message;
                TempData["ShowForm"] = res.Success ? false : true;  // Use TempData
                return RedirectToAction("Index");
                //return RedirectToAction("Index", contractDto);
            }
            catch (Exception ex)
            {
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = "An unexpected error occurred.";
                TempData["ShowForm"] = true;  // Using TempData instead of ViewData
                TempData["ContractDto"] = contractDto;
                return RedirectToAction("Index", contractDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetContractById(long id)
        {
            try
            {
                var result = await _contractService.GetContractById(id);

                if (!result.Success)
                {
                    return Json(new { success = false, message = result.Message });
                }

                // Assuming ResponseDto includes a Data property to hold contract details
                return Json(new { success = true, contractDetails = result.Data });
            }
            catch (Exception ex)
            {
                // Log the exception here if needed, e.g., using a logging framework
                // _logger.LogError(ex, "Error in GetContractById");

                return Json(new
                {
                    success = false,
                    message = "An unexpected error occurred while processing the request. Please try again later."
                });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetContractsForSearching(string contractId)
        {
            try
            {
                var contracts = await _contractService.GetContractForSearchingByContractId(contractId);

                var result = contracts.Select(c => new
                {
                    id = c.Id,
                    text = c.ContractID
                });

                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching contracts." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckIfContractExists(string contractId = null, string contractName = null, string checkType = "id")
        {
            try
            {
                var contract = await _contractService.CheckIfContractIDAlreadyExist(contractId, contractName, checkType);

                if (contract != null)
                {
                    // Return true if the contract exists
                    return Json(new { exists = true });
                }
                else
                {
                    // Return null if no contract is found
                    return Json(new { exists = false });
                }
            }
            catch (Exception ex)
            {
                // Return a server error with a message if there's an exception
                return StatusCode(500, new { message = "An error occurred while fetching contracts.", error = ex.Message });
            }
        }

    }
}
